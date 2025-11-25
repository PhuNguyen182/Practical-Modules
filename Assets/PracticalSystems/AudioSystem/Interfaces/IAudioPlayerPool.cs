namespace PracticalSystems.AudioSystem.Interfaces
{
    /// <summary>
    /// Interface for audio player object pooling
    /// </summary>
    public interface IAudioPlayerPool
    {
        /// <summary>
        /// Gets an audio player from the pool
        /// </summary>
        public IAudioPlayer GetAudioPlayer();
        
        /// <summary>
        /// Returns an audio player to the pool
        /// </summary>
        public void ReturnAudioPlayer(IAudioPlayer audioPlayer);
        
        /// <summary>
        /// Preloads audio players for better performance
        /// </summary>
        public void PreloadPlayers(int count);
        
        /// <summary>
        /// Gets the number of active audio players
        /// </summary>
        public int ActivePlayerCount { get; }
        
        /// <summary>
        /// Gets the number of pooled audio players
        /// </summary>
        public int PooledPlayerCount { get; }
    }
}

