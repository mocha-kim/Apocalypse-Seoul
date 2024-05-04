using ItemSystem.Inventory;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using UserActionBind;

namespace DataSystem.SaveLoad
{
    public class SaveLoadController : MonoBehaviour
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        private void Awake()
        {
            _saveButton?.onClick.AddListener(Save);
            _loadButton?.onClick.AddListener(Load);
        }

        public void Save()
        {
            SaveSystem.SaveData(DataManager.PlayerInventory, "PlayerInventory");
            SaveSystem.SaveData(DataManager.Storage, "Storage");
            SaveSystem.SaveData(DataManager.QuickSlots, "QuickSlots");

            SaveSystem.SaveData(DataManager.MerchantInventories, "Merchant");
            SaveSystem.SaveData(DataManager.SpawnedItemBoxData, "SpawnedItemBoxData");

            SaveSystem.SaveData(DataManager.Stat, "Stat");

            SaveSystem.SaveData(DataManager.CurrentMap, "CurrentMap");

            SaveSystem.SaveData(InputBinding.Bindings, "InputBinding");
        }

        public void Load()
        {
            DataManager.PlayerInventory = SaveSystem.LoadData("PlayerInventory", DataManager.PlayerInventory);
            DataManager.Storage = SaveSystem.LoadData("Storage", DataManager.Storage);
            DataManager.QuickSlots = SaveSystem.LoadData("QuickSlots", DataManager.QuickSlots);

            DataManager.MerchantInventories = SaveSystem.LoadData("Merchant", DataManager.MerchantInventories);
            DataManager.SpawnedItemBoxData = SaveSystem.LoadData("SpawnedItemBoxData", DataManager.SpawnedItemBoxData);

            DataManager.Stat = SaveSystem.LoadData("Stat", DataManager.Stat);

            DataManager.CurrentMap = SaveSystem.LoadData("CurrentMap", DataManager.CurrentMap);

            InputBinding.Bindings = SaveSystem.LoadData("InputBinding", InputBinding.Bindings);
        }
    }
}