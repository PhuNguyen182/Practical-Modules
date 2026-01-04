using System.Collections.Generic;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Object pool for audio players
    /// Optimizes performance by reusing AudioSource GameObjects
    /// </summary>
    public class AudioPlayerPool : IAudioPlayerPool
    {
        private readonly GameObject _audioPlayerPrefab;
        private readonly Transform _poolParent;
        private readonly Queue<IAudioPlayer> _availablePlayers;
        private readonly HashSet<IAudioPlayer> _activePlayers;
        private readonly IAudioMixerController _mixerController;
        private int _nextPlayerId;
        
        public int ActivePlayerCount => this._activePlayers.Count;
        public int PooledPlayerCount => this._availablePlayers.Count;
        
        public AudioPlayerPool(
            GameObject audioPlayerPrefab, 
            Transform poolParent, 
            IAudioMixerController mixerController,
            int initialPoolSize = 10)
        {
            this._audioPlayerPrefab = audioPlayerPrefab;
            this._poolParent = poolParent;
            this._mixerController = mixerController;
            this._availablePlayers = new Queue<IAudioPlayer>();
            this._activePlayers = new HashSet<IAudioPlayer>();
            this._nextPlayerId = 0;
            
            this.PreloadPlayers(initialPoolSize);
        }
        
        public IAudioPlayer GetAudioPlayer()
        {
            IAudioPlayer player;
            
            if (this._availablePlayers.Count > 0)
            {
                player = this._availablePlayers.Dequeue();
            }
            else
            {
                player = this.CreateNewPlayer();
            }
            
            this._activePlayers.Add(player);
            player.Transform.gameObject.SetActive(true);
            
            return player;
        }
        
        public void ReturnAudioPlayer(IAudioPlayer audioPlayer)
        {
            if (audioPlayer == null)
            {
                return;
            }
            
            if (!this._activePlayers.Remove(audioPlayer))
            {
                Debug.LogWarning("AudioPlayerPool: Attempted to return player that was not active");
                return;
            }
            
            audioPlayer.Reset();
            audioPlayer.Transform.gameObject.SetActive(false);
            this._availablePlayers.Enqueue(audioPlayer);
        }
        
        public void PreloadPlayers(int count)
        {
            if (count <= 0)
            {
                return;
            }
            
            for (int i = 0; i < count; i++)
            {
                var player = this.CreateNewPlayer();
                player.Transform.gameObject.SetActive(false);
                this._availablePlayers.Enqueue(player);
            }
            
            Debug.Log($"AudioPlayerPool: Preloaded {count} audio players (Total: {this._availablePlayers.Count})");
        }
        
        /// <summary>
        /// Creates a new audio player instance
        /// </summary>
        private IAudioPlayer CreateNewPlayer()
        {
            var playerObject = Object.Instantiate(this._audioPlayerPrefab, this._poolParent);
            playerObject.name = $"AudioPlayer_{this._nextPlayerId++}";
            var player = playerObject.GetComponent<IAudioPlayer>() ?? playerObject.AddComponent<AudioPlayer>();
            player.InitializePoolService(this);
            return player;
        }
        
        /// <summary>
        /// Configures the audio player with appropriate mixer group
        /// </summary>
        public void ConfigurePlayerMixerGroup(IAudioPlayer player, AudioKind audioKind)
        {
            if (player == null || player.AudioSource == null)
            {
                return;
            }
            
            var mixerGroup = this._mixerController.GetMixerGroup(audioKind);
            if (mixerGroup != null)
            {
                player.AudioSource.outputAudioMixerGroup = mixerGroup;
            }
        }
    }
}

