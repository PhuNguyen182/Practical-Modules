using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;
using UnityEditor.Animations;

namespace PracticalSystems.ThemeSystem.Components
{
    /// <summary>
    /// Component that applies character themes to characters, including materials, animations, and effects
    /// </summary>
    public class CharacterThemeComponent : BaseThemeComponent
    {
        [Header("Character References")]
        [SerializeField] private Renderer[] characterRenderers;
        [SerializeField] private Animator[] characterAnimators;
        [SerializeField] private ParticleSystem[] characterParticles;
        [SerializeField] private Light[] characterLights;
        [SerializeField] private AudioSource[] characterAudioSources;
        [SerializeField] private Rigidbody[] characterRigidbodies;
        
        [Header("Character UI")]
        [SerializeField] private Canvas[] characterUICanvas;
        [SerializeField] private UnityEngine.UI.Image[] healthBars;
        [SerializeField] private UnityEngine.UI.Image[] manaBars;
        [SerializeField] private UnityEngine.UI.Image[] expBars;
        
        [Header("Character Settings")]
        [SerializeField] private bool applyMaterials = true;
        [SerializeField] private bool applyAnimations = true;
        [SerializeField] private bool applyParticles = true;
        [SerializeField] private bool applyLighting = true;
        [SerializeField] private bool applyAudio = true;
        [SerializeField] private bool applyPhysics = true;
        [SerializeField] private bool applyUI = true;
        [SerializeField] private bool applyEffects = true;
        
        private CharacterThemeData currentThemeData;
        private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
        private Dictionary<Animator, RuntimeAnimatorController> originalAnimators = new Dictionary<Animator, RuntimeAnimatorController>();
        
        protected override void Start()
        {
            base.Start();
            CacheOriginalValues();
            AutoFindCharacterElements();
        }
        
        /// <summary>
        /// Caches original values for theme application
        /// </summary>
        private void CacheOriginalValues()
        {
            originalMaterials.Clear();
            originalAnimators.Clear();
            
            // Cache original materials
            foreach (var renderer in characterRenderers)
            {
                if (renderer != null)
                    originalMaterials[renderer] = renderer.materials;
            }
            
            // Cache original animators
            foreach (var animator in characterAnimators)
            {
                if (animator != null)
                    originalAnimators[animator] = animator.runtimeAnimatorController;
            }
        }
        
        /// <summary>
        /// Automatically finds character elements
        /// </summary>
        private void AutoFindCharacterElements()
        {
            if (characterRenderers == null || characterRenderers.Length == 0)
                characterRenderers = GetComponentsInChildren<Renderer>();
                
            if (characterAnimators == null || characterAnimators.Length == 0)
                characterAnimators = GetComponentsInChildren<Animator>();
                
            if (characterParticles == null || characterParticles.Length == 0)
                characterParticles = GetComponentsInChildren<ParticleSystem>();
                
            if (characterLights == null || characterLights.Length == 0)
                characterLights = GetComponentsInChildren<Light>();
                
            if (characterAudioSources == null || characterAudioSources.Length == 0)
                characterAudioSources = GetComponentsInChildren<AudioSource>();
                
            if (characterRigidbodies == null || characterRigidbodies.Length == 0)
                characterRigidbodies = GetComponentsInChildren<Rigidbody>();
                
            if (characterUICanvas == null || characterUICanvas.Length == 0)
                characterUICanvas = GetComponentsInChildren<Canvas>();
        }
        
        protected override void OnThemeApplied(ITheme theme)
        {
            if (theme is CharacterTheme charTheme)
            {
                ApplyCharacterTheme(charTheme.ThemeData);
            }
        }
        
        /// <summary>
        /// Applies character theme data to all character elements
        /// </summary>
        /// <param name="themeData">The character theme data to apply</param>
        public void ApplyCharacterTheme(CharacterThemeData themeData)
        {
            currentThemeData = themeData;
            
            if (applyMaterials)
                ApplyMaterialTheme(themeData);
                
            if (applyAnimations)
                ApplyAnimationTheme(themeData);
                
            if (applyParticles)
                ApplyParticleTheme(themeData);
                
            if (applyLighting)
                ApplyLightingTheme(themeData);
                
            if (applyAudio)
                ApplyAudioTheme(themeData);
                
            if (applyPhysics)
                ApplyPhysicsTheme(themeData);
                
            if (applyUI)
                ApplyUITheme(themeData);
                
            if (applyEffects)
                ApplyEffectsTheme(themeData);
        }
        
