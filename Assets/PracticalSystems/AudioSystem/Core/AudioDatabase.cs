using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Manages audio entries with efficient lookup and optional Addressables support
    /// Optimized for large audio databases (1000+ entries)
    /// </summary>
    public class AudioDatabase : IAudioDatabase
    {
        private readonly AudioDatabaseConfig _config;
        private readonly Dictionary<string, IAudioEntry> _audioEntryLookup;
        private readonly HashSet<string> _loadedEntries;
        
        public AudioDatabase(AudioDatabaseConfig config)
        {
            this._config = config;
            this._audioEntryLookup = new Dictionary<string, IAudioEntry>();
            this._loadedEntries = new HashSet<string>();
            
            this.InitializeDatabase();
        }
        
        /// <summary>
        /// Initializes the database with audio entries
        /// </summary>
        private void InitializeDatabase()
        {
            foreach (var entry in this._config.AudioEntries)
            {
                if (string.IsNullOrEmpty(entry.AudioId))
                {
                    Debug.LogWarning("AudioDatabase: Found audio entry with empty ID, skipping");
                    continue;
                }
                
                if (this._audioEntryLookup.ContainsKey(entry.AudioId))
                {
                    Debug.LogWarning($"AudioDatabase: Duplicate audio ID '{entry.AudioId}', skipping");
                    continue;
                }
                
                this._audioEntryLookup.Add(entry.AudioId, entry);
            }
            
            Debug.Log($"AudioDatabase: Initialized with {this._audioEntryLookup.Count} audio entries");
        }
        
        public IAudioEntry GetAudioEntry(string audioId)
        {
            if (string.IsNullOrEmpty(audioId))
            {
                Debug.LogWarning("AudioDatabase: Attempted to get audio entry with null or empty ID");
                return null;
            }
            
            if (this._audioEntryLookup.TryGetValue(audioId, out var entry))
            {
                return entry;
            }
            
            Debug.LogWarning($"AudioDatabase: Audio entry '{audioId}' not found");
            return null;
        }
        
        public async UniTask<IAudioEntry> LoadAudioEntryAsync(string audioId, CancellationToken cancellationToken = default)
        {
            if (this._config.UseAddressables)
            {
                // Addressables implementation will be added in optional section
                Debug.LogWarning("AudioDatabase: Addressables support not yet implemented");
                return this.GetAudioEntry(audioId);
            }
            
            // For non-Addressables, return immediately
            await UniTask.Yield(cancellationToken);
            return this.GetAudioEntry(audioId);
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
            
            if (this._config.UseAddressables)
            {
                // Addressables preloading will be implemented in optional section
                Debug.Log($"AudioDatabase: Preloading {audioIds.Length} audio entries (Addressables not yet implemented)");
                await UniTask.Yield(cancellationToken);
                return;
            }
            
            // For direct references, audio clips are already loaded
            await UniTask.Yield(cancellationToken);
            
            foreach (var audioId in audioIds)
            {
                if (this.HasAudioEntry(audioId))
                {
                    this._loadedEntries.Add(audioId);
                }
            }
            
            Debug.Log($"AudioDatabase: Preloaded {audioIds.Length} audio entries");
        }
        
        public void UnloadAudioEntries(string[] audioIds)
        {
            if (audioIds == null || audioIds.Length == 0)
            {
                return;
            }
            
            if (this._config.UseAddressables)
            {
                // Addressables unloading will be implemented in optional section
                Debug.Log($"AudioDatabase: Unloading {audioIds.Length} audio entries (Addressables not yet implemented)");
                return;
            }
            
            // For direct references, no unloading needed
            foreach (var audioId in audioIds)
            {
                this._loadedEntries.Remove(audioId);
            }
        }
    }
}

