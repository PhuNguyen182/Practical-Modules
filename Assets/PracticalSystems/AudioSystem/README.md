# Professional Audio System for Unity

A comprehensive, production-ready audio management system for Unity following SOLID principles and best practices.

## Features

### ✅ Core Features
- **Multiple Audio Types**: Background Music, Ambient, SFX, Voice, UI, Cinematic, etc.
- **Full Playback Control**: Volume, pitch, position, spatial blend, and more
- **Fade In/Out**: Smooth transitions with customizable durations
- **Pause/Resume**: Individual or group-based control with fade support
- **AudioMixer Integration**: Complete control over mixer groups and parameters
- **Object Pooling**: Efficient GameObject reuse for audio sources
- **Frequency Control**: Prevents audio spam for high-rate sounds (gunfire, etc.)
- **3D Spatial Audio**: Positional and attached audio support
- **Addressables Support**: Optional on-demand loading for large audio libraries

### 🏗️ Architecture
- **No Singleton**: Dependency injection friendly
- **SOLID Principles**: Clean, maintainable, extensible code
- **Interface-Based**: Easy to test and mock
- **Async/Await**: Modern async patterns with UniTask
- **Performance Optimized**: Object pooling, efficient lookups, minimal allocations

## Installation

1. **Prerequisites**:
   - Unity 2021.3 or higher
   - UniTask package (com.cysharp.unitask)
   - Optional: Addressables package (com.unity.addressables)

2. **Import the Audio System**:
   - Copy the `AudioSystem` folder to your project's `Assets/PracticalSystems/` directory

3. **Setup AudioMixer**:
   - Create an AudioMixer asset in Unity
   - Create mixer groups for each AudioType you want to use
   - Expose volume parameters for each group

## Quick Start

### 1. Create Configuration Assets

#### Audio Mixer Config
```
Right-click in Project → Create → Foundations/Audio/Audio Mixer Config
```
- Assign your AudioMixer
- Map audio types to mixer groups
- Configure volume parameters

#### Audio Database Config
```
Right-click in Project → Create → Foundations/Audio/Audio Database Config
```
- Add audio entries with IDs and clips
- Configure playback parameters
- Set frequency limits

### 2. Setup Audio Manager

1. Create an empty GameObject in your scene: `AudioManager`
2. Add the `AudioManager` component
3. Assign the configuration assets
4. Assign an Audio Player prefab (or it will create one automatically)

### 3. Play Audio

```csharp
using Foundations.Audio.Interfaces;
using Foundations.Audio.Data;
using Cysharp.Threading.Tasks;

public class MyGameScript : MonoBehaviour
{
    [SerializeField] private GameObject audioManagerObject;
    private IAudioManager audioManager;
    
    private void Start()
    {
        this.audioManager = this.audioManagerObject.GetComponent<IAudioManager>();
    }
    
    private async UniTaskVoid PlayBackgroundMusic()
    {
        var parameters = AudioPlaybackParameters.WithFadeIn(2f);
        parameters.overrideLoop = true;
        parameters.loop = true;
        
        var handle = await this.audioManager.PlayAudioAsync(
            "bgm_main_theme",
            parameters
        );
    }
    
    private async UniTaskVoid PlaySoundEffect()
    {
        var parameters = AudioPlaybackParameters.WithRandomPitch(0.1f);
        
        await this.audioManager.PlayAudioAsync(
            "sfx_button_click",
            parameters
        );
    }
}
```

## Usage Examples

### Basic Audio Playback

```csharp
// Simple playback
await audioManager.PlayAudioAsync("sfx_explosion");

// With custom parameters
var parameters = new AudioPlaybackParameters
{
    volumeMultiplier = 0.8f,
    pitchMultiplier = 1.2f,
    fadeInDuration = 1f
};
await audioManager.PlayAudioAsync("bgm_battle", parameters);
```

### 3D Positional Audio

```csharp
// Play at specific position
Vector3 explosionPos = new Vector3(10, 0, 5);
await audioManager.PlayAudioAtPositionAsync(
    "sfx_explosion",
    explosionPos
);

// Attach to moving object
await audioManager.PlayAudioAttachedAsync(
    "sfx_engine_running",
    carTransform
);
```

### Individual Audio Control

```csharp
var handle = await audioManager.PlayAudioAsync("sfx_ambient_wind");

// Control volume
handle.SetVolume(0.5f);

// Fade volume
await handle.FadeVolumeAsync(1f, 2f);

// Change pitch
handle.SetPitch(0.8f);

// Pause/Resume
handle.Pause(0.5f); // Fade out
handle.Resume(0.5f); // Fade in

// Stop
handle.Stop(1f); // Fade out over 1 second
```

### Group Control

```csharp
// Pause all sound effects
audioManager.PauseAudioType(AudioType.SoundEffect, 0.5f);

// Resume all sound effects
audioManager.ResumeAudioType(AudioType.SoundEffect, 0.5f);

// Stop all background music
audioManager.StopAudioType(AudioType.BackgroundMusic, 2f);

// Pause everything
audioManager.PauseAllAudio(1f);

// Stop everything
audioManager.StopAllAudio(2f);
```

### Volume Control

```csharp
// Set master volume
audioManager.SetMasterVolume(0.8f);

// Set audio type volumes
audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, 0.6f);
audioManager.SetAudioTypeVolume(AudioType.SoundEffect, 0.9f);

// Get current volumes
float masterVol = audioManager.GetMasterVolume();
float bgmVol = audioManager.GetAudioTypeVolume(AudioType.BackgroundMusic);
```

### High-Frequency Audio (Weapon Fire)

The system automatically handles high-frequency audio to prevent spam:

```csharp
// Fire rapidly - system will limit overlapping sounds
for (int i = 0; i < 20; i++)
{
    var parameters = AudioPlaybackParameters.WithRandomPitch(0.05f);
    await audioManager.PlayAudioAsync("sfx_gun_shot", parameters);
    await UniTask.Delay(50); // 20 rounds per second
}
```

Configure in Audio Entry:
- `minTimeBetweenPlays`: Minimum time between consecutive plays (e.g., 0.1s)
- `maxConcurrentInstances`: Maximum simultaneous instances (e.g., 3)

### Random Variation

```csharp
var parameters = new AudioPlaybackParameters
{
    useRandomPitch = true,
    randomPitchRange = 0.1f,  // ±10% pitch variation
    useRandomVolume = true,
    randomVolumeRange = 0.1f  // ±10% volume variation
};

await audioManager.PlayAudioAsync("sfx_footstep", parameters);
```

## Audio Entry Configuration

### Direct Reference Entry

```csharp
var entry = new AudioEntry(
    audioId: "sfx_explosion",
    audioType: AudioType.SoundEffect,
    audioClip: explosionClip,
    defaultVolume: 0.8f,
    defaultPitch: 1f,
    isLooping: false,
    priority: 128,
    minTimeBetweenPlays: 0.1f,      // Min 100ms between plays
    maxConcurrentInstances: 3,       // Max 3 simultaneous instances
    spatialBlend: 1f,                // Full 3D
    minDistance: 1f,
    maxDistance: 50f
);
```

### Addressable Entry (Optional)

For large audio libraries (1000+ sounds), use Addressables:

```csharp
var entry = new AddressableAudioEntry
{
    audioId = "bgm_main_theme",
    audioType = AudioType.BackgroundMusic,
    audioClipReference = addressableReference,
    // ... other parameters
};

// Audio will be loaded on-demand
var handle = await audioManager.PlayAudioAsync("bgm_main_theme");

// Unload when no longer needed
audioDatabase.UnloadAudioEntries(new[] { "bgm_main_theme" });
```

## AudioMixer Integration

### Setup Mixer Groups

1. Create AudioMixer in Unity
2. Create groups: Master, BGM, SFX, Voice, UI, etc.
3. Expose parameters: `MasterVolume`, `BGMVolume`, `SFXVolume`, etc.

### Configure in AudioMixerConfig

```csharp
// Map audio types to mixer groups
[SerializeField] AudioTypeMixerMapping[] mappings = new[]
{
    new AudioTypeMixerMapping
    {
        audioType = AudioType.BackgroundMusic,
        mixerGroup = bgmGroup,
        volumeParameterName = "BGMVolume",
        defaultVolume = 0.7f
    },
    new AudioTypeMixerMapping
    {
        audioType = AudioType.SoundEffect,
        mixerGroup = sfxGroup,
        volumeParameterName = "SFXVolume",
        defaultVolume = 0.8f
    }
};
```

## Performance Considerations

### Object Pooling

The system uses object pooling for AudioSource GameObjects:

```csharp
// Configure initial pool size
audioDatabase.InitialPoolSize = 20; // Preload 20 players

// Pool grows automatically as needed
// Returns to pool when audio completes
```

### Memory Management

```csharp
// With Addressables - unload unused audio
audioDatabase.UnloadAudioEntries(new[] 
{ 
    "level1_bgm", 
    "level1_ambience" 
});

// Preload audio before gameplay
await audioDatabase.PreloadAudioEntriesAsync(new[] 
{ 
    "level2_bgm", 
    "level2_ambience" 
});
```

### Frequency Control

Prevents audio spam without manual management:

```csharp
// In Audio Entry Config:
minTimeBetweenPlays = 0.1f;      // 100ms cooldown
maxConcurrentInstances = 3;       // Max 3 at once

// System automatically:
// 1. Blocks rapid-fire plays within cooldown
// 2. Stops oldest instance when limit reached
// 3. No duplicate sounds playing simultaneously
```

## API Reference

### IAudioManager

Main interface for audio control:

- `PlayAudioAsync(audioId, parameters)` - Play audio
- `PlayAudioAtPositionAsync(audioId, position, parameters)` - Play 3D audio
- `PlayAudioAttachedAsync(audioId, transform, parameters)` - Play attached audio
- `StopAudioType(audioType, fadeOut)` - Stop audio by type
- `StopAllAudio(fadeOut)` - Stop all audio
- `PauseAudioType(audioType, fadeOut)` - Pause by type
- `PauseAllAudio(fadeOut)` - Pause all
- `ResumeAudioType(audioType, fadeIn)` - Resume by type
- `ResumeAllAudio(fadeIn)` - Resume all
- `SetAudioTypeVolume(audioType, volume)` - Set type volume
- `SetMasterVolume(volume)` - Set master volume

### IAudioHandle

Individual audio control:

- `IsPlaying` - Check if playing
- `IsPaused` - Check if paused
- `CurrentTime` - Get playback time
- `Duration` - Get total duration
- `Stop(fadeOut)` - Stop playback
- `Pause(fadeOut)` - Pause playback
- `Resume(fadeIn)` - Resume playback
- `SetVolume(volume)` - Set volume
- `SetPitch(pitch)` - Set pitch
- `FadeVolumeAsync(target, duration)` - Fade volume
- `WaitForCompletionAsync()` - Wait until finished

## Troubleshooting

### Audio Not Playing

1. Check AudioDatabaseConfig has the audio entry
2. Verify AudioClip is assigned
3. Check mixer groups are set up correctly
4. Verify volume levels (master and type)
5. Check frequency limits aren't blocking playback

### Performance Issues

1. Increase initial pool size
2. Use Addressables for large libraries
3. Set appropriate maxConcurrentInstances
4. Unload unused audio entries
5. Profile with Unity Profiler

### Addressables Not Loading

1. Verify Addressables package is installed
2. Check audio clips are marked as Addressable
3. Ensure AssetReferences are assigned
4. Check for loading errors in console

## Best Practices

1. **Use Audio Types**: Organize audio by type for easy control
2. **Set Frequency Limits**: Prevent spam on high-rate audio
3. **Fade Transitions**: Use fade in/out for smooth audio experience
4. **3D Audio Settings**: Configure min/max distance appropriately
5. **Preload Critical Audio**: Load menu/gameplay audio on scene start
6. **Unload Level Audio**: Free memory when changing scenes
7. **Use Handles**: Keep references for important audio (BGM, ambient)
8. **Random Variation**: Add variety to repetitive sounds
9. **Mixer Groups**: Use mixer for flexible audio balancing
10. **Test on Target**: Test audio performance on target devices

## License

This audio system is part of the Practical Game Modules project.

## Support

For issues, questions, or contributions, please refer to the main project repository.