        private void ApplyMaterialTheme(CharacterThemeData themeData)
        {
            foreach (var renderer in characterRenderers)
            {
                if (renderer == null) continue;
                
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;
                    
                    var material = materials[i];
                    
                    // Apply material properties based on material name or properties
                    if (material.name.ToLower().Contains("skin"))
                    {
                        ApplySkinMaterial(material, themeData);
                    }
                    else if (material.name.ToLower().Contains("hair"))
                    {
                        ApplyHairMaterial(material, themeData);
                    }
                    else if (material.name.ToLower().Contains("eye"))
                    {
                        ApplyEyeMaterial(material, themeData);
                    }
                    else if (material.name.ToLower().Contains("clothing") || material.name.ToLower().Contains("cloth"))
                    {
                        ApplyClothingMaterial(material, themeData);
                    }
                    else if (material.name.ToLower().Contains("accessory"))
                    {
                        ApplyAccessoryMaterial(material, themeData);
                    }
                    else
                    {
                        ApplyDefaultMaterial(material, themeData);
                    }
                    
                    // Apply common material properties
                    material.SetFloat("_Metallic", themeData.metallic);
                    material.SetFloat("_Smoothness", themeData.smoothness);
                    material.SetFloat("_BumpScale", themeData.normalMapIntensity);
                    
                    if (themeData.emissionIntensity > 0f)
                    {
                        material.SetColor("_EmissionColor", themeData.emissionColor * themeData.emissionIntensity);
                        material.EnableKeyword("_EMISSION");
                    }
                    else
                    {
                        material.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
        
        private void ApplySkinMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = themeData.skinColor;
        }
        
        private void ApplyHairMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = themeData.hairColor;
        }
        
        private void ApplyEyeMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = themeData.eyeColor;
        }
        
        private void ApplyClothingMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = themeData.clothingColor;
        }
        
        private void ApplyAccessoryMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = themeData.accessoryColor;
        }
        
        private void ApplyDefaultMaterial(Material material, CharacterThemeData themeData)
        {
            if (material.HasProperty("_Color"))
                material.color = Color.Lerp(material.color, themeData.clothingColor, 0.5f);
        }
        
        private void ApplyAnimationTheme(CharacterThemeData themeData)
        {
            foreach (var animator in characterAnimators)
            {
                if (animator == null) continue;
                
                // Set animation speed
                animator.speed = themeData.animationSpeed;
                
                // Apply specific animations if available
                if (themeData.idleAnimations != null && themeData.idleAnimations.Length > 0)
                {
                    // This would require custom animator controller setup
                    // For now, we'll just adjust the speed
                }
            }
        }
        
        private void ApplyParticleTheme(CharacterThemeData themeData)
        {
            foreach (var particleSystem in characterParticles)
            {
                if (particleSystem == null) continue;
                
                var main = particleSystem.main;
                main.startColor = themeData.particleColor;
                
                if (themeData.enableParticles)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
        }
        
        private void ApplyLightingTheme(CharacterThemeData themeData)
        {
            foreach (var light in characterLights)
            {
                if (light == null) continue;
                
                light.color = themeData.characterLightColor;
                light.intensity = themeData.characterLightIntensity;
                light.range = themeData.characterLightRange;
                light.enabled = themeData.enableCharacterLight;
            }
        }
        
        private void ApplyAudioTheme(CharacterThemeData themeData)
        {
            foreach (var audioSource in characterAudioSources)
            {
                if (audioSource == null) continue;
                
                audioSource.volume = themeData.audioVolume;
                audioSource.pitch = themeData.audioPitch;
                audioSource.outputAudioMixerGroup = themeData.characterAudioMixerGroup;
                audioSource.loop = themeData.loopAudio;
            }
        }
        
        private void ApplyPhysicsTheme(CharacterThemeData themeData)
        {
            foreach (var rigidbody in characterRigidbodies)
            {
                if (rigidbody == null) continue;
                
                rigidbody.mass = themeData.mass;
                rigidbody.linearDamping = themeData.drag;
                rigidbody.angularDamping = themeData.angularDrag;
                rigidbody.useGravity = themeData.useGravity;
                rigidbody.isKinematic = themeData.isKinematic;
            }
        }
        
        private void ApplyUITheme(CharacterThemeData themeData)
        {
            // Apply health bar color
            foreach (var healthBar in healthBars)
            {
                if (healthBar != null)
                    healthBar.color = themeData.healthBarColor;
            }
            
            // Apply mana bar color
            foreach (var manaBar in manaBars)
            {
                if (manaBar != null)
                    manaBar.color = themeData.manaBarColor;
            }
            
            // Apply exp bar color
            foreach (var expBar in expBars)
            {
                if (expBar != null)
                    expBar.color = themeData.expBarColor;
            }
        }
        
        private void ApplyEffectsTheme(CharacterThemeData themeData)
        {
            // Apply glow effect
            if (themeData.enableGlow)
            {
                ApplyGlowEffect(themeData);
            }
            
            // Apply outline effect
            if (themeData.enableOutline)
            {
                ApplyOutlineEffect(themeData);
            }
            
            // Apply shadow effect
            if (themeData.enableShadow)
            {
                ApplyShadowEffect(themeData);
            }
        }
        
        private void ApplyGlowEffect(CharacterThemeData themeData)
        {
            foreach (var renderer in characterRenderers)
            {
                if (renderer == null) continue;
                
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;
                    
                    var material = materials[i];
                    
                    // Enable emission for glow effect
                    material.SetColor("_EmissionColor", themeData.glowColor * themeData.glowIntensity);
                    material.EnableKeyword("_EMISSION");
                }
            }
        }
        
        private void ApplyOutlineEffect(CharacterThemeData themeData)
        {
            // This would require a custom outline shader
            // For now, we'll create a simple outline using duplicate renderers
            foreach (var renderer in characterRenderers)
            {
                if (renderer == null) continue;
                
                // Create outline renderer
                var outlineObject = new GameObject($"{renderer.name}_Outline");
                outlineObject.transform.SetParent(renderer.transform);
                outlineObject.transform.localPosition = Vector3.zero;
                outlineObject.transform.localRotation = Quaternion.identity;
                outlineObject.transform.localScale = Vector3.one * (1f + themeData.outlineWidth);
                
                var outlineRenderer = outlineObject.AddComponent<MeshRenderer>();
                var outlineFilter = outlineObject.AddComponent<MeshFilter>();
                
                outlineFilter.mesh = renderer.GetComponent<MeshFilter>().mesh;
                
                // Create outline material
                var outlineMaterial = new Material(Shader.Find("Standard"));
                outlineMaterial.color = themeData.outlineColor;
                outlineMaterial.SetFloat("_Metallic", 0f);
                outlineMaterial.SetFloat("_Smoothness", 0f);
                
                outlineRenderer.material = outlineMaterial;
                outlineRenderer.enabled = true;
            }
        }
        
        private void ApplyShadowEffect(CharacterThemeData themeData)
        {
            // This would require custom shadow rendering
            // For now, we'll use Unity's built-in shadow casting
            foreach (var renderer in characterRenderers)
            {
                if (renderer == null) continue;
                
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }
        
        /// <summary>
        /// Plays a character sound effect
        /// </summary>
        /// <param name="soundIndex">Index of the sound in the character audio clips array</param>
        public void PlayCharacterSound(int soundIndex)
        {
            if (currentThemeData == null || currentThemeData.characterAudioClips == null || 
                soundIndex < 0 || soundIndex >= currentThemeData.characterAudioClips.Length)
                return;
                
            PlayCharacterSFX(currentThemeData.characterAudioClips[soundIndex]);
        }
        
        /// <summary>
        /// Plays a character sound effect
        /// </summary>
        /// <param name="clip">The audio clip to play</param>
        public void PlayCharacterSFX(AudioClip clip)
        {
            if (characterAudioSources == null || characterAudioSources.Length == 0 || clip == null)
                return;
                
            // Use the first available audio source
            foreach (var audioSource in characterAudioSources)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(clip);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Updates character stats based on theme data
        /// </summary>
        public void UpdateCharacterStats()
        {
            if (currentThemeData == null) return;
            
            // This would integrate with a character stats system
            // For now, we'll just log the values
            Debug.Log($"Character Stats - Health: {currentThemeData.health}/{currentThemeData.maxHealth}, " +
                     $"Speed: {currentThemeData.speed}, Attack: {currentThemeData.attackDamage}, " +
                     $"Defense: {currentThemeData.defense}");
        }
        
        [ContextMenu("Auto-Find Character Elements")]
        private void AutoFindCharacterElementsMenu()
        {
            AutoFindCharacterElements();
        }
    }
}
