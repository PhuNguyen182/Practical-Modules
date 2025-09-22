using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using PracticalSystems.ThemeSystem.Core;
using PracticalSystems.ThemeSystem.Themes;

namespace PracticalSystems.ThemeSystem.Components
{
    /// <summary>
    /// Component that applies UI themes to UI elements with TextMeshPro support
    /// </summary>
    public class UIThemeComponent : BaseThemeComponent
    {
        [Header("UI Elements")]
        [SerializeField] private Image[] images;
        [SerializeField] private Button[] buttons;
        [SerializeField] private TMP_Text[] tmpTexts;
        [SerializeField] private TMP_InputField[] tmpInputFields;
        [SerializeField] private TMP_Dropdown[] tmpDropdowns;
        [SerializeField] private ScrollRect[] scrollRects;
        [SerializeField] private Slider[] sliders;
        [SerializeField] private Toggle[] toggles;
        
        [Header("Custom UI Elements")]
        [SerializeField] private UIElementData[] customElements;
        
        private UIThemeData currentThemeData;
        private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
        private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>();
        
        protected override void Start()
        {
            base.Start();
            CacheOriginalValues();
        }
        
        /// <summary>
        /// Caches original values for animation purposes
        /// </summary>
        private void CacheOriginalValues()
        {
            originalScales.Clear();
            originalImageColors.Clear();
            
            // Cache image colors
            foreach (var img in images)
            {
                if (img != null)
                    originalImageColors[img] = img.color;
            }
            
            // Cache button scales
            foreach (var button in buttons)
            {
                if (button != null)
                    originalScales[button.gameObject] = button.transform.localScale;
            }
        }
        
        protected override void OnThemeApplied(ITheme theme)
        {
            if (theme is UITheme uiTheme)
            {
                ApplyUITheme(uiTheme.ThemeData);
            }
        }
        
        /// <summary>
        /// Applies UI theme data to all UI elements
        /// </summary>
        /// <param name="themeData">The UI theme data to apply</param>
        public void ApplyUITheme(UIThemeData themeData)
        {
            currentThemeData = themeData;
            
            // Apply to images
            ApplyThemeToImages(themeData);
            
            // Apply to buttons
            ApplyThemeToButtons(themeData);
            
            // Apply to TextMeshPro texts
            ApplyThemeToTMPTexts(themeData);
            
            // Apply to input fields
            ApplyThemeToTMPInputFields(themeData);
            
            // Apply to dropdowns
            ApplyThemeToTMPDropdowns(themeData);
            
            // Apply to scroll rects
            ApplyThemeToScrollRects(themeData);
            
            // Apply to sliders
            ApplyThemeToSliders(themeData);
            
            // Apply to toggles
            ApplyThemeToToggles(themeData);
            
            // Apply to custom elements
            ApplyThemeToCustomElements(themeData);
            
            // Apply shadows if enabled
            if (themeData.enableShadows)
            {
                ApplyShadows(themeData);
            }
        }
        
        private void ApplyThemeToImages(UIThemeData themeData)
        {
            foreach (var img in images)
            {
                if (img == null) continue;
                
                // Apply color based on image name or tag
                if (img.name.ToLower().Contains("background"))
                    img.color = themeData.backgroundColor;
                else if (img.name.ToLower().Contains("button"))
                    img.color = themeData.buttonColor;
                else
                    img.color = themeData.primaryColor;
            }
        }
        
        private void ApplyThemeToButtons(UIThemeData themeData)
        {
            foreach (var button in buttons)
            {
                if (button == null) continue;
                
                var buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = themeData.buttonColor;
                }
                
                // Apply TextMeshPro text color if present
                var tmpText = button.GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.color = themeData.textColor;
                    tmpText.font = themeData.primaryFont;
                    tmpText.fontSize = themeData.fontSize;
                }
                
                // Add hover and click effects
                if (themeData.enableHoverEffects)
                {
                    AddButtonEffects(button, themeData);
                }
            }
        }
        
        private void ApplyThemeToTMPTexts(UIThemeData themeData)
        {
            foreach (var tmpText in tmpTexts)
            {
                if (tmpText == null) continue;
                
                tmpText.color = themeData.textColor;
                tmpText.font = themeData.primaryFont;
                
                // Apply font size based on text type
                if (tmpText.name.ToLower().Contains("heading") || tmpText.name.ToLower().Contains("title"))
                {
                    tmpText.fontSize = themeData.headingFontSize;
                    tmpText.fontStyle = themeData.headingFontStyle;
                }
                else if (tmpText.name.ToLower().Contains("subheading") || tmpText.name.ToLower().Contains("subtitle"))
                {
                    tmpText.fontSize = themeData.subheadingFontSize;
                }
                else if (tmpText.name.ToLower().Contains("small") || tmpText.name.ToLower().Contains("caption"))
                {
                    tmpText.fontSize = themeData.smallFontSize;
                }
                else
                {
                    tmpText.fontSize = themeData.fontSize;
                    tmpText.fontStyle = themeData.defaultFontStyle;
                }
                
                // Apply shadow if enabled
                if (themeData.enableShadows)
                {
                    tmpText.fontSharedMaterial = themeData.primaryFont.material;
                }
            }
        }
        
        private void ApplyThemeToTMPInputFields(UIThemeData themeData)
        {
            foreach (var inputField in tmpInputFields)
            {
                if (inputField == null) continue;
                
                inputField.textComponent.color = themeData.textColor;
                inputField.textComponent.font = themeData.primaryFont;
                inputField.textComponent.fontSize = themeData.fontSize;
                
                // Apply placeholder color
                if (inputField.placeholder != null)
                {
                    var placeholderText = inputField.placeholder.GetComponent<TMP_Text>();
                    if (placeholderText != null)
                    {
                        placeholderText.color = themeData.textColor * 0.5f;
                    }
                }
            }
        }
        
        private void ApplyThemeToTMPDropdowns(UIThemeData themeData)
        {
            foreach (var dropdown in tmpDropdowns)
            {
                if (dropdown == null) continue;
                
                dropdown.captionText.color = themeData.textColor;
                dropdown.captionText.font = themeData.primaryFont;
                dropdown.captionText.fontSize = themeData.fontSize;
                
                dropdown.itemText.color = themeData.textColor;
                dropdown.itemText.font = themeData.primaryFont;
                dropdown.itemText.fontSize = themeData.fontSize;
            }
        }
        
        private void ApplyThemeToScrollRects(UIThemeData themeData)
        {
            foreach (var scrollRect in scrollRects)
            {
                if (scrollRect == null) continue;
                
                var scrollbar = scrollRect.verticalScrollbar;
                if (scrollbar != null)
                {
                    var scrollbarImage = scrollbar.GetComponent<Image>();
                    if (scrollbarImage != null)
                    {
                        scrollbarImage.color = themeData.secondaryColor;
                    }
                }
                
                var horizontalScrollbar = scrollRect.horizontalScrollbar;
                if (horizontalScrollbar != null)
                {
                    var scrollbarImage = horizontalScrollbar.GetComponent<Image>();
                    if (scrollbarImage != null)
                    {
                        scrollbarImage.color = themeData.secondaryColor;
                    }
                }
            }
        }
        
        private void ApplyThemeToSliders(UIThemeData themeData)
        {
            foreach (var slider in sliders)
            {
                if (slider == null) continue;
                
                slider.fillRect.GetComponent<Image>().color = themeData.accentColor;
                slider.handleRect.GetComponent<Image>().color = themeData.buttonColor;
            }
        }
        
        private void ApplyThemeToToggles(UIThemeData themeData)
        {
            foreach (var toggle in toggles)
            {
                if (toggle == null) continue;
                
                var toggleImage = toggle.GetComponent<Image>();
                if (toggleImage != null)
                {
                    toggleImage.color = themeData.buttonColor;
                }
                
                var tmpText = toggle.GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.color = themeData.textColor;
                    tmpText.font = themeData.primaryFont;
                    tmpText.fontSize = themeData.fontSize;
                }
            }
        }
        
        private void ApplyThemeToCustomElements(UIThemeData themeData)
        {
            foreach (var element in customElements)
            {
                if (element.target == null) continue;
                
                ApplyCustomElementTheme(element, themeData);
            }
        }
        
        private void ApplyCustomElementTheme(UIElementData element, UIThemeData themeData)
        {
            switch (element.elementType)
            {
                case UIElementType.Image:
                    var img = element.target.GetComponent<Image>();
                    if (img != null)
                        img.color = GetColorFromProperty(themeData, element.colorProperty);
                    break;
                    
                case UIElementType.Text:
                    var tmpText = element.target.GetComponent<TMP_Text>();
                    if (tmpText != null)
                    {
                        tmpText.color = GetColorFromProperty(themeData, element.colorProperty);
                        tmpText.font = GetFontFromProperty(themeData, element.fontProperty);
                        tmpText.fontSize = element.fontSize > 0 ? element.fontSize : themeData.fontSize;
                    }
                    break;
                    
                case UIElementType.Button:
                    var button = element.target.GetComponent<Button>();
                    if (button != null)
                    {
                        var buttonImg = button.GetComponent<Image>();
                        if (buttonImg != null)
                            buttonImg.color = GetColorFromProperty(themeData, element.colorProperty);
                    }
                    break;
            }
        }
        
        private Color GetColorFromProperty(UIThemeData themeData, string propertyName)
        {
            switch (propertyName.ToLower())
            {
                case "primary": return themeData.primaryColor;
                case "secondary": return themeData.secondaryColor;
                case "accent": return themeData.accentColor;
                case "background": return themeData.backgroundColor;
                case "text": return themeData.textColor;
                case "button": return themeData.buttonColor;
                case "error": return themeData.errorColor;
                case "warning": return themeData.warningColor;
                case "success": return themeData.successColor;
                default: return themeData.primaryColor;
            }
        }
        
        private TMP_FontAsset GetFontFromProperty(UIThemeData themeData, string propertyName)
        {
            switch (propertyName.ToLower())
            {
                case "primary": return themeData.primaryFont;
                case "secondary": return themeData.secondaryFont;
                case "accent": return themeData.accentFont;
                default: return themeData.primaryFont;
            }
        }
        
        private void AddButtonEffects(Button button, UIThemeData themeData)
        {
            var buttonImage = button.GetComponent<Image>();
            if (buttonImage == null) return;
            
            // Add hover effect
            var hoverEvent = new Button.ButtonClickedEvent();
            hoverEvent.AddListener(() => StartCoroutine(AnimateButtonHover(button, themeData)));
            
            // Add click effect
            var clickEvent = new Button.ButtonClickedEvent();
            clickEvent.AddListener(() => StartCoroutine(AnimateButtonClick(button, themeData)));
        }
        
        private IEnumerator AnimateButtonHover(Button button, UIThemeData themeData)
        {
            var originalColor = button.GetComponent<Image>().color;
            var targetColor = themeData.buttonHoverColor;
            
            float elapsed = 0f;
            while (elapsed < themeData.transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = themeData.transitionCurve.Evaluate(elapsed / themeData.transitionDuration);
                button.GetComponent<Image>().color = Color.Lerp(originalColor, targetColor, t);
                yield return null;
            }
        }
        
        private IEnumerator AnimateButtonClick(Button button, UIThemeData themeData)
        {
            if (!themeData.enableClickEffects) yield break;
            
            var originalScale = button.transform.localScale;
            var targetScale = originalScale * 0.95f;
            
            // Scale down
            float elapsed = 0f;
            while (elapsed < themeData.transitionDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = themeData.transitionCurve.Evaluate(elapsed / (themeData.transitionDuration * 0.5f));
                button.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            // Scale back up
            elapsed = 0f;
            while (elapsed < themeData.transitionDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = themeData.transitionCurve.Evaluate(elapsed / (themeData.transitionDuration * 0.5f));
                button.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
        }
        
        private void ApplyShadows(UIThemeData themeData)
        {
            foreach (var tmpText in tmpTexts)
            {
                if (tmpText == null) continue;
                
                // Add shadow effect to TextMeshPro
                var material = tmpText.fontSharedMaterial;
                if (material != null)
                {
                    // This would require custom shader setup for shadows
                    // For now, we'll use the built-in shadow options
                }
            }
        }
        
        [ContextMenu("Auto-Find UI Elements")]
        private void AutoFindUIElements()
        {
            images = GetComponentsInChildren<Image>();
            buttons = GetComponentsInChildren<Button>();
            tmpTexts = GetComponentsInChildren<TMP_Text>();
            tmpInputFields = GetComponentsInChildren<TMP_InputField>();
            tmpDropdowns = GetComponentsInChildren<TMP_Dropdown>();
            scrollRects = GetComponentsInChildren<ScrollRect>();
            sliders = GetComponentsInChildren<Slider>();
            toggles = GetComponentsInChildren<Toggle>();
        }
    }
    
    /// <summary>
    /// Data for custom UI elements
    /// </summary>
    [System.Serializable]
    public class UIElementData
    {
        public GameObject target;
        public UIElementType elementType;
        public string colorProperty = "primary";
        public string fontProperty = "primary";
        public float fontSize = 0f;
    }
    
    /// <summary>
    /// Types of UI elements
    /// </summary>
    public enum UIElementType
    {
        Image,
        Text,
        Button,
        InputField,
        Dropdown,
        Slider,
        Toggle,
        ScrollRect
    }
}
