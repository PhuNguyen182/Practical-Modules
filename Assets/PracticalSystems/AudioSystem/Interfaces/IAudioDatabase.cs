using System.Threading;
using Cysharp.Threading.Tasks;

namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Interface for audio database that manages all audio entries
    /// Supports both direct references and Addressables
    /// </summary>
    public interface IAudioDatabase
    {
        /// <summary>
        /// Gets an audio entry by ID
        /// </summary>
        public IAudioEntry GetAudioEntry(string audioId);
        
        /// <summary>
        /// Loads an audio entry asynchronously (for Addressables)
        /// </summary>
        public UniTask<IAudioEntry> LoadAudioEntryAsync(string audioId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Checks if an audio entry exists
        /// </summary>
        public bool HasAudioEntry(string audioId);
        
        /// <summary>
        /// Preloads audio entries for better performance
        /// </summary>
        public UniTask PreloadAudioEntriesAsync(string[] audioIds, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Unloads audio entries to free memory
        /// </summary>
        public void UnloadAudioEntries(string[] audioIds);
    }
}

