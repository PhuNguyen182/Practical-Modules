using Cysharp.Threading.Tasks;
using PracticalSystems.AudioSystem.Data;
using PracticalSystems.AudioSystem.Interfaces;
using UnityEngine;

namespace PracticalSystems.AudioSystem.Core
{
    /// <summary>
    /// Handle for controlling individual audio playback
    /// Provides fine-grained control with fade and async operations
    /// </summary>
    public class AudioHandle : IAudioHandle
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly IAudioPlayerPool _playerPool;
        private readonly IAudioEntry _audioEntry;
        private float _originalVolume;
        private bool _isDisposed;
        
        public bool IsPlaying => !this._isDisposed && this._audioPlayer is { IsPlaying: true };
        public bool IsPaused => !this._isDisposed && this._audioPlayer is { IsPaused: true };
        
        public float CurrentTime => this._audioPlayer?.AudioSource?.time ?? 0f;
        public float Duration => this._audioEntry?.AudioClip?.length ?? 0f;
        public AudioKind AudioKind => this._audioEntry?.AudioKind ?? AudioKind.SoundEffect;
        
        public AudioHandle(
            IAudioPlayer audioPlayer, 
            IAudioPlayerPool playerPool, 
            IAudioEntry audioEntry)
        {
            this._audioPlayer = audioPlayer;
            this._playerPool = playerPool;
            this._audioEntry = audioEntry;
            this._originalVolume = audioPlayer?.AudioSource?.volume ?? 1f;
            this._isDisposed = false;
        }
        
        public void Stop(float fadeOutDuration = 0f)
        {
            if (this._isDisposed || this._audioPlayer == null)
            {
                return;
            }
            
            if (fadeOutDuration > 0f)
            {
                this.FadeOutAndStopAsync(fadeOutDuration).Forget();
            }
            else
            {
                this.StopImmediate();
            }
        }
        
        public void Pause(float fadeOutDuration = 0f)
        {
            if (this._isDisposed || this._audioPlayer == null)
            {
                return;
            }
            
            if (fadeOutDuration > 0f)
            {
                this.FadeVolumeAsync(0f, fadeOutDuration).ContinueWith(() =>
                {
                    this._audioPlayer?.Pause();
                }).Forget();
            }
            else
            {
                this._audioPlayer.Pause();
            }
        }
        
        public void Resume(float fadeInDuration = 0f)
        {
            if (this._isDisposed || this._audioPlayer == null)
            {
                return;
            }
            
            this._audioPlayer.Resume();
            
            if (fadeInDuration > 0f)
            {
                this.FadeVolumeAsync(this._originalVolume, fadeInDuration).Forget();
            }
        }
        
        public void SetVolume(float volume)
        {
            if (this._isDisposed || !this._audioPlayer?.AudioSource)
            {
                return;
            }
            
            this._originalVolume = Mathf.Clamp01(volume);
            this._audioPlayer.AudioSource.volume = this._originalVolume;
        }
        
        public void SetPitch(float pitch)
        {
            if (this._isDisposed || !this._audioPlayer?.AudioSource)
            {
                return;
            }
            
            this._audioPlayer.AudioSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        }
        
        public async UniTask FadeVolumeAsync(float targetVolume, float duration)
        {
            if (this._isDisposed || this._audioPlayer?.AudioSource == null)
            {
                return;
            }
            
            var startVolume = this._audioPlayer.AudioSource.volume;
            var elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                if (this._isDisposed || !this._audioPlayer?.AudioSource)
                {
                    return;
                }
                
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / duration);
                this._audioPlayer.AudioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                
                await UniTask.Yield();
            }
            
            if (!this._isDisposed && this._audioPlayer?.AudioSource)
            {
                this._audioPlayer.AudioSource.volume = targetVolume;
            }
        }
        
        public async UniTask WaitForCompletionAsync()
        {
            if (this._isDisposed || !this._audioPlayer?.AudioSource)
            {
                return;
            }
            
            // If looping, this will never complete
            if (this._audioEntry?.IsLooping == true)
            {
                Debug.LogWarning("AudioHandle: Cannot wait for completion of looping audio");
                return;
            }
            
            while (this.IsPlaying)
            {
                await UniTask.NextFrame();
                if (this._isDisposed)
                    return;
            }
            
            this.StopImmediate();
        }
        
        /// <summary>
        /// Fades out and stops the audio
        /// </summary>
        private async UniTaskVoid FadeOutAndStopAsync(float duration)
        {
            await this.FadeVolumeAsync(0f, duration);
            this.StopImmediate();
        }
        
        /// <summary>
        /// Stops the audio immediately and returns to pool
        /// </summary>
        private void StopImmediate()
        {
            if (this._isDisposed)
                return;
            
            this._isDisposed = true;
            if (this._audioPlayer == null) 
                return;
            
            this._audioPlayer.Stop();
            this._playerPool?.ReturnAudioPlayer(this._audioPlayer);
        }
    }
}

