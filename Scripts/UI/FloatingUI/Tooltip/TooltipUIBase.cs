using System;
using Event;
using ItemSystem.Item;
using Manager;
using UnityEngine;

namespace UI.FloatingUI.Tooltip
{
    public abstract class TooltipUIBase<T> : UIBase where T : class 
    {
        private Item _targetItem;
        private GameObject _renderRoot;
        protected RectTransform RectTransform;
        
        protected virtual void Awake()
        {
            _renderRoot = transform.GetChild(0).gameObject;
            RectTransform = GetComponent<RectTransform>();
            
            EventManager.Subscribe(gameObject, Message.OnTryTooltipOpen, OpenTooltip);
            EventManager.Subscribe(gameObject, Message.OnTryTooltipClose, _ => CloseTooltip());
        }

        public override void Open()
        {
            // This UI cannot be opened by UIManager, Do nothing
        }

        public override void Close()
        {
            // This UI cannot be closed by UIManager, Do nothing
        }

        protected abstract void SetTooltipInfo(T information);
        protected abstract void ResetTooltipInfo();
        
        private void OpenTooltip(EventManager.Event e)
        {
            if (e.Args.Length < 1 || e.Args[0] == null)
            {
                Debug.LogError("[TooltipUI] Information must be sent to Event.Args");
                return;
            }

            T data = null;
            try
            {
                data = e.Args[0] as T;
            }
            catch
            {
                Debug.LogError($"[TooltipUI] Argument type conversion failed. Target type muse be: {typeof(T)}");
                return;
            }
            
            SetTooltipInfo(data);
            RectTransform.position = Input.mousePosition;
            _renderRoot.SetActive(true);
        }
        
        private void CloseTooltip()
        {
            ResetTooltipInfo();
            _renderRoot.SetActive(false);
        }
    }
}