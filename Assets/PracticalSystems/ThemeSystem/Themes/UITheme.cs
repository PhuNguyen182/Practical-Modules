using PracticalSystems.ThemeSystem.Components;
using PracticalSystems.ThemeSystem.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PracticalSystems.ThemeSystem.Themes
{
    /// <summary>
    /// UI theme data structure
    /// </summary>
    [CreateAssetMenu(fileName = "UI Theme", menuName = "Theme System/UI Theme")]
    public class UITheme : BaseTheme
    {
        [Header("UI Theme Settings")]
        [SerializeField] private UIThemeData themeData = new UIThemeData();
        
        public UIThemeData ThemeData => themeData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            category = "UI";
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            category = "UI";
        }
        
        public override bool ApplyTo(IThemeComponent component)
        {
            if (component is UIThemeComponent uiComponent)
            {
                uiComponent.ApplyUITheme(themeData);
                return base.ApplyTo(component);
            }
            return false;
        }
    }
    
    /// <summary>
    /// UI theme data container
    /// </summary>
    [System.Serializable]
    public class UIThemeData
    {
        [Header("Colors")]
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
        public Color accentColor = Color.blue;
        public Color backgroundColor = Color.black;
        public Color textColor = Color.white;
        public Color buttonColor = Color.gray;
        public Color buttonHoverColor = Color.white;
        public Color buttonPressedColor = Color.green;
        public Color errorColor = Color.red;
        public Color warningColor = Color.yellow;
        public Color successColor = Color.green;
        
        [Header("TextMeshPro Settings")]
        public TMP_FontAsset primaryFont;
        public TMP_FontAsset secondaryFont;
        public TMP_FontAsset accentFont;
        public float fontSize = 14f;
        public float headingFontSize = 18f;
        public float subheadingFontSize = 16f;
        public float smallFontSize = 12f;
        public FontStyles defaultFontStyle = FontStyles.Normal;
        public FontStyles headingFontStyle = FontStyles.Bold;
        
        [Header("UI Element Settings")]
        public float buttonCornerRadius = 5f;
        public float panelCornerRadius = 10f;
        public float borderWidth = 1f;
        public Color borderColor = Color.white;
        
        [Header("Animation Settings")]
        public float transitionDuration = 0.3f;
        public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool enableHoverEffects = true;
        public bool enableClickEffects = true;
        
        [Header("Spacing")]
        public float smallSpacing = 5f;
        public float mediumSpacing = 10f;
        public float largeSpacing = 20f;
        
        [Header("Shadows")]
        public bool enableShadows = true;
        public Vector2 shadowOffset = new Vector2(2, -2);
        public Color shadowColor = new Color(0, 0, 0, 0.5f);
        public float shadowBlur = 2f;
        
        [Header("Gradients")]
        public bool enableGradients = false;
        public Gradient primaryGradient = new Gradient();
        public Gradient buttonGradient = new Gradient();
    }
}
