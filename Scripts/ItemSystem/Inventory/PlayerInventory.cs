using Event;
using Manager;
using UnityEngine;

namespace ItemSystem.Inventory
{
    public class PlayerInventory : Inventory
    {
        public PlayerInventory(int slotSize) : base(slotSize, InventoryType.Player)
        {
        }

        public PlayerInventory(int activatedSlotSize, int maxSlotSize) : base(activatedSlotSize, maxSlotSize, InventoryType.Player)
        {
        }

        public override int AddItem(Item.Item item, int amount)
        {
            var result = base.AddItem(item, amount);
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.QuickSlot);

            return result;
        }

        public override bool UseItem(int id, int amount)
        {
            var result =  base.UseItem(id, amount);
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.QuickSlot);

            return result;
        }

        public override bool UseItemBySlotIndex(int id, int amount)
        {
            var result =  base.UseItemBySlotIndex(id, amount);
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.QuickSlot);

            return result;
        }

        public override bool RemoveItem(int id, int amount)
        {
            var result =  base.RemoveItem(id, amount);
            EventManager.OnNext(Message.OnUpdateInventory, InventoryType.QuickSlot);

            return result;
        }
    }
}