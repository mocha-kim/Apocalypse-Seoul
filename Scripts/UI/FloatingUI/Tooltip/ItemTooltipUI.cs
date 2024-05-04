using DataSystem.Database;
using ItemSystem.Item;
using Manager;
using TMPro;
using UnityEngine;

namespace UI.FloatingUI.Tooltip
{
    public class ItemTooltipUI : TooltipUIBase<Item>
    {
        private Vector2 _offsetSize;
        
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private GameObject _effectListRoot;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public override UIType GetUIType() => UIType.ItemTooltipUI;

        private GameObject _effectInfoUI;
        private Vector2 _effectInfoUISize;

        private RectTransform _descriptionRect;
        private Vector2 _descriptionOffset;

        protected override void Awake()
        {
            base.Awake();

            _offsetSize = RectTransform.sizeDelta;
            
            _effectInfoUI = ResourceManager.GetPrefab("EffectInfoUI");
            _effectInfoUISize = _effectInfoUI.GetComponent<RectTransform>().sizeDelta;

            _descriptionRect = _descriptionText.GetComponent<RectTransform>();
            _descriptionOffset = _descriptionRect.anchoredPosition;
        }

        protected override void SetTooltipInfo(Item information)
        {
            _titleText.text = information.name + " <color=grey>";
            _titleText.text += information.type switch
            {
                ItemType.Normal => Database.ToString(Database.Key.ItemTypeNormal),
                ItemType.Consumable => Database.ToString(Database.Key.ItemTypeConsume),
                _ => ""
            } + "</color>";
            _descriptionText.text = information.description;

            if (information is not ConsumeItem consumeItem)
            {
                _effectListRoot.SetActive(false);
                RectTransform.sizeDelta = _offsetSize;
                return;
            }

            var index = 0;
            var effectList = Database.GetEffect(consumeItem.effectId)?.GetEffectInfo();
            if (effectList == null)
            {
                _effectListRoot.SetActive(false);
                RectTransform.sizeDelta = _offsetSize;
                return;
            }

            for (index = 0; index < effectList.Count; index++)
            {
                var pair = effectList[index];
                GameObject effectGameObject;
                if (index < _effectListRoot.transform.childCount)
                {
                    effectGameObject = _effectListRoot.transform.GetChild(index).gameObject;
                    effectGameObject.SetActive(true);
                }
                else
                {
                    effectGameObject = Instantiate(_effectInfoUI, _effectListRoot.transform);
                    effectGameObject.name += " " + index;
                }

                // TODO: Attribute icon get.. from where??
                // _effectIcon.sprite = 
                effectGameObject.GetComponentInChildren<TextMeshProUGUI>().text = pair.Key + " " + pair.Value;
                effectGameObject.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0, -index * _effectInfoUISize.y);
            }

            _effectListRoot.SetActive(index != 0);
            _descriptionRect.anchoredPosition = _descriptionOffset - new Vector2(0f, index * _effectInfoUISize.y);
            RectTransform.sizeDelta = new Vector2(_offsetSize.x, _offsetSize.y + index * _effectInfoUISize.y);
        }

        protected override void ResetTooltipInfo()
        {
            _titleText.text = "";
            for (var i = 0; i < _effectListRoot.transform.childCount; i++)
            {
                _effectListRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
            _effectListRoot.SetActive(false);
            _descriptionText.text = "";
        }
    }
}