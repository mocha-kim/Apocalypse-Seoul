using System.Collections.Generic;
using CharacterSystem.Stat;
using DataSystem;
using DataSystem.Database;
using ItemSystem.Inventory;
using ItemSystem.Produce;
using Settings.Scene;

namespace Manager
{
    public static class DataManager
    {
        public static PlayerStat Stat;
        
        public static Inventory PlayerInventory;
        public static Inventory Storage;
        
        public static Dictionary<int, Inventory> MerchantInventories;
        
        public static QuickSlot[] QuickSlots;
        public static Dictionary<int, Inventory> SpawnedItemBoxData;
        
        public static MapData CurrentMap { get; set; }

        public static void Init()
        {
            Stat = new PlayerStat();

            PlayerInventory = new PlayerInventory(Constants.Inventory.DefaultInventorySize, Constants.Inventory.MaxInventorySize);
            Storage = new Inventory(Constants.Inventory.MaxStorageSize, InventoryType.Storage);
            MerchantInventories = new Dictionary<int, Inventory>();

            QuickSlots = new QuickSlot[Constants.Inventory.QuickSlotSize];
            for (var i = 0; i < Constants.Inventory.QuickSlotSize; i++)
            {
                QuickSlots[i] = new QuickSlot();
            }

            SpawnedItemBoxData = new Dictionary<int, Inventory>();
        }

        public static Producer GetCurrentProducer(ProducerType type) => Database.GetProducer(type, Stat.ProducerLevel[type]);
    }
}