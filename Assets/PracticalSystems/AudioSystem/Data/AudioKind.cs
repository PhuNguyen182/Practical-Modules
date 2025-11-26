using System;

namespace PracticalSystems.AudioSystem.Data
{
    /// <summary>
    /// Enum defining different types of audio in the game
    /// Used for categorization, mixing, and volume control
    /// </summary>
    [Serializable]
    public enum AudioKind
    {
        /// <summary>
        /// Background music tracks
        /// </summary>
        BackgroundMusic = 0,
        
        /// <summary>
        /// Ambient environmental sounds
        /// </summary>
        Ambient = 1,
        
        /// <summary>
        /// Sound effects (gameplay actions)
        /// </summary>
        SoundEffect = 2,
        
        /// <summary>
        /// Voice acting and dialogue
        /// </summary>
        Voice = 3,
        
        /// <summary>
        /// User interface sounds
        /// </summary>
        UserInterface = 4,
        
        /// <summary>
        /// Cinematic or cutscene audio
        /// </summary>
        Cinematic = 5,
        
        /// <summary>
        /// Music stingers or short musical cues
        /// </summary>
        MusicStinger = 6,
        
        /// <summary>
        /// Footsteps and movement sounds
        /// </summary>
        Footstep = 7,
        
        /// <summary>
        /// Weapon sounds (separate from general SFX)
        /// </summary>
        Weapon = 8,
        
        /// <summary>
        /// Environmental interaction sounds
        /// </summary>
        Environment = 9
    }
}

