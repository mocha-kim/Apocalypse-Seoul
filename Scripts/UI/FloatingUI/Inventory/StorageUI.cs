using DataSystem;
using EventSystem;
using ItemSystem.Inventory;

namespace UI.FloatingUI.Inventory
{
    public class StorageUI : InventoryUI<ItemSlotUI>
    {
        public override UIType GetUIType() => UIType.StorageUI;
        public override ItemSystem.Inventory.Inventory GetInventory() => DataManager.Storage;
        
        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.Storage)
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