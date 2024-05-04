using Event;
using ItemSystem.Produce;
using Manager;
using UI;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    public abstract class ProducerObject : InteractableObject
    {
        protected Producer data;
        private bool _isUIOpened = false;
        
        protected virtual void Start()
        {
            data = DataManager.GetCurrentProducer(GetProducerType());
            if (data == null)
            {
                gameObject.SetActive(false);
            }
            
            EventManager.Subscribe(gameObject, Message.OnUIClosed, OnUIClosed);
            EventManager.Subscribe(gameObject, Message.OnTryProducerUpgrade, OnTryProducerUpgrade);
            EventManager.Subscribe(gameObject, Message.OnClickProduce, OnClickProduce);
        }

        protected abstract ProducerType GetProducerType();
        protected abstract UIType GetUItype();

        public override void Interact()
        {
            if (_isUIOpened)
            {
                UIManager.Instance.Close(GetUItype());
                UIManager.Instance.Close(UIType.StorageUI);
                _isUIOpened = false;
            }
            else
            {
                UIManager.Instance.Open(GetUItype());
                UIManager.Instance.Open(UIType.StorageUI);
                _isUIOpened = true;
            }
        }
        
        private void Produce(int recipeId) => data.Produce(recipeId);
        
        private void OnClickProduce(EventManager.Event e)
        {
            try
            {
                Produce((int)e.Args[0]);
            }
            catch
            {
                Debug.LogError("[ProducerObject] OnClickProduce(): Invalid event argument");
            }
        }
        
        private void OnTryProducerUpgrade(EventManager.Event e)
        {
            try
            {
                if ((ProducerType)e.Args[0] != data.type)
                {
                    return;
                }
            }
            catch
            {
                Debug.LogError("[ProducerObject] OnTryProducerUpgrade(): Invalid event argument");
                return;
            }

            data.Upgrade();
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
            
            UIManager.Instance.Close(GetUItype());
            UIManager.Instance.Close(UIType.StorageUI);
            _isUIOpened = false;
        }
    }
}