using System;
using System.Collections;
using System.Collections.Generic;
using Alpha;
using CharacterSystem.Effect;
using DataSystem;
using DataSystem.Database;
using Event;
using ItemSystem.Item;
using Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace ItemSystem.Inventory
{
    [Serializable]
    public class ItemSlot
    {
        [JsonProperty]
        private List<ItemType> _allowedType;

        public bool IsActivated { get; protected set; }
        public InventoryType ParentType { get; protected set; }
        public Item.Item Item { get; protected set; }
        public int Amount  { get; protected set; }
        public Effect ItemEffect { get; protected set; }

        public bool IsInValid => !IsActivated || Amount <= 0 || Item == null;

        public ItemSlot(List<ItemType> allowedType, InventoryType parentType, bool isActivated = true)
        {
            _allowedType = allowedType;
            ParentType = parentType;
            Item = null;
            Amount = 0;
            IsActivated = isActivated;
        }

        public ItemSlot(List<ItemType> allowedType, InventoryType parentType, Item.Item item, int amount)
        {
            _allowedType = allowedType;
            ParentType = parentType;
            Item = item;
            Amount = amount;
            IsActivated = true;
        }
        
        [JsonConstructor]
        public ItemSlot(){}
        
        public float GetValue() => Item?.value * Amount ?? 0;
        public List<ItemType> GetAllowedType() => _allowedType;

        public void SetActive(bool value) => IsActivated = value;

        public virtual bool UseItem(int value)
        {
            if (IsInValid)
            {
                Debug.LogWarning($"ItemSlot.UseItem(): Slot is not valid");
                return false;
            }
            if (Amount < value)
            {
                Debug.LogWarning($"ItemSlot.UseItem(): There are not enough items(id:{Item.id})");
                return false;
            }

            if (Item.type == ItemType.Consumable && value != 1)
            {
                Debug.LogWarning($"ItemSlot.UseItem(): Consumable Item must be used one by one.");
                return false;
            }
            
            Amount -= value;
            if (Item is ConsumeItem consumeItem)
            {
                EventManager.OnNext(Message.OnItemUsed, consumeItem.effectId);
                return true;
            }
            if (Amount <= 0)
            {
                Item = null;
            }

            return true;
        }

        public virtual bool AddItem(int value)
        {
            if (Amount + value > Item.maxStackCount)
            {
                return false;
            }
            Amount += value;
            return true;
        }
        
        public virtual bool AllocateItem(Item.Item newItem, int value)
        {
            if (!_allowedType.Contains(newItem.type))
            {
                return false;
            }
            Item = newItem;
            Amount = value;
            return true;
        }
        
        public virtual void AllocateItem(int id, int value)
        {
            Item = Database.GetItem(id);
            AllocateItem(Item, value);
        }

        public virtual void SwapItem(ItemSlot other)
        {
            if (other == null || other.Amount <= 0)
            {
                return;
            }

            if (!_allowedType.Contains(other.Item.type))
            {
                return;
            }

            if (Amount <= 0)
            {
                Item = other.Item;
                Amount = other.Amount;
                other.ClearItem();
                return;
            }
            
            if (!other._allowedType.Contains(Item.type))
            {
                return;
            }
            
            if (Item.id == other.Item.id)
            {
                if (Amount == Item.maxStackCount || other.Amount == Item.maxStackCount)
                {
                    (other.Amount, Amount) = (Amount, other.Amount);
                }
                if (Amount + other.Amount > Item.maxStackCount)
                {
                    var remain = Amount + other.Amount - Item.maxStackCount;
                    Amount = Item.maxStackCount;
                    other.Amount = remain;
                }
                else
                {
                    Amount += other.Amount;
                    other.ClearItem();
                }
                return;
            }

            (other.Item, Item) = (Item, other.Item);
            (other.Amount, Amount) = (Amount, other.Amount);
        }

        public virtual void RemoveItem(int amount)
        {
            Amount -= amount;
            Amount = Amount < -1 ? -1 : Amount;
        }

        public virtual void ClearItem()
        {
            Item = null;
            Amount = 0;
        }
    }
}