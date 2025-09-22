using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace PracticalSystems.ThemeSystem.Themes
{
    /// <summary>
    /// Character theme data structure
    /// </summary>
    [CreateAssetMenu(fileName = "Character Theme", menuName = "Theme System/Character Theme")]
    public class CharacterTheme : BaseTheme
    {
        [Header("Character Theme Settings")]
        [SerializeField] private CharacterThemeData themeData = new CharacterThemeData();
        
        public CharacterThemeData ThemeData => themeData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            category = "Character";
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            category = "Character";
        }
        
        public override bool ApplyTo(IThemeComponent component)
        {
            if (component is CharacterThemeComponent charComponent)
            {
                charComponent.ApplyCharacterTheme(themeData);
                return base.ApplyTo(component);
            }
            return false;
        }
    }
    
    /// <summary>
    /// Character theme data container
    /// </summary>
    [System.Serializable]
    public class CharacterThemeData
    {
        [Header("Character Materials")]
        public Material[] characterMaterials;
        public Color skinColor = new Color(0.8f, 0.6f, 0.4f, 1f);
        public Color hairColor = new Color(0.2f, 0.1f, 0.05f, 1f);
        public Color eyeColor = new Color(0.2f, 0.4f, 0.8f, 1f);
        public Color clothingColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color accessoryColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        [Header("Material Properties")]
        public float metallic = 0f;
        public float smoothness = 0.5f;
        public float normalMapIntensity = 1f;
        public Color emissionColor = Color.black;
        public float emissionIntensity = 0f;
        
        [Header("Character Animation")]
        public AnimationClip[] idleAnimations;
        public AnimationClip[] walkAnimations;
        public AnimationClip[] runAnimations;
        public AnimationClip[] jumpAnimations;
        public AnimationClip[] attackAnimations;
        public AnimationClip[] deathAnimations;
        public float animationSpeed = 1f;
        public bool loopAnimations = true;
        
        [Header("Particle Effects")]
        public GameObject[] characterParticleEffects;
        public Color particleColor = Color.white;
        public float particleIntensity = 1f;
        public bool enableParticles = true;
        
        [Header("Character Lighting")]
        public Color characterLightColor = Color.white;
        public float characterLightIntensity = 1f;
        public float characterLightRange = 5f;
        public bool enableCharacterLight = false;
        
        [Header("Character Physics")]
        public float mass = 1f;
        public float drag = 0f;
        public float angularDrag = 0.05f;
        public bool useGravity = true;
        public bool isKinematic = false;
        
        [Header("Character Stats")]
        public float health = 100f;
        public float maxHealth = 100f;
        public float speed = 5f;
        public float jumpForce = 10f;
        public float attackDamage = 10f;
        public float defense = 5f;
        
        [Header("Character Audio")]
        public AudioClip[] characterAudioClips;
        public float audioVolume = 0.8f;
        public bool loopAudio = false;
        public float audioPitch = 1f;
        public AudioMixerGroup characterAudioMixerGroup;
        
        [Header("Character UI")]
        public Color healthBarColor = Color.red;
        public Color manaBarColor = Color.blue;
        public Color expBarColor = Color.green;
        public bool showHealthBar = true;
        public bool showManaBar = true;
        public bool showExpBar = true;
        
        [Header("Character Effects")]
        public bool enableGlow = false;
        public Color glowColor = Color.white;
        public float glowIntensity = 1f;
        public bool enableOutline = false;
        public Color outlineColor = Color.black;
        public float outlineWidth = 0.1f;
        public bool enableShadow = true;
        public Vector2 shadowOffset = new Vector2(2, -2);
        public Color shadowColor = new Color(0, 0, 0, 0.5f);
    }
}
