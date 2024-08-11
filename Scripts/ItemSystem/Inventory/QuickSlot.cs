using System.Linq;
using DataSystem;
using EventSystem;
using ItemSystem.Item;

namespace ItemSystem.Inventory
{
    public class QuickSlot : ItemSlot
    {
        private static ItemType[] _allowedType = { ItemType.Consumable };
        
        public QuickSlot()
            : base(_allowedType.ToList() , InventoryType.QuickSlot)
        {}

        public QuickSlot(Item.Item item, int amount)
            : base(_allowedType.ToList(), InventoryType.QuickSlot, item, amount)
        {}

        public override bool UseItem(int value)
        {
            if (Amount <= 0 || Item == null || Item.id < 0)
            {
                return false;
            }
            
            DataManager.PlayerInventory.UseItem(Item.id, value);
            SyncItemAmount();
            
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
            return true;
        }

        public override bool AddItem(int value)
        {
            Amount += value;
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
            return true;
        }

        public override bool AllocateItem(Item.Item newItem, int value)
        {
            var isSuccess = base.AllocateItem(newItem, value);
            if (!isSuccess)
            {
                return false;
            }
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
            return true;
        }

        public override void SwapItem(ItemSlot other)
        {
            base.SwapItem(other);
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
        }

        public override void RemoveItem(int amount)
        {
            base.RemoveItem(amount);
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
        }

        public override void ClearItem()
        {
            base.ClearItem();
            EventManager.OnNext(Message.OnUpdateInventory, ParentType);
        }

        public void SyncItemAmount()
        {
            if (Item == null)
            {
                return;
            }
            Amount = DataManager.PlayerInventory.GetTotalAmount(Item.id);
        }
    }
}