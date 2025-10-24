using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using I2.Loc;
using TMPro;

namespace PracticalModules.Localization.Components
{
    public class TextMeshProInputFieldSetLocalizedText : MonoBehaviour
    {
        [ValueDropdown("GetTermsList")]
        [SerializeField] public string termKey;
        [SerializeField] private TMP_InputField inputField;
        [ReadOnly] 
        [SerializeField] private TMP_Text placeHolderText;
        
        private void OnEnable()
        {
            LocalizationManager.OnLocalizeEvent += SetTranslatedText;
            SetTranslatedText();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizeEvent -= SetTranslatedText;
        }
        
        private List<string> GetTermsList() => LocalizationManager.GetTermsList();
        
        private void SetTranslatedText()
        {
            this.placeHolderText.text = LocalizationManager.GetTranslation(termKey);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                inputField ??= GetComponent<TMP_InputField>();
                placeHolderText = inputField.placeholder as TMP_Text;
                this.SetTranslatedText();
            }
        }
#endif
    }
}
