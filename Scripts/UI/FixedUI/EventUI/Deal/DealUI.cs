using DataSystem.Database;
using Event;
using ItemSystem.Inventory;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI.EventUI.Deal
{
    public class DealUI : EventUIBase
    {
        [SerializeField] private Button _dealButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private TMP_Text _playerValueText;
        [SerializeField] private TMP_Text _merchantValueText;
        
        private float _playerValue  = 0;
        private float _merchantValue  = 0;

        [SerializeField] private Transform _arrow;
        private float _arrowAngle = 0;
        
        private int _nextScriptId;

        public override UIType GetUIType() => UIType.DealUI;

        public static int MerchantInventoryID { get; private set; } = -1;

        public static Inventory PlayerDeal;
        public static Inventory MerchantDeal;

        public override void Init()
        {
            base.Init();
            
            PlayerDeal = new Inventory(10, InventoryType.PlayerDeal);
            MerchantDeal = new Inventory(10, InventoryType.MerchantDeal);
            
            _dealButton.onClick.AddListener(DoDeal);
            _resetButton.onClick.AddListener(ResetDeal);
            _closeButton.onClick.AddListener(Close);
        }

        public override void Open()
        {
            base.Open();
            if (MerchantInventoryID == -1)
            {
                Close();
                Debug.LogError("[DealUI]Invalid MerchantInventoryID");
                return;
            }
            
            EventManager.OnNext(Message.OnDealUILink);
        }
        
        public override void Close()
        {
            ResetDeal();
            base.Close();
        }

        public override int Next(int curId)
        {
            Close();
            return _nextScriptId;
        }

        public void SetEventInfo(int npcId, int nextScriptId)
        {
            MerchantInventoryID = Database.GetNPC(npcId).inventoryID;
            _nextScriptId = nextScriptId;

            EventManager.OnNext(Message.OnDealUIInit);
        }

        public void SetPlayerValue(float value)
        {
            _playerValue = value;
            _playerValueText.text = value.ToString();
            UpdateValue();
        }

        public void SetMerchantValue(float value)
        {
            _merchantValue = value;
            _merchantValueText.text = value.ToString();
            UpdateValue();
        }

        private void UpdateValue()
        {
            _arrowAngle = _merchantValue - _playerValue;
            _arrowAngle = Mathf.Clamp(_arrowAngle, -90f, 90f);
            
            UpdateArrow();
        }

        private void UpdateArrow()
        {
            _arrow.rotation = Quaternion.Euler(0, 0, _arrowAngle);
        }

        private void ResetDeal()
        {
            EventManager.OnNext(Message.OnResetDeal);
        }

        private void DoDeal()
        {
            if (_arrowAngle <= 0)
            {
                EventManager.OnNext(Message.OnDoDeal);
            }
        }
    }
}