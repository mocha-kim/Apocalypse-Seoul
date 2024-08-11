using EventSystem;
using UI;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    public class StorageObject : InteractableObject
    {
        private bool _isUIOpened = false;

        protected override void Awake()
        {
            base.Awake();
            
            EventManager.Subscribe(gameObject, Message.OnUIClosed, OnUIClosed);
        }
        
        public override void Interact()
        {
            if (_isUIOpened)
            {
                UIManager.Instance.Close(UIType.StorageUI);
                _isUIOpened = false;
            }
            else
            {
                UIManager.Instance.Open(UIType.StorageUI);
                _isUIOpened = true;
            }
        }
        
        private void OnUIClosed(EventManager.Event e)
        {
            UIType type = (UIType)e.Args[0];
            if (type == UIType.StorageUI)
            {
                _isUIOpened = false;
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            
            UIManager.Instance.Close(UIType.StorageUI);
            _isUIOpened = false;
        }
    }
}
