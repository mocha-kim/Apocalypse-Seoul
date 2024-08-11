using System;
using System.Collections.Generic;
using System.Linq;
using DataSystem.Database;
using EventSystem;
using ItemSystem.Item;
using Newtonsoft.Json;
using UnityEngine;

namespace ItemSystem.Inventory
{
    [Serializable]
    public class Inventory
    {
        protected InventoryType Type;
        public List<ItemType> AllowedType { get; private set; }
        
        public List<ItemSlot> slots = new();
        public int Size => slots.Count;
        public bool HasEmptySlot => GetEmptySlotCount() > 0;

        public Inventory(int slotSize, InventoryType type)
        {
            Type = type;
            AllowedType = type switch
            {
                InventoryType.QuickSlot => new List<ItemType>(1)
                {
                    ItemType.Consumable
                },
                _ => new List<ItemType>(3)
                {
                    ItemType.Normal,
                    ItemType.Consumable,
                }
            };
            
            for (var i = 0; i < slotSize; i++)
            {
                slots.Add(new ItemSlot(AllowedType, type));
            }
        }
        
        public Inventory(int activatedSlotSize, int maxSlotSize, InventoryType type)
        {
            Type = type;
            AllowedType = type switch
            {
                InventoryType.QuickSlot => new List<ItemType>(1)
                {
                    ItemType.Consumable
                },
                _ => new List<ItemType>(3)
                {
                    ItemType.Normal,
                    ItemType.Consumable,
                }
            };
            
            for (var i = 0; i < activatedSlotSize; i++)
            {
                slots.Add(new ItemSlot(AllowedType, type));
            }
            
            for (var i = activatedSlotSize; i < maxSlotSize; i++)
            {
                slots.Add(new ItemSlot(AllowedType, type, false));
            }
        }
        
        [JsonConstructor]
        public Inventory(){}

        public int GetTotalAmount(int id) => slots
            .Where(slot => slot.Amount > 0 && slot.Item.id == id)
            .Sum(slot => slot.Amount);

        public int GetEmptySlotCount() => slots.Count(slot => slot.IsActivated && slot.Amount <= 0);
        
        private ItemSlot GetEmptySlot() => slots
            .FirstOrDefault(slot => slot.IsActivated && slot.Amount <= 0);

        // returns added item amount
        private int AddNewItem(Item.Item item, int amount)
        {
            if (item == null || amount < 1 || !AllowedType.Contains(item.type))
            {
                return 0;
            }

            var addedAmount = 0;
            var remainAmount = amount;
            var maxStackCount = item.maxStackCount;
            var requiredSlotCount = amount / maxStackCount + (amount % maxStackCount == 0 ? 0 : 1);
            ItemSlot slot;
            for (int i = 0; i < requiredSlotCount - 1; i++)
            {
                slot = GetEmptySlot();
                if (slot == null)
                {
                    return addedAmount;
                }
                slot.AllocateItem(item, maxStackCount);
                remainAmount -= maxStackCount;
                addedAmount += maxStackCount;
            }

            slot = GetEmptySlot();
            if (slot != null)
            {
                slot.AllocateItem(item, remainAmount);
                addedAmount += remainAmount;
            }
            return addedAmount;
        }

        // returns added item amount
        public virtual int AddItem(Item.Item item, int amount)
        {
            if (item == null || amount == 0)
            {
                return 0;
            }

            var addedAmount = 0;
            // get a slot that item has already allocated
            var slot = slots.FirstOrDefault(slot => slot.Amount > 0
                                                     && slot.Item.id == item.id
                                                     && slot.Amount < slot.Item.maxStackCount);
            
            if (slot == null)   // if the item is new in this inventory
            {
                addedAmount = AddNewItem(item, amount);
                if (addedAmount == 0)
                {
                    Debug.LogWarning("There's no space in inventory");
                    return 0;
                }
            }
            else                // if there is slot that item has already allocated
            {
                // if [new value + existing value] is over max stack count
                if (slot.Amount + amount > item.maxStackCount)          
                {
                    var remain = item.maxStackCount - slot.Amount;
                    amount -= remain;

                    slot.AddItem(remain);
                    addedAmount = remain;
                    
                    addedAmount += AddNewItem(item, amount);
                    if (addedAmount <= remain)
                    {
                        Debug.LogWarning("There's no space in inventory");
                    }
                }
                else
                {
                    slot.AddItem(amount);
                    addedAmount = amount;
                }
            }

            EventManager.OnNext(Message.OnUpdateInventory, Type);
            return addedAmount;
        }

        // returns added item amount
        public int AddItem(int id, int amount) => AddItem(Database.GetItem(id), amount);

        public virtual bool UseItemBySlotIndex(int id, int amount)
        {
            if (slots.Count <= id)
            {
                Debug.LogWarning($"Inventory.UseItemBySlotIndex(): Slots OutOfIndex)");
                return false;
            }

            if (!slots[id].UseItem(amount))
            {
                return false;
            }
            
            EventManager.OnNext(Message.OnUpdateInventory, Type);
            return true;
        }
        
        public virtual bool UseItem(int id, int amount)
        {
            if (GetTotalAmount(id) < amount)
            {
                Debug.LogWarning($"There are not enough items(id:{id}) in the inventory");
                return false;
            }

            while(amount > 0)
            {
                var slot = slots.LastOrDefault(slot => slot.Amount > 0 && slot.Item.id == id);
                if (amount > slot.Amount)
                {
                    amount -= slot.Amount;
                    slot.UseItem(slot.Amount);
                }
                else
                {
                    slot.UseItem(amount);
                    amount = 0;
                }
            }

            EventManager.OnNext(Message.OnUpdateInventory, Type);
            return true;
        }
        
        public virtual bool RemoveItem(int id, int amount)
        {
            while(amount > 0)
            {
                var slot = slots.LastOrDefault(slot => slot.Amount > 0 && slot.Item.id == id);
                if (slot == null)
                {
                    break;
                }
                if (amount > slot.Amount)
                {
                    amount -= slot.Amount;
                    slot.RemoveItem(slot.Amount);
                }
                else
                {
                    slot.ClearItem();
                    break;
                }
            }

            EventManager.OnNext(Message.OnUpdateInventory, Type);
            return true;
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                slot.ClearItem();
            }
            
            Debug.Log("Clear Inventory");
            EventManager.OnNext(Message.OnUpdateInventory, Type);
        }

        public InventoryType GetInventoryType()
        {
            return Type;
        }
    }
}