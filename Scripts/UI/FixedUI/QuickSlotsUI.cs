using System;
using Alpha;
using Core;
using DataSystem;
using EventSystem;
using ItemSystem.Inventory;
using UnityEngine;

namespace UI.FixedUI
{
    public class QuickSlotsUI : MouseTriggerUI
    {
        [SerializeField] protected GameObject SlotParent;
        private QuickSlotUI[] _slotUIs;

        private void Start()
        {
            Init();
        }
        
        public override UIType GetUIType() => UIType.ComponentUI;
        protected override bool HasEnterExitTrigger => false;

        public override void Init()
        {
            base.Init();

            _slotUIs = SlotParent.GetComponentsInChildren<QuickSlotUI>();
            for (var i = 0; i < Constants.Inventory.QuickSlotSize; i++)
            {
                _slotUIs[i].Init();
                _slotUIs[i].LinkSlotData(DataManager.QuickSlots[i]);
                _slotUIs[i].SyncSlotItemAmount();
                _slotUIs[i].UpdateSlot();
            }
            EventManager.Subscribe(gameObject, Message.OnUpdateInventory, e => OnUpdateInventory(e));
        }

        private void OnUpdateInventory(EventManager.Event e)
        {
            InventoryType type;
            try
            {
                type = (InventoryType)e.Args[0];
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

            if (type is InventoryType.QuickSlot or InventoryType.Player)
            {
                foreach (var slotUI in _slotUIs)
                {
                    slotUI.SyncSlotItemAmount();
                    slotUI.UpdateSlot();
                }
            }
        }

        public override void Open()
        {
            // This UI is always on, Do nothing
        }

        public override void Close()
        {
            // This UI cannot closed, Do nothing
        }
    }
}