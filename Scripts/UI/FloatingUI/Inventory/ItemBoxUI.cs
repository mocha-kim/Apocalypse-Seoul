using ItemSystem.Inventory;
using Manager;

namespace UI.FloatingUI.Inventory
{
    public class ItemBoxUI : InventoryUI<ItemSlotUI>
    {
        public override UIType GetUIType() => UIType.ItemBoxUI;

        public override ItemSystem.Inventory.Inventory GetInventory()
        {
            return DataManager.SpawnedItemBoxData.TryGetValue(UIManager.OpenItemBoxID, out var itemBox)
                ? itemBox : null;
        }

        public override void Close()
        {
            base.Close();
            UIManager.OpenItemBoxID = -1;
        }
        
        protected override void OnUpdateInventory(EventManager.Event e)
        {
            if ((InventoryType)e.Args[0] != InventoryType.ItemBox)
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