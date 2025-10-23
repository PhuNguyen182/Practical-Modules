using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using I2.Loc;
using TMPro;

namespace PracticalModules.Localization.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextMeshSetLocalizeLanguage : MonoBehaviour
    {
        [ValueDropdown("GetTermsList")]
        [SerializeField] public string termKey;
        [SerializeField] private TMP_Text targetText;
        
        private List<string> GetTermsList() => LocalizationManager.GetTermsList();
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            targetText ??= GetComponent<TMP_Text>();
            targetText.text = LocalizationManager.GetTranslation(termKey);
        }
        #endif
    }
}
