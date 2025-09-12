using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using TMPro;

namespace Foundations.UIModules.UIComponents
{
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour
    {
        [SerializeField] private float clickDelay = 0.25f;
        
        [Header("UI elements")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText; 
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private EventTrigger eventTrigger;

        private event Action OnClick;
        private event Action OnPointerDown;
        private event Action OnPointerUp;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
            RegisterEventTrigger();
        }
        
        private void RegisterEventTrigger()
        {
            if (eventTrigger == null)
                return;
            
            var pointerDownTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDownTrigger.callback.AddListener(data => OnPointerDownCallback((PointerEventData) data));
            
            var pointerUpTrigger = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUpTrigger.callback.AddListener(data => OnPointerUpCallback((PointerEventData) data));
            
            eventTrigger.triggers.Add(pointerDownTrigger);
            eventTrigger.triggers.Add(pointerUpTrigger);
        }
        
        private void OnPointerDownCallback(PointerEventData eventData)
        {
            OnPointerDown?.Invoke();
        }

        private void OnPointerUpCallback(PointerEventData eventData)
        {
            OnPointerUp?.Invoke();
        }

        private void OnButtonClick()
        {
            OnClick?.Invoke();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            UniTask.Delay(TimeSpan.FromSeconds(clickDelay)).ContinueWith(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }).Forget();
        }
        
        public void SetButtonText(string text) => buttonText.text = text;

        public void AddOnClickListener(Action onClick) => OnClick += onClick;
        
        public void RemoveOnClickListener(Action onClick) => OnClick -= onClick;
        
        public void AddOnPointerDownListener(Action onPointerDown) => OnPointerDown += onPointerDown;
        
        public void RemoveOnPointerDownListener(Action onPointerDown) => OnPointerDown -= onPointerDown;
        
        public void AddOnPointerUpListener(Action onPointerUp) => OnPointerUp += onPointerUp;
        
        public void RemoveOnPointerUpListener(Action onPointerUp) => OnPointerUp -= onPointerUp;
        
        public void SetInteractable(bool isInteractable) => button.interactable = isInteractable;

        private void OnDestroy()
        {
            OnClick = null;
            button.onClick.RemoveAllListeners();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (button == null)
                button = GetComponent<Button>();
        }
#endif
    }
}
