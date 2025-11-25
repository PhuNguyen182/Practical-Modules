using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Addressables
{
    /// <summary>
    /// Audio database implementation using Addressables
    /// Provides on-demand loading and memory management for large audio libraries
    /// </summary>
    public class AddressableAudioDatabase : IAudioDatabase
    {
        private readonly AudioDatabaseConfig _audioDatabaseConfig;
        private readonly Dictionary<string, AddressableAudioEntry> _audioEntryLookup;
        private readonly Dictionary<string, AsyncOperationHandle<AudioClip>> _loadOperations;
        private readonly HashSet<string> _loadedEntries;
        
        public AddressableAudioDatabase(AudioDatabaseConfig audioDatabaseConfig)
        {
            this._audioDatabaseConfig = audioDatabaseConfig;
            this._audioEntryLookup = new Dictionary<string, AddressableAudioEntry>();
            this._loadOperations = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
            this._loadedEntries = new HashSet<string>();
            
            this.InitializeDatabase();
        }
        
        /// <summary>
        /// Initializes the database with addressable audio entries
        /// </summary>
        private void InitializeDatabase()
        {
            // Process addressable audio entries
            foreach (var addressableEntry in this._audioDatabaseConfig.AddressableAudioEntries)
            {
                if (addressableEntry == null)
                {
                    Debug.LogWarning("AddressableAudioDatabase: Found null addressable entry, skipping");
                    continue;
                }
                
                if (string.IsNullOrEmpty(addressableEntry.AudioId))
                {
                    Debug.LogWarning("AddressableAudioDatabase: Found audio entry with empty ID, skipping");
                    continue;
                }
                
                if (this._audioEntryLookup.ContainsKey(addressableEntry.AudioId))
                {
                    Debug.LogWarning($"AddressableAudioDatabase: Duplicate audio ID '{addressableEntry.AudioId}', skipping");
                    continue;
                }
                
                this._audioEntryLookup.Add(addressableEntry.AudioId, addressableEntry);
            }
            
            Debug.Log($"AddressableAudioDatabase: Initialized with {this._audioEntryLookup.Count} addressable audio entries");
        }
        
        public IAudioEntry GetAudioEntry(string audioId)
        {
            if (string.IsNullOrEmpty(audioId))
            {
                Debug.LogWarning("AddressableAudioDatabase: Attempted to get audio entry with null or empty ID");
                return null;
            }
            
            if (this._audioEntryLookup.TryGetValue(audioId, out var entry))
            {
                return entry;
            }
            
            Debug.LogWarning($"AddressableAudioDatabase: Audio entry '{audioId}' not found");
            return null;
        }
        
        public async UniTask<IAudioEntry> LoadAudioEntryAsync(string audioId, CancellationToken cancellationToken = default)
        {
            if (!this._audioEntryLookup.TryGetValue(audioId, out var entry))
            {
                Debug.LogWarning($"AddressableAudioDatabase: Audio entry '{audioId}' not found");
                return null;
            }
            
            // If already loaded, return immediately
            if (entry.IsLoaded)
            {
                return entry;
            }
            
            // If currently loading, wait for existing operation
            if (this._loadOperations.TryGetValue(audioId, out var existingOperation))
            {
                await existingOperation.Task;
                return entry;
            }
            
            // Load the audio clip
            try
            {
                var loadOperation = entry.AudioClipReference.LoadAssetAsync();
                this._loadOperations[audioId] = loadOperation;
                
                var audioClip = await loadOperation.Task.AsUniTask().AttachExternalCancellation(cancellationToken);
                
                entry.SetLoadedClip(audioClip);
                this._loadedEntries.Add(audioId);
                
                Debug.Log($"AddressableAudioDatabase: Loaded audio '{audioId}'");
                
                return entry;
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log($"AddressableAudioDatabase: Loading of audio '{audioId}' was cancelled");
                return null;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AddressableAudioDatabase: Failed to load audio '{audioId}': {ex.Message}");
                return null;
            }
            finally
            {
                this._loadOperations.Remove(audioId);
            }
        }
        
        public bool HasAudioEntry(string audioId)
        {
            if (string.IsNullOrEmpty(audioId))
            {
                return false;
            }
            
            return this._audioEntryLookup.ContainsKey(audioId);
        }
        
        public async UniTask PreloadAudioEntriesAsync(string[] audioIds, CancellationToken cancellationToken = default)
        {
            if (audioIds == null || audioIds.Length == 0)
            {
                return;
            }
            
            var loadTasks = new List<UniTask<IAudioEntry>>();
            
            foreach (var audioId in audioIds)
            {
                if (this.HasAudioEntry(audioId) && !this._loadedEntries.Contains(audioId))
                {
                    loadTasks.Add(this.LoadAudioEntryAsync(audioId, cancellationToken));
                }
            }
            
            if (loadTasks.Count > 0)
            {
                await UniTask.WhenAll(loadTasks);
                Debug.Log($"AddressableAudioDatabase: Preloaded {loadTasks.Count} audio entries");
            }
        }
        
        public void UnloadAudioEntries(string[] audioIds)
        {
            if (audioIds == null || audioIds.Length == 0)
            {
                return;
            }
            
            foreach (var audioId in audioIds)
            {
                if (!this._audioEntryLookup.TryGetValue(audioId, out var entry))
                {
                    continue;
                }
                
                if (entry.AudioClipReference.IsValid())
                {
                    entry.AudioClipReference.ReleaseAsset();
                    entry.SetLoadedClip(null);
                    this._loadedEntries.Remove(audioId);
                    
                    Debug.Log($"AddressableAudioDatabase: Unloaded audio '{audioId}'");
                }
            }
        }
        
        /// <summary>
        /// Unloads all loaded audio entries
        /// </summary>
        public void UnloadAll()
        {
            var loadedIds = this._loadedEntries.ToArray();
            this.UnloadAudioEntries(loadedIds);
        }
    }
}

