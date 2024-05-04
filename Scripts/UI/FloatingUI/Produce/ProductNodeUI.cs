using DataSystem.Database;
using Event;
using ItemSystem.Item;
using ItemSystem.Produce;
using JetBrains.Annotations;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloatingUI.Produce
{
    public class ProductNodeUI : UIBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _defaultSlot;
        [SerializeField] private Sprite _selectedSlot;
        
        [SerializeField] private Image _iconImage;
        [SerializeField] [CanBeNull] private TextMeshProUGUI _nameText;
        [SerializeField] [CanBeNull] private TextMeshProUGUI _amountText;
        
        [SerializeField] [CanBeNull] private Button _button;

        private UIBase _contextProducerUI;
        
        private ItemRecipe _itemRecipe;
        private Item _product;
        
        public override UIType GetUIType() => UIType.ComponentUI;
        
        private void Awake()
        {
            if (_button == null)
            {
                return;
            }

            _button.OnClickAsObservable()
                .Subscribe(_ => OnClickProduct())
                .AddTo(gameObject);
            EventManager.Subscribe(gameObject, Message.OnClickProductNode, OnClickProductNode);
        }

        public void SetContext(UIBase producerUI)
        {
            _contextProducerUI = producerUI;
        }
        
        public void SetData(ItemRecipe itemRecipe)
        {
            _itemRecipe = itemRecipe;
            _product = Database.GetItem(itemRecipe.productId);
            
            InitUI();
        }
        
        private void InitUI()
        {
            if (_product == null)
            {
                return;
            }
            
            _background.sprite = _defaultSlot;
            _iconImage.sprite = ResourceManager.GetSprite(_product.iconPath); 
            if (_nameText != null)
            {
                _nameText.text = _product.name;
            }
            if (_amountText != null)
            {
                _amountText.text = _itemRecipe.productAmount.ToString();
            }
        }

        private void UpdateUI(bool isSelected)
        {
            if (_background == null)
            {
                _background = GetComponent<Image>();
            }
            _background.sprite = isSelected ? _selectedSlot : _defaultSlot;
        }

        private void OnClickProduct()
        {
            MouseData.FocusedUI = _contextProducerUI;
            EventManager.OnNext(Message.OnClickProductNode, _itemRecipe.id);
        }
        
        private void OnClickProductNode(EventManager.Event e)
        {
            var recipeId = -1;
            try
            {
                recipeId = (int)e.Args[0];
            }
            catch
            {
                Debug.LogError("[ProductNodeUI] OnClickProductNode(): Invalid event argument");
            }

            UpdateUI(recipeId == _itemRecipe.id);
        }
    }
}