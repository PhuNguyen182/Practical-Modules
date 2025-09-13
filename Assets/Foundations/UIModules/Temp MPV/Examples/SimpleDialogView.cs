using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem.MVP.Examples
{
    /// <summary>
    /// View for simple dialog popup
    /// </summary>
    public class SimpleDialogView : BaseView<SimpleDialogData>
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        protected override void OnInitialize()
        {
            // Setup button click handler
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }
        
        protected override void OnUpdateView(SimpleDialogData data)
        {
            if (data == null) return;
            
            // Update UI elements
            if (titleText != null)
                titleText.text = data.title;
                
            if (messageText != null)
                messageText.text = data.message;
                
            if (buttonText != null)
                buttonText.text = data.buttonText;
        }
        
        private void OnButtonClicked()
        {
            // Notify presenter through model
            var presenter = GetComponent<SimpleDialogPresenter>();
            presenter?.OnButtonClicked();
        }
        
        protected override void OnDispose()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }
            
            base.OnDispose();
        }
    }

    /// <summary>
    /// View for confirm dialog popup
    /// </summary>
    public class ConfirmDialogView : BaseView<ConfirmDialogData>
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private TextMeshProUGUI cancelButtonText;
        
        protected override void OnInitialize()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        protected override void OnUpdateView(ConfirmDialogData data)
        {
            if (data == null) return;
            
            if (titleText != null)
                titleText.text = data.title;
                
            if (messageText != null)
                messageText.text = data.message;
                
            if (confirmButtonText != null)
                confirmButtonText.text = data.confirmText;
                
            if (cancelButtonText != null)
                cancelButtonText.text = data.cancelText;
        }
        
        private void OnConfirmClicked()
        {
            var presenter = GetComponent<ConfirmDialogPresenter>();
            presenter?.OnConfirmClicked();
        }
        
        private void OnCancelClicked()
        {
            var presenter = GetComponent<ConfirmDialogPresenter>();
            presenter?.OnCancelClicked();
        }
        
        protected override void OnDispose()
        {
            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(OnConfirmClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelClicked);
                
            base.OnDispose();
        }
    }

    /// <summary>
    /// View for input dialog popup
    /// </summary>
    public class InputDialogView : BaseView<InputDialogData>
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private TextMeshProUGUI cancelButtonText;
        
        protected override void OnInitialize()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelClicked);
                
            if (inputField != null)
                inputField.onValueChanged.AddListener(OnInputChanged);
        }
        
        protected override void OnUpdateView(InputDialogData data)
        {
            if (data == null) return;
            
            if (titleText != null)
                titleText.text = data.title;
                
            if (messageText != null)
                messageText.text = data.message;
                
            if (inputField != null)
            {
                inputField.placeholder.GetComponent<TextMeshProUGUI>().text = data.placeholder;
                inputField.text = data.initialValue;
            }
                
            if (confirmButtonText != null)
                confirmButtonText.text = data.confirmText;
                
            if (cancelButtonText != null)
                cancelButtonText.text = data.cancelText;
        }
        
        private void OnInputChanged(string value)
        {
            var presenter = GetComponent<InputDialogPresenter>();
            presenter?.OnInputChanged(value);
        }
        
        private void OnConfirmClicked()
        {
            var presenter = GetComponent<InputDialogPresenter>();
            presenter?.OnConfirmClicked();
        }
        
        private void OnCancelClicked()
        {
            var presenter = GetComponent<InputDialogPresenter>();
            presenter?.OnCancelClicked();
        }
        
        protected override void OnDispose()
        {
            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(OnConfirmClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelClicked);
                
            if (inputField != null)
                inputField.onValueChanged.RemoveListener(OnInputChanged);
                
            base.OnDispose();
        }
    }

    /// <summary>
    /// View for loading popup
    /// </summary>
    public class LoadingPopupView : BaseView<LoadingPopupData>
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private GameObject progressContainer;
        
        protected override void OnInitialize()
        {
            // Initialize progress UI
            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
                progressSlider.value = 0f;
            }
        }
        
        protected override void OnUpdateView(LoadingPopupData data)
        {
            if (data == null) return;
            
            if (messageText != null)
                messageText.text = data.message;
                
            if (progressContainer != null)
                progressContainer.SetActive(data.showProgress);
                
            if (progressSlider != null)
                progressSlider.value = data.progress;
                
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(data.progress * 100)}%";
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
