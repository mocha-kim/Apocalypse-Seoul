using DataSystem;
using EventSystem;
using ItemSystem.Produce;
using UI;
using UnityEngine;

namespace InteractableObject
{
    public class ProducerObject : InteractableObject
    {
        protected Producer data = null;
        private bool _isUIOpened = false;

        [SerializeField] private ProducerType _producerType;
        
        protected virtual ProducerType GetProducerType() => _producerType;
        protected virtual UIType GetUIType() => UIType.ProducerUI;
        
        protected virtual void Start()
        {
            data ??= DataManager.GetCurrentProducer(GetProducerType());
            if (data == null)
            {
                gameObject.SetActive(false);
            }
            
            EventManager.Subscribe(gameObject, Message.OnUIClosed, OnUIClosed);
            EventManager.Subscribe(gameObject, Message.OnTryProducerUpgrade, OnTryProducerUpgrade);
            EventManager.Subscribe(gameObject, Message.OnClickProduce, OnClickProduce);
        }

        public override void Interact()
        {
            if (_isUIOpened)
            {
                UIManager.Instance.Close(GetUIType());
            }
            else
            {
                UIManager.OpenProducerId = data.id;
                UIManager.OpenProducerType = data.type;
                UIManager.Instance.Open(GetUIType());
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
            if (type == GetUIType())
            {
                UIManager.OpenProducerId = -1;
                UIManager.OpenProducerType = null;
                _isUIOpened = false;
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            
            UIManager.Instance.Close(GetUIType());
        }
    }
}