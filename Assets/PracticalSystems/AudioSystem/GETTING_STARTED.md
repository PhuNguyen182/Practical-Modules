# Getting Started with the Audio System

This guide will walk you through setting up and using the professional audio system in your Unity project.

## Prerequisites

Before starting, ensure you have:
- Unity 2021.3 or higher
- UniTask package installed (`com.cysharp.unitask`)
- Basic understanding of Unity and C#

## Step 1: Install UniTask

1. Open Unity Package Manager (Window → Package Manager)
2. Click "+" → "Add package from git URL"
3. Enter: `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`
4. Click "Add"

## Step 2: Create Audio Mixer

1. **Create AudioMixer Asset**:
   - Right-click in Project window
   - Create → Audio → Audio Mixer
   - Name it `MainAudioMixer`

2. **Create Mixer Groups**:
   - Open the AudioMixer window
   - Create groups for each audio type:
     - Master (default)
     - BGM (Background Music)
     - SFX (Sound Effects)
     - Voice
     - UI
     - Ambient

3. **Expose Volume Parameters**:
   - For each group, click the volume parameter
   - Right-click → "Expose 'Volume' to script"
   - In the Exposed Parameters panel, rename them:
     - `MasterVolume`
     - `BGMVolume`
     - `SFXVolume`
     - `VoiceVolume`
     - `UIVolume`
     - `AmbientVolume`

## Step 3: Create Configuration Assets

### 3.1 Create AudioMixerConfig

1. Right-click in Project window
2. Create → Foundations → Audio → Audio Mixer Config
3. Name it `MainAudioMixerConfig`
4. Select the asset and configure:

```
Audio Mixer: MainAudioMixer (assign your mixer)
Master Volume Parameter: MasterVolume
Default Master Volume: 1

Audio Type Mappings:
  - Audio Type: BackgroundMusic
    Mixer Group: BGM
    Volume Parameter Name: BGMVolume
    Default Volume: 0.7
    
  - Audio Type: SoundEffect
    Mixer Group: SFX
    Volume Parameter Name: SFXVolume
    Default Volume: 0.8
    
  - Audio Type: Voice
    Mixer Group: Voice
    Volume Parameter Name: VoiceVolume
    Default Volume: 1
    
  - Audio Type: UserInterface
    Mixer Group: UI
    Volume Parameter Name: UIVolume
    Default Volume: 0.9
    
  - Audio Type: Ambient
    Mixer Group: Ambient
    Volume Parameter Name: AmbientVolume
    Default Volume: 0.6
```

### 3.2 Create AudioDatabaseConfig

1. Right-click in Project window
2. Create → Foundations → Audio → Audio Database Config
3. Name it `MainAudioDatabase`
4. Configure settings:

```
Use Addressables: false (for now)
Preload On Start: true
Initial Pool Size: 10
```

5. Add your audio entries (see Step 4)

## Step 4: Add Audio Entries

For each audio clip in your project:

1. Select `MainAudioDatabase`
2. Expand "Audio Entries"
3. Click "+" to add a new entry
4. Configure the entry:

```
Audio ID: "bgm_main_theme" (unique identifier)
Audio Type: BackgroundMusic
Audio Clip: [Assign your audio clip]
Default Volume: 1
Default Pitch: 1
Is Looping: true (for music)
Priority: 128
Min Time Between Plays: 0
Max Concurrent Instances: 1 (for music)
Spatial Blend: 0 (2D audio)
Min Distance: 1
Max Distance: 500
```

### Example Audio Entries

**Background Music:**
```
Audio ID: bgm_main_theme
Audio Type: BackgroundMusic
Default Volume: 1
Is Looping: true
Max Concurrent Instances: 1
```

**Sound Effect:**
```
Audio ID: sfx_button_click
Audio Type: UserInterface
Default Volume: 0.8
Is Looping: false
Max Concurrent Instances: 5
```

**Gunshot (High-frequency audio):**
```
Audio ID: sfx_gun_shot
Audio Type: Weapon
Default Volume: 0.9
Is Looping: false
Min Time Between Plays: 0.05 (50ms cooldown)
Max Concurrent Instances: 3
Spatial Blend: 1 (3D audio)
```

**Footstep:**
```
Audio ID: sfx_footstep
Audio Type: Footstep
Default Volume: 0.7
Is Looping: false
Min Time Between Plays: 0.3 (300ms cooldown)
Max Concurrent Instances: 2
```

## Step 5: Create Audio Manager in Scene

### Method 1: Using Menu (Recommended)

1. In Unity Editor, go to: `GameObject → Foundations → Audio System → Create Audio Manager`
2. This creates an AudioManager GameObject with the component attached

### Method 2: Manual Setup

1. Create empty GameObject: `GameObject → Create Empty`
2. Rename it to "AudioManager"
3. Add component: `AudioManager`

### Configure Audio Manager

Select the AudioManager GameObject and assign:

```
Audio Database Config: MainAudioDatabase
Audio Mixer Config: MainAudioMixerConfig
Audio Player Prefab: AudioPlayerPrefab (see Step 6)
```

## Step 6: Create Audio Player Prefab

### Method 1: Using Menu (Recommended)

1. Go to: `GameObject → Foundations → Audio System → Create Audio Player Prefab`
2. Save it as a prefab in your Prefabs folder
3. Assign to Audio Manager

### Method 2: Manual Setup

1. Create empty GameObject in scene
2. Add `AudioSource` component
3. Add `AudioPlayer` component
4. Configure AudioSource:
   - Play On Awake: false
   - Spatial Blend: 0
5. Save as prefab
6. Delete from scene
7. Assign prefab to Audio Manager

## Step 7: Use the Audio System

### Basic Script Setup

Create a new script:

```csharp
using UnityEngine;
using Cysharp.Threading.Tasks;
using Foundations.Audio.Interfaces;
using Foundations.Audio.Data;

public class GameAudioController : MonoBehaviour
{
    [SerializeField] private GameObject audioManagerObject;
    private IAudioManager audioManager;
    
    private void Start()
    {
        // Get audio manager reference
        this.audioManager = this.audioManagerObject.GetComponent<IAudioManager>();
        
        // Play background music
        this.PlayBackgroundMusic().Forget();
    }
    
    private async UniTaskVoid PlayBackgroundMusic()
    {
        var parameters = AudioPlaybackParameters.WithFadeIn(2f);
        parameters.overrideLoop = true;
        parameters.loop = true;
        
        await this.audioManager.PlayAudioAsync("bgm_main_theme", parameters);
    }
    
    public void OnButtonClick()
    {
        // Play UI sound
        this.audioManager.PlayAudioAsync("sfx_button_click").Forget();
    }
}
```

### Attach Script to GameObject

1. Create empty GameObject: "GameAudioController"
2. Add your script
3. Assign AudioManager reference in inspector

## Step 8: Test Your Setup

1. **Play Mode Test**:
   - Enter Play mode
   - Background music should start playing with fade in
   - Console should show: "AudioManager: Initialized successfully"

2. **Test Sound Effects**:
   ```csharp
   // In your button's OnClick event:
   audioManager.PlayAudioAsync("sfx_button_click").Forget();
   ```

3. **Test Volume Control**:
   ```csharp
   // Set master volume
   audioManager.SetMasterVolume(0.5f);
   
   // Set BGM volume
   audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, 0.3f);
   ```

## Common Scenarios

### Scenario 1: Play Background Music

```csharp
private async UniTaskVoid PlayBackgroundMusic()
{
    // With fade in and looping
    var parameters = AudioPlaybackParameters.WithFadeIn(2f);
    parameters.overrideLoop = true;
    parameters.loop = true;
    
    var handle = await this.audioManager.PlayAudioAsync(
        "bgm_main_theme",
        parameters
    );
    
    // Keep reference for later control
    this.currentBGM = handle;
}
```

### Scenario 2: Play UI Button Click

```csharp
public void OnButtonClick()
{
    // Simple one-shot sound
    this.audioManager.PlayAudioAsync("sfx_button_click").Forget();
}
```

### Scenario 3: Play 3D Explosion

```csharp
private async UniTaskVoid PlayExplosion(Vector3 position)
{
    var parameters = AudioPlaybackParameters.Default;
    
    await this.audioManager.PlayAudioAtPositionAsync(
        "sfx_explosion",
        position,
        parameters
    );
}
```

### Scenario 4: Play Gunfire (High-frequency)

```csharp
private async UniTaskVoid FireWeapon()
{
    // System automatically limits frequency
    var parameters = AudioPlaybackParameters.WithRandomPitch(0.05f);
    
    await this.audioManager.PlayAudioAsync(
        "sfx_gun_shot",
        parameters
    );
}
```

### Scenario 5: Pause/Resume Game Audio

```csharp
public void OnGamePause()
{
    // Pause all sound effects
    this.audioManager.PauseAudioType(AudioType.SoundEffect, 0.5f);
    this.audioManager.PauseAudioType(AudioType.Ambient, 0.5f);
    
    // Keep UI sounds active
}

public void OnGameResume()
{
    // Resume all paused audio
    this.audioManager.ResumeAllAudio(0.5f);
}
```

### Scenario 6: Settings Menu Volume Control

```csharp
public class AudioSettings : MonoBehaviour
{
    [SerializeField] private GameObject audioManagerObject;
    private IAudioManager audioManager;
    
    public void OnMasterVolumeChanged(float value)
    {
        this.audioManager.SetMasterVolume(value);
    }
    
    public void OnBGMVolumeChanged(float value)
    {
        this.audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, value);
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        this.audioManager.SetAudioTypeVolume(AudioType.SoundEffect, value);
    }
}
```

## Troubleshooting

### Problem: No audio plays

**Solutions**:
1. Check AudioManager is in scene and active
2. Verify AudioDatabaseConfig is assigned
3. Verify AudioMixerConfig is assigned
4. Check audio entries have clips assigned
5. Check volume levels (master and type)
6. Check Console for error messages

### Problem: Audio plays but very quiet

**Solutions**:
1. Check master volume: `audioManager.SetMasterVolume(1f)`
2. Check audio type volume
3. Check AudioMixer group volumes
4. Check individual audio entry default volume
5. Check in AudioMixer window (not just exposed parameters)

### Problem: Audio doesn't stop playing

**Solutions**:
1. Make sure you're calling `Stop()` on handles
2. Use `StopAudioType()` or `StopAllAudio()`
3. Check for memory leaks (handles not being released)

### Problem: Too many audio instances

**Solutions**:
1. Set `maxConcurrentInstances` in audio entry
2. Set `minTimeBetweenPlays` for cooldown
3. Manually stop old instances

### Problem: Performance issues

**Solutions**:
1. Increase `initialPoolSize` in AudioDatabaseConfig
2. Reduce max concurrent instances
3. Use Addressables for large libraries
4. Check for excessive PlayAudio calls

## Next Steps

Now that you have the basics working:

1. **Explore Examples**: Check `ExampleAudioUsage.cs` for more scenarios
2. **Read Documentation**: See `README.md` for comprehensive API reference
3. **Review Architecture**: See `ARCHITECTURE.md` for system design
4. **Advanced Features**: Try Addressables for large audio libraries
5. **Optimize**: Profile your audio system and adjust settings

## Quick Reference

### Most Common Operations

```csharp
// Play simple audio
await audioManager.PlayAudioAsync("audio_id");

// Play with fade in
var params = AudioPlaybackParameters.WithFadeIn(1f);
await audioManager.PlayAudioAsync("audio_id", params);

// Play 3D audio
await audioManager.PlayAudioAtPositionAsync("audio_id", position);

// Stop all of a type
audioManager.StopAudioType(AudioType.BackgroundMusic, 2f);

// Pause all audio
audioManager.PauseAllAudio(0.5f);

// Resume all audio
audioManager.ResumeAllAudio(0.5f);

// Set volumes
audioManager.SetMasterVolume(0.8f);
audioManager.SetAudioTypeVolume(AudioType.BackgroundMusic, 0.6f);
```

## Support

If you encounter issues:
1. Check Console for error messages
2. Use validation buttons in inspector
3. Review example scripts
4. Check documentation

Happy game developing! 🎮🔊

