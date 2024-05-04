using Event;
using ItemSystem.Inventory;
using Manager;
using UnityEngine;

namespace UI.FixedUI.EventUI.Deal
{
    public class PlayerDealUI : DealInventoryUI
    {
        [SerializeField] private DealUI _dealUI;
        
        public override UIType GetUIType() => UIType.ComponentUI;
        public override Inventory GetInventory() => DealUI.PlayerDeal;
        protected override Inventory PairInventory => DataManager.PlayerInventory;

        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.PlayerDeal)
            {
                return;
            }

            float value = 0;
            foreach (var slotUI in SlotUIs)
            {
                slotUI.UpdateSlot();
                var str = (slotUI.GetValue() * 0.9f).ToString("0.0");
                value += float.Parse(str);
            }
            _dealUI.SetPlayerValue(value);
        }

        public override void ResetDeal()
        {
            foreach (var slotUI in SlotUIs)
            {
                slotUI.MoveItemTo(PairInventory);
            }
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.PlayerDeal);
        }
        
        public override void DoDeal()
        {
            foreach (var slotUI in SlotUIs)
            {
                slotUI.MoveItemTo(DataManager.MerchantInventories[DealUI.MerchantInventoryID]);
            }
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.PlayerDeal);
        }
    }
}