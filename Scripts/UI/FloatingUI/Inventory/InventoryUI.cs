using System.Collections.Generic;
using System.Linq;
using DataSystem;
using EventSystem;
using InputSystem;
using UI.FixedUI.EventUI.Deal;
using UnityEngine;

namespace UI.FloatingUI.Inventory
{
    public abstract class InventoryUI<T> : MouseTriggerUI where T : ItemSlotUI
    {
        [SerializeField] protected GameObject SlotParent;
        [SerializeField] protected Vector2 Space;
        [SerializeField] protected int ColCount;
        protected List<T> SlotUIs;
        
        public abstract ItemSystem.Inventory.Inventory GetInventory();
        protected override bool HasEnterExitTrigger => true;

        public override void Init()
        {
            base.Init();

            // add events
            EventManager.Subscribe(gameObject, Message.OnUpdateInventory, e => OnUpdateInventory(e));

            //slot generate.
            var slotPrefab = typeof(T) == typeof(DealItemSlotUI)
                ? ResourceManager.GetPrefab("DealSlotUI")
                : ResourceManager.GetPrefab("SlotUI");
            var sizeDelta = slotPrefab.GetComponent<RectTransform>().sizeDelta;

            if (GetInventory() == null)
            {
                Debug.LogError("Cannot find inventory data in " + gameObject.name);
                gameObject.SetActive(false);
                return;
            }

            for (int i = 0; i < GetInventory().Size; i++)
            {
                GameObject newSlot = Instantiate(slotPrefab, SlotParent.transform);
                newSlot.name += " " + i;

                float x = (Space.x + sizeDelta.x) * (i % ColCount);
                float y = -(Space.y + sizeDelta.y) * (i / ColCount);
                newSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                
                var slotUI = newSlot.GetComponent<T>();
                slotUI.Init();
                slotUI.SetContext(this);
            }

            SlotUIs = SlotParent.GetComponentsInChildren<T>().ToList();
        }

        public override void Open()
        {
            LinkSlots();
            base.Open();
        }

        protected void LinkSlots()
        {
            var inventory = GetInventory();
            for (int i = 0; i < inventory.Size; i++)
            {
                SlotUIs[i].LinkSlotData(inventory.slots[i]);
                SlotUIs[i].UpdateSlot();
            }
        }

        protected abstract void OnUpdateInventory(EventManager.Event e);

        protected override void OnMouseEnter()
        {
            MouseData.MouseHoveredInventory = GetInventory();
        }
        
        protected override void OnMouseExit()
        {
            MouseData.MouseHoveredInventory = null;
        }
    }
}