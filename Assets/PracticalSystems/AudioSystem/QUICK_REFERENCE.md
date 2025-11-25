# Audio System - Quick Reference

## Setup Checklist

- [ ] Install UniTask package
- [ ] Create AudioMixer with groups
- [ ] Expose volume parameters
- [ ] Create AudioMixerConfig asset
- [ ] Create AudioDatabaseConfig asset
- [ ] Add audio entries to database
- [ ] Create AudioManager in scene
- [ ] Create AudioPlayer prefab
- [ ] Assign configs to AudioManager
- [ ] Test in Play mode

## Common Code Patterns

### Basic Playback
```csharp
// Simple
await audioManager.PlayAudioAsync("audio_id");

// With parameters
var params = AudioPlaybackParameters.Default;
params.volumeMultiplier = 0.8f;
await audioManager.PlayAudioAsync("audio_id", params);
```

### Fade In/Out
```csharp
// Fade in
var params = AudioPlaybackParameters.WithFadeIn(2f);
var handle = await audioManager.PlayAudioAsync("audio_id", params);

// Fade out
handle.Stop(2f);
```

### 3D Audio
```csharp
// At position
await audioManager.PlayAudioAtPositionAsync("audio_id", position);

// Attached to object
await audioManager.PlayAudioAttachedAsync("audio_id", transform);
```

### Volume Control
```csharp
// Master
audioManager.SetMasterVolume(0.8f);

// By type
audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, 0.6f);

// Individual
handle.SetVolume(0.5f);
```

### Pause/Resume
```csharp
// Pause all
audioManager.PauseAllAudio(0.5f);

// Resume all
audioManager.ResumeAllAudio(0.5f);

// By type
audioManager.PauseAudioType(AudioType.SoundEffect, 0.5f);
audioManager.ResumeAudioType(AudioType.SoundEffect, 0.5f);

// Individual
handle.Pause(0.5f);
handle.Resume(0.5f);
```

### Stop Audio
```csharp
// Stop all
audioManager.StopAllAudio(2f);

// Stop by type
audioManager.StopAudioType(AudioType.BackgroundMusic, 2f);

// Individual
handle.Stop(1f);
```

### Random Variation
```csharp
var params = new AudioPlaybackParameters
{
    useRandomPitch = true,
    randomPitchRange = 0.1f,
    useRandomVolume = true,
    randomVolumeRange = 0.1f
};
await audioManager.PlayAudioAsync("audio_id", params);
```

### Handle Control
```csharp
var handle = await audioManager.PlayAudioAsync("audio_id");

// Check state
if (handle.IsPlaying) { }
if (handle.IsPaused) { }

// Get info
float time = handle.CurrentTime;
float duration = handle.Duration;

// Control
handle.SetVolume(0.5f);
handle.SetPitch(1.2f);
await handle.FadeVolumeAsync(1f, 2f);
await handle.WaitForCompletionAsync();
```

## Audio Entry Configuration

### Background Music
```
Audio ID: bgm_main_theme
Audio Type: BackgroundMusic
Is Looping: true
Max Concurrent Instances: 1
Spatial Blend: 0 (2D)
```

### UI Sound
```
Audio ID: sfx_button_click
Audio Type: UserInterface
Max Concurrent Instances: 5
Spatial Blend: 0 (2D)
```

### Gunshot (High-frequency)
```
Audio ID: sfx_gun_shot
Audio Type: Weapon
Min Time Between Plays: 0.05
Max Concurrent Instances: 3
Spatial Blend: 1 (3D)
```

### Footstep
```
Audio ID: sfx_footstep
Audio Type: Footstep
Min Time Between Plays: 0.3
Max Concurrent Instances: 2
```

## Audio Types

- `BackgroundMusic` - Music tracks
- `Ambient` - Environmental sounds
- `SoundEffect` - Gameplay sounds
- `Voice` - Dialogue and narration
- `UserInterface` - UI sounds
- `Cinematic` - Cutscene audio
- `MusicStinger` - Short musical cues
- `Footstep` - Movement sounds
- `Weapon` - Combat sounds
- `Environment` - World interactions

## Keyboard Shortcuts (Example Script)

```
1 - Play background music
2 - Play sound effect
3 - Play 3D audio
4 - Play high-frequency audio
P - Pause all
R - Resume all
S - Stop all
```

## Troubleshooting Quick Fixes

**No audio?**
```csharp
// Check volume
audioManager.SetMasterVolume(1f);
audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, 1f);
```

**Too many instances?**
```
In Audio Entry:
- Set maxConcurrentInstances = 3
- Set minTimeBetweenPlays = 0.1
```

**Performance issues?**
```
In AudioDatabaseConfig:
- Increase initialPoolSize = 20
- Enable useAddressables = true
```

## Editor Menu Shortcuts

- `GameObject → Foundations → Audio System → Create Audio Manager`
- `GameObject → Foundations → Audio System → Create Audio Player Prefab`
- `Create → Foundations → Audio → Audio Mixer Config`
- `Create → Foundations → Audio → Audio Database Config`

## Validation

### In Inspector
- Click "Validate Database" button on AudioDatabaseConfig
- Click "Validate Mixer Setup" button on AudioMixerConfig

### In Code
```csharp
AudioSystemSetup.ValidateAudioSystem(audioManagerObject);
```

## Performance Tips

1. Set appropriate pool size (10-20 for most games)
2. Use minTimeBetweenPlays for high-frequency audio
3. Limit maxConcurrentInstances per audio
4. Use Addressables for large libraries (> 100MB)
5. Preload critical audio on scene start
6. Unload unused audio between levels

## Common Mistakes to Avoid

❌ Using `Vector3.Distance` for range checks
✅ Use squared distance in frequency controller

❌ Not setting frequency limits on weapons
✅ Set minTimeBetweenPlays and maxConcurrentInstances

❌ Playing audio without checking return value
✅ Store handles for important audio

❌ Forgetting to unload Addressables
✅ Call UnloadAudioEntries when done

❌ Not testing on target hardware
✅ Always profile on actual devices

## Best Practices Checklist

- [ ] Use appropriate audio types
- [ ] Set frequency limits on repetitive sounds
- [ ] Use fade in/out for smooth transitions
- [ ] Configure 3D audio settings properly
- [ ] Preload critical audio
- [ ] Keep handles for important audio
- [ ] Add random variation to repetitive sounds
- [ ] Test volume balance
- [ ] Profile performance
- [ ] Document custom audio IDs

## Emergency Stops

```csharp
// Nuclear option - stop everything immediately
audioManager.StopAllAudio(0f);

// Pause everything for debugging
audioManager.PauseAllAudio(0f);

// Mute everything
audioManager.SetMasterVolume(0f);
```

## Documentation Links

- Full API: `README.md`
- Setup Guide: `GETTING_STARTED.md`
- Architecture: `ARCHITECTURE.md`
- Summary: `SUMMARY.md`
- Examples: `Example/ExampleAudioUsage.cs`

## Support

Check console for error messages with detailed information.
All methods provide helpful warnings when something goes wrong.

