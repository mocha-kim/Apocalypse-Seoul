using System.ComponentModel;
using Event;
using Manager;
using UI.FloatingUI.Inventory;

namespace UI.FixedUI.EventUI.Deal
{
    public class DealItemSlotUI : ItemSlotUI
    {
        [Description ("Not self inventory, for quick item move")]
        private ItemSystem.Inventory.Inventory _pairInventory;
        
        protected override void OnMouseClick()
        {
            if (_slotData == null || _slotData.Amount <= 0 || !UIManager.Instance.IsOpened(UIType.DealUI))
            {
                return;
            }

            if (InputManager.Instance.IsFunctionDown)
            {
                _pairInventory.AddItem(_slotData.Item.id, _slotData.Amount);
                _slotData.RemoveItem(_slotData.Amount);
            }
            else
            {
                _pairInventory.AddItem(_slotData.Item.id, 1);
                _slotData.RemoveItem(1);
            }

            EventManager.OnNext(Message.OnUpdateInventory, _slotData.ParentType);
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
                if (MouseData.MouseHoveredSlot.ParentType == slot.ParentType ||
                    MouseData.MouseHoveredSlot.ParentType == _pairInventory.GetInventoryType())
                {
                    MouseData.MouseHoveredSlot.SwapItem(slot);
                    EventManager.OnNext(Message.OnUpdateInventory, MouseData.MouseHoveredSlot.ParentType);
                }
            }
            else if (MouseData.MouseHoveredInventory == null)
            {
                slot.ClearItem();
            }
            EventManager.OnNext(Message.OnUpdateInventory, slot.ParentType);
        }

        public float GetValue() =>  _slotData.GetValue();
        
        public void SetPairInventory(ItemSystem.Inventory.Inventory pairInventory) => _pairInventory = pairInventory;

        public void MoveItemTo(ItemSystem.Inventory.Inventory inventory)
        {
            if (_slotData != null && _slotData.Item != null)
            {
                inventory.AddItem(_slotData.Item.id, _slotData.Amount);
            }
            _slotData.ClearItem();
        }
    }
}