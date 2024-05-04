using ItemSystem.Inventory;
using Manager;

namespace UI.FixedUI.EventUI.Deal
{
    public class MerchantDealInventoryUI : DealInventoryUI
    {
        public override UIType GetUIType() => UIType.ComponentUI;
        public override Inventory GetInventory() => DataManager.MerchantInventories[DealUI.MerchantInventoryID];
        protected override Inventory PairInventory => DealUI.MerchantDeal;

        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.Merchant)
            {
                return;
            }

            foreach (var slotUI in SlotUIs)
            {
                slotUI.UpdateSlot();
            }
        }
    }
}