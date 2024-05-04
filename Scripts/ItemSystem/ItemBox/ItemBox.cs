using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CharacterSystem.Stat;
using DataSystem;
using DataSystem.Database;
using ItemSystem.Inventory;
using ItemSystem.Item;
using Manager;
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
            
            ItemSystem.Inventory.Inventory spawnedInventory = new(12, InventoryType.ItemBox);
            List<SpawnData> spawnDataList;
            var instantValue = 0;
            var sumRatio = 0f;
            
            spawnDataList = Database.GetSpawnDataList(id);
            sumRatio = spawnDataList.Max(x => x.spawnRate);
            instantValue = ItemUtils.GetCorrectedBoxValue(value);
            
            var totalValue = 0;
            while (totalValue < instantValue)
            {
                // pick random Item.
                var spawnItemID = 0;
                var target = Random.Range(0f, sumRatio);
                for (var i = 0; i < spawnDataList.Count; i++)
                {
                    if (target < spawnDataList[i].spawnRate)
                    {
                        spawnItemID = spawnDataList[i].itemId;
                        break;
                    }
                }
                
                // spawn Item.
                var item = Database.GetItem(spawnItemID);
                if (item == null)
                {
                    continue;
                }
                
                var min = 1;
                var max = Mathf.Min((instantValue - totalValue) / item.value + 1, item.maxStackCount);
                var amount = Mathf.Clamp( Random.Range(0, instantValue / item.value), min, max);
                totalValue += amount * item.value;

                spawnedInventory.AddItem(item, amount);
                
                // stop if inventory is full.
                if (spawnedInventory.HasEmptySlot == false)
                {
                    break;
                }
            }
            
            DataManager.SpawnedItemBoxData[IdentifyKey] = spawnedInventory;
            isSpawned = true;
        }
    }
}