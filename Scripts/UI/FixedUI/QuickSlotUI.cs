using InputSystem;
using ItemSystem.Inventory;
using UI.FloatingUI.Inventory;

namespace UI.FixedUI
{
    public class QuickSlotUI : ItemSlotUI
    {
        public void SyncSlotItemAmount()
        {
            if (_slotData.Item == null)
            {
                return;
            }
            var quickSlot = _slotData as QuickSlot;
            quickSlot.SyncItemAmount();
        }

        protected override void OnSwapItem(ItemSlot slot)
        {
            if (MouseData.MouseHoveredSlot.ParentType == InventoryType.QuickSlot)
            {
                MouseData.MouseHoveredSlot.SwapItem(slot);
            }
            else
            {
                slot.ClearItem();
            }
        }
    }
}