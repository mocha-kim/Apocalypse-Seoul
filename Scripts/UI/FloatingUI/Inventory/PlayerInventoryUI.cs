using ItemSystem.Inventory;
using Manager;

namespace UI.FloatingUI.Inventory
{
    public class PlayerInventoryUI : InventoryUI<ItemSlotUI>
    {
        public override UIType GetUIType() => UIType.PlayerInventoryUI;
        public override ItemSystem.Inventory.Inventory GetInventory() => DataManager.PlayerInventory;

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