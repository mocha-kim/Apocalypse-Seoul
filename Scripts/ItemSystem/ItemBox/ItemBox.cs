using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CharacterSystem.Stat;
using DataSystem;
using DataSystem.Database;
using ItemSystem.Inventory;
using ItemSystem.Item;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ItemSystem.ItemBox
{
    [Serializable]
    public class ItemBox
    {
        [Description("Id to identify spawned item box in world")] public int objectId = 0;
        [Description("Id to get data from database")] public int id = -1;
        
        public bool isSpawned = false;

        public int value = -1;
        public int requiredDexterity = -1;
        public string spritePath = "";
        
        public int IdentifyKey => id * Constants.Database.IndividualIdRange + objectId;
        
        private static int _min = 1;

        public ItemBox(int id, int value, int requiredDexterity, string spritePath)
        {
            this.id = id;
            this.value = value;
            this.requiredDexterity = requiredDexterity;
            this.spritePath = spritePath;
            
            while (DataManager.SpawnedItemBoxData.ContainsKey(IdentifyKey))
            {
                objectId++;
            }
        }

        public ItemBox(ItemBox other) : this(other.id, other.value, other.requiredDexterity, other.spritePath)
        {
        }

        public bool IsUnlockable()
        {
            return DataManager.Stat.GetAttributeValue(AttributeType.Dexterity) >= requiredDexterity;
        }

        public void SpawnItem()
        {
            if (isSpawned)
            {
                return;
            }

            if (DataManager.SpawnedItemBoxData[IdentifyKey] == null)
            {
                DataManager.SpawnedItemBoxData[IdentifyKey] = new Inventory.Inventory(12, InventoryType.ItemBox);
            }
            else
            {
                DataManager.SpawnedItemBoxData[IdentifyKey].Clear();
            }
            var spawnedInventory = DataManager.SpawnedItemBoxData[IdentifyKey];
            var instantValue = 0;
            var sumRatio = 0f;
            
            var spawnDataList = Database.GetSpawnDataList(id);
            sumRatio = spawnDataList.Max(x => x.spawnRate);
            instantValue = ItemUtils.GetCorrectedBoxValue(value);
            
            var totalValue = 0;
            while (totalValue < instantValue)
            {
                var spawnItemID = 0;
                var target = Random.Range(0f, sumRatio);
                foreach (var data in spawnDataList)
                {
                    if (target < data.spawnRate)
                    {
                        spawnItemID = data.itemId;
                        break;
                    }
                }
                
                var item = Database.GetItem(spawnItemID);
                if (item == null)
                {
                    continue;
                }

                var max = Mathf.Min((instantValue - totalValue) / item.value + 1, item.maxStackCount);
                var amount = Mathf.Clamp( Random.Range(0, instantValue / item.value), _min, max);
                totalValue += amount * item.value;
                
                spawnedInventory.AddItem(item, amount);
                if (spawnedInventory.HasEmptySlot == false)
                {
                    break;
                }
            }
            
            isSpawned = true;
        }
    }
}