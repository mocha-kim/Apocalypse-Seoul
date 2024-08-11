using System.Collections;
using CharacterSystem.Stat;
using DataSystem;
using DataSystem.Database;
using ItemSystem.ItemBox;
using UI;
using UI.InGameUI;
using UnityEngine;
using UnityEngine.UI;

namespace InteractableObject
{
    public class ItemBoxObject : InteractableObject
    {
        [SerializeField] private int _id = -1;
        private ItemBox _data;
        
        private bool _isUnlocked = false;
        
        private GameObject _canvas = null;
        [SerializeField] private Image _interactGauge = null;
        
        private Camera _camera = null;
        private Coroutine _runningCoroutine = null;
        
        protected override void Awake()
        {
            if (_id < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            base.Awake();
            
            _camera = Camera.main;
            _canvas = GetComponentInChildren<Canvas>().gameObject;
            _interactGauge.gameObject.SetActive(false);
        }

        private void Start()
        {
            _data = Database.CreateItemBox(_id);
            if (_data.id < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            _data.SpawnItem();
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            
            _interactGauge.gameObject.SetActive(false);
            if (UIManager.OpenItemBoxID != -1)
            {
                UIManager.Instance.Close(UIType.ItemBoxUI);
            }

            if (_runningCoroutine == null)
            {
                return;
            }

            StopCoroutine(_runningCoroutine);
            _runningCoroutine = null;
        }

        protected override void SetConditionData(out string iconPath, out string text)
        {
            iconPath = Constants.Path.AttributeIconPath[AttributeType.Dexterity];
            text = DataManager.Stat.GetAttributeValue(AttributeType.Dexterity) + "/" + _data.requiredDexterity;
        }

        public override void Interact()
        {
            if (UIManager.OpenItemBoxID == _data.IdentifyKey)
            {
                UIManager.Instance.Close(UIType.ItemBoxUI);
                return;
            }
            
            if (_runningCoroutine != null)
            {
                StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }

            if (_isUnlocked)
            {
                UnlockBox();
                return;
            }
            
            if (!_data.IsUnlockable())
            {
                NotifyInteractCondition();
                return;
            }
            _runningCoroutine = StartCoroutine(WaitAndInteract());
        }

        private void UnlockBox()
        {
            UIManager.OpenItemBoxID = _data.IdentifyKey;
            UIManager.Instance.Open(UIType.ItemBoxUI);

            _isUnlocked = true; 
            _interactGauge.gameObject.SetActive(false);
            _runningCoroutine = null;
        }
        
        private IEnumerator WaitAndInteract()
        {
            _interactGauge.gameObject.SetActive(true);
            _interactGauge.fillAmount = 1f;
            var time = 0f;
            while (time < Constants.InteractDelay)
            {
                time += Time.deltaTime;
                _interactGauge.fillAmount = (Constants.InteractDelay - time) / Constants.InteractDelay;
                yield return null;
            }
    
            UnlockBox();
        }
    }
}
