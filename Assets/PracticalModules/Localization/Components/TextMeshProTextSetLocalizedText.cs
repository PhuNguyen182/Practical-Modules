using I2.Loc;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;

namespace PracticalModules.Localization.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextMeshProTextSetLocalizedText : MonoBehaviour
    {
        [ValueDropdown("GetTermsList")]
        [SerializeField] public string termKey;
        [SerializeField] private TMP_Text targetText;

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
        
        private void SetTranslatedText() => this.targetText.text = LocalizationManager.GetTranslation(termKey);
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            targetText ??= GetComponent<TMP_Text>();
            this.SetTranslatedText();
        }
        #endif
    }
}
