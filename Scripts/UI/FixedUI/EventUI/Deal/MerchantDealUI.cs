using DataSystem;
using EventSystem;
using ItemSystem.Inventory;
using UnityEngine;

namespace UI.FixedUI.EventUI.Deal
{
    public class MerchantDealUI : DealInventoryUI
    {
        [SerializeField] private DealUI _dealUI;
        
        public override UIType GetUIType() => UIType.ComponentUI;
        public override Inventory GetInventory() => DealUI.MerchantDeal;
        protected override Inventory PairInventory => DataManager.MerchantInventories[DealUI.MerchantInventoryID];

        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.MerchantDeal)
            {
                return;
            }

            float value = 0;
            foreach (var slotUI in SlotUIs)
            {
                slotUI.UpdateSlot();
                value += slotUI.GetValue();
            }
            _dealUI.SetMerchantValue(value);
        }
        
        public override void ResetDeal()
        {
            foreach (var slotUI in SlotUIs)
            {
                slotUI.MoveItemTo(PairInventory);
            }
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.MerchantDeal);
        }
        
        public override void DoDeal()
        {
            foreach (var slotUI in SlotUIs)
            {
                slotUI.MoveItemTo(DataManager.PlayerInventory);
            }
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.MerchantDeal);
        }
    }
}