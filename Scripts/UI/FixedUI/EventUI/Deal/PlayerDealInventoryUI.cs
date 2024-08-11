using DataSystem;
using EventSystem;
using ItemSystem.Inventory;

namespace UI.FixedUI.EventUI.Deal
{
    public class PlayerDealInventoryUI : DealInventoryUI
    {
        public override UIType GetUIType() => UIType.ComponentUI;
        public override Inventory GetInventory() => DataManager.PlayerInventory;
        protected override Inventory PairInventory => DealUI.PlayerDeal;

        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.Player)
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