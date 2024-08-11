using DataSystem;
using DataSystem.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloatingUI.Produce
{
    public class MaterialNodeUI : UIBase
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        private ItemSystem.Produce.Material _data;
        
        public override UIType GetUIType() => UIType.ComponentUI;

        public void SetData(ItemSystem.Produce.Material data) => _data = data;

        public void UpdateUI()
        {
            if (_data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            var item = Database.GetItem(_data.id);
            _iconImage.sprite = ResourceManager.GetSprite(item.iconPath);
            
            var curAmount = DataManager.Storage.GetTotalAmount(_data.id);
            var colorCode = curAmount < _data.billAmount ? "orange" : "black";
            _amountText.text = $"<color={colorCode}>{curAmount}</color> / {_data.billAmount}";
        }
    }
}