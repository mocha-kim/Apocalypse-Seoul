using System.Collections.Generic;
using CharacterSystem.Stat;
using DataSystem.SaveLoad;
using EnvironmentSystem;
using EnvironmentSystem.Time;
using EventSystem;
using InputSystem.UserActionBind;
using ItemSystem.Inventory;
using ItemSystem.Produce;
using Settings;
using Settings.Scene;
using UnityEngine;

namespace DataSystem
{
    public static class DataManager
    {
        public static PlayerStat Stat;
        
        public static Inventory PlayerInventory;
        public static Inventory Storage;
        public static QuickSlot[] QuickSlots;
        
        public static Dictionary<int, Inventory> MerchantInventories;
        public static Dictionary<int, Inventory> SpawnedItemBoxData;
        
        public static MapData CurrentMap { get; set; }

        public static void Init()
        {
            Stat = new PlayerStat();

            PlayerInventory = new PlayerInventory(Constants.Inventory.DefaultInventorySize, Constants.Inventory.MaxInventorySize);
            Storage = new Inventory(Constants.Inventory.MaxStorageSize, InventoryType.Storage);
            QuickSlots = new QuickSlot[Constants.Inventory.QuickSlotSize];
            for (var i = 0; i < Constants.Inventory.QuickSlotSize; i++)
            {
                QuickSlots[i] = new QuickSlot();
            }

            MerchantInventories = new Dictionary<int, Inventory>();
            SpawnedItemBoxData = new Dictionary<int, Inventory>();
        }
        
        public static Producer GetCurrentProducer(ProducerType type)
        {
            if (type == ProducerType.Field)
            {
                return null;
            }
            return Database.Database.GetProducer(type, Stat.ProducerLevel[type]);
        }

        public static void Save()
        {
            SaveSystem.SaveData(Stat, "Stat");
            
            SaveSystem.SaveData(PlayerInventory, "PlayerInventory");
            SaveSystem.SaveData(Storage, "Storage");
            SaveSystem.SaveData(QuickSlots, "QuickSlots");

            SaveSystem.SaveData(MerchantInventories, "Merchant");
            SaveSystem.SaveData(SpawnedItemBoxData, "SpawnedItemBoxData");

            SaveSystem.SaveData(CurrentMap, "CurrentMap");
            var position = GameManager.Instance.Player.transform.position;
            SaveSystem.SaveData(position.x + " " + position.y, "PlayerPosition");
            SaveSystem.SaveData(TimeManager.GetDataString(), "TimeManager");
            
            SaveSystem.SaveData(InputBinding.Bindings, "InputBinding");

            EventManager.OnNext(Message.OnDataSaved);
        }

        public static void Load()
        {
            Stat = SaveSystem.LoadData("Stat", Stat);
            
            PlayerInventory = SaveSystem.LoadData("PlayerInventory", PlayerInventory);
            Storage = SaveSystem.LoadData("Storage", Storage);
            QuickSlots = SaveSystem.LoadData("QuickSlots", QuickSlots);

            MerchantInventories = SaveSystem.LoadData("Merchant", MerchantInventories);
            SpawnedItemBoxData = SaveSystem.LoadData("SpawnedItemBoxData", SpawnedItemBoxData);

            CurrentMap = SaveSystem.LoadData("CurrentMap", CurrentMap);
            string[] characterPosition = SaveSystem.LoadData("PlayerPosition", TimeManager.GetDayString()).Split();
            var position = new Vector3(float.Parse(characterPosition[0]), float.Parse(characterPosition[1]));
            string[] timeData = SaveSystem.LoadData("TimeManager", TimeManager.GetDayString()).Split(":");
            TimeManager.SetTime(timeData);

            InputBinding.Bindings = SaveSystem.LoadData("InputBinding", InputBinding.Bindings);
            
            EventManager.OnNext(Message.OnTrySceneLoad, CurrentMap.id, position);
            EventManager.OnNext(Message.OnDataLoaded);
        }

        public static void Reset()
        {
            SaveSystem.ResetData();
        }

        public static void ExecuteDeadPenalty()
        {
            Stat.SetAttributeValue(
                AttributeType.Hp
                , (int)(Stat.Attributes[AttributeType.Hp].BaseValue * Constants.Character.HPPenaltyRatio)
            );
            Stat.SetAttributeValue(AttributeType.Sp, 0);
            Stat.AddAttributeValue(AttributeType.Hunger, -50);
            Stat.AddAttributeValue(AttributeType.Thirst, -50);
            
            DataManager.PlayerInventory.Clear();
        }
    }
}