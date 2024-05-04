using Event;
using ItemSystem.Inventory;
using Manager;
using UI.FloatingUI.Inventory;
using UnityEngine;

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

        protected override void OnEndDragItem()
        {
            Destroy(MouseData.DraggingItem);
            
            var slot = MouseData.DragBeginSlot;
            if (slot == null || slot.Amount <= 0)
            {
                return;
            }
            MouseData.DragBeginSlot = null;
            
            if (MouseData.MouseHoveredSlot != null)
            {
                if (MouseData.MouseHoveredSlot.ParentType == InventoryType.QuickSlot)
                {
                    MouseData.MouseHoveredSlot.SwapItem(slot);
                }
                else
                {
                    slot.ClearItem();
                }
                EventManager.OnNext(Message.OnUpdateInventory, MouseData.MouseHoveredSlot.ParentType);
            }
            else if (MouseData.MouseHoveredInventory == null)
            {
                slot.ClearItem();
            }
            Debug.Log(slot.ParentType);
            EventManager.OnNext(Message.OnUpdateInventory, slot.ParentType);
        }
    }
}