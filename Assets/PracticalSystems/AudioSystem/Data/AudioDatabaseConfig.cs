using System.Collections.Generic;
using PracticalSystems.AudioSystem.Addressables;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// ScriptableObject configuration for audio database
    /// Supports large collections of audio entries with efficient lookup
    /// </summary>
    [CreateAssetMenu(fileName = "AudioDatabaseConfig", menuName = "Foundations/Audio/Audio Database Config")]
    public class AudioDatabaseConfig : ScriptableObject
    {
        [Header("Audio Entries")] [SerializeField]
        private List<AudioEntry> audioEntries = new();

        [Header("Addressable Audio Entries")] [SerializeField]
        private List<AddressableAudioEntry> addressableAudioEntries = new();

        [Header("Database Settings")] [SerializeField]
        private bool useAddressables;

        [SerializeField] private bool preloadOnStart = true;
        [SerializeField] private int initialPoolSize = 10;

        public IReadOnlyList<AudioEntry> AudioEntries => this.audioEntries;
        public IReadOnlyList<AddressableAudioEntry> AddressableAudioEntries => this.addressableAudioEntries;
        public bool UseAddressables => this.useAddressables;
        public bool PreloadOnStart => this.preloadOnStart;
        public int InitialPoolSize => this.initialPoolSize;

#if UNITY_EDITOR
        /// <summary>
        /// Validates audio entries to ensure no duplicate IDs
        /// </summary>
        private void OnValidate()
        {
            var uniqueIds = new HashSet<string>();
            var duplicates = new List<string>();

            // Validate regular audio entries
            foreach (var entry in this.audioEntries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.AudioId))
                {
                    continue;
                }

                if (!uniqueIds.Add(entry.AudioId))
                {
                    duplicates.Add(entry.AudioId);
                }
            }

            // Validate addressable audio entries
            foreach (var entry in this.addressableAudioEntries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.AudioId))
                {
                    continue;
                }

                if (!uniqueIds.Add(entry.AudioId))
                {
                    duplicates.Add(entry.AudioId);
                }
            }

            if (duplicates.Count > 0)
            {
                Debug.LogWarning($"AudioDatabaseConfig has duplicate audio IDs: {string.Join(", ", duplicates)}", this);
            }
        }
#endif
    }
}

