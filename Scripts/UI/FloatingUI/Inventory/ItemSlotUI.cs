using Event;
using ItemSystem.Inventory;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.FloatingUI.Inventory
{
    public class ItemSlotUI : MouseTriggerUI
    {
        protected ItemSlot _slotData;
        private UIBase _contextInventoryUI;

        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _slotImage;
        
        [SerializeField] private Sprite _activatedSlot;
        [SerializeField] private Sprite _deactivatedSlot;
        
        private Vector3 _defaultScale = new(1f, 1f, 1f);
        private Vector2 _defaultSizeDelta = new Vector3(32, 32);

        public override UIType GetUIType() => UIType.ComponentUI;
        protected override bool HasEnterExitTrigger => true;

        public override void Init()
        {
            base.Init();

            _activatedSlot ??= ResourceManager.GetSprite("UI_Inventory_Slot");
            _deactivatedSlot ??= ResourceManager.GetSprite("UI_Inventory_Slot_Beclosed");
            
            EventManager.AddEventTrigger(gameObject, EventTriggerType.BeginDrag, _ => OnBeginDragItem());
            EventManager.AddEventTrigger(gameObject, EventTriggerType.Drag, _ => OnDragItem());
            EventManager.AddEventTrigger(gameObject, EventTriggerType.EndDrag, _ => OnEndDragItem());
        }

        public void SetContext(UIBase inventoryUI)
        {
            _contextInventoryUI = inventoryUI;
        }

        public void LinkSlotData(ItemSlot slotData)
        {
            _slotData = slotData;
        }

        public void UpdateSlot()
        {
            if (_slotData == null)
            {
                return;
            }
            _slotImage.sprite = _slotData.IsActivated ? _activatedSlot : _deactivatedSlot;
            
            if (_slotData.IsInValid)
            {
                _amountText.text = "";
                _iconImage.sprite = null;
                _iconImage.gameObject.SetActive(false);
                return;
            }
            _amountText.text = _slotData.Amount.ToString();
            _iconImage.sprite = ResourceManager.GetSprite(_slotData.Item.iconPath);
            _iconImage.gameObject.SetActive(true);
        }
        
        protected override void OnMouseEnter()
        {
            MouseData.MouseHoveredSlot = _slotData;
        }

        protected override void OnMouseExit()
        {
            MouseData.MouseHoveredSlot = null;
        }
        
        protected override void OnMouseClick()
        {
            MouseData.FocusedUI = _contextInventoryUI;
            if (_slotData == null || _slotData.IsInValid)
            {
                return;
            }

            if (_slotData.ParentType == InventoryType.QuickSlot ||
                UIManager.Instance.ActiveInventories.Count != 2 ||
                !InputManager.Instance.IsFunctionDown)
            {
                return;
            }

            var movedAmount = -1;
            if (UIManager.Instance.ActiveInventories[0].GetInventory() == MouseData.MouseHoveredInventory)
            {
                movedAmount = UIManager.Instance.ActiveInventories[1].GetInventory()
                    .AddItem(_slotData.Item, _slotData.Amount);
            }
            else if (UIManager.Instance.ActiveInventories[1].GetInventory() == MouseData.MouseHoveredInventory)
            {
                movedAmount = UIManager.Instance.ActiveInventories[0].GetInventory()
                    .AddItem(_slotData.Item, _slotData.Amount);
            }

            _slotData.RemoveItem(movedAmount);
            EventManager.OnNext(Message.OnUpdateInventory, _slotData.ParentType);
        }
        
        private void OnBeginDragItem()
        {
            if (_slotData == null || _slotData.IsInValid)
            {
                return;
            }
            MouseData.DragBeginSlot = _slotData;
            
            var dragImageObject = new GameObject();
            MouseData.DraggingItem = dragImageObject;

            MouseData.DraggingItemTransform = dragImageObject.AddComponent<RectTransform>();
            dragImageObject.transform.SetParent(transform.parent);
            dragImageObject.transform.localScale = _defaultScale;
            MouseData.DraggingItemTransform.sizeDelta = _defaultSizeDelta;
            
            var image = dragImageObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.sprite = ResourceManager.GetSprite(_slotData.Item.iconPath);
            
            dragImageObject.name = "Drag Image";
        }
        
        private void OnDragItem()
        {
            if (MouseData.DraggingItem == null)
            {
                return;
            }

            MouseData.DraggingItem.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        protected virtual void OnEndDragItem()
        {
            Destroy(MouseData.DraggingItem);

            var slot = MouseData.DragBeginSlot;
            if (slot == null || _slotData.IsInValid)
            {
                return;
            }
            MouseData.DragBeginSlot = null;
            
            if (MouseData.MouseHoveredSlot != null)
            {
                if (!MouseData.MouseHoveredSlot.IsActivated)
                {
                    return;
                }
                
                if (MouseData.MouseHoveredSlot.ParentType == InventoryType.QuickSlot)
                {
                    if (slot.ParentType == InventoryType.Player)
                    {
                        var quickSlot = MouseData.MouseHoveredSlot as QuickSlot;
                        quickSlot.AllocateItem(slot.Item, -1);
                        quickSlot.SyncItemAmount();
                    }
                }
                else
                {
                    MouseData.MouseHoveredSlot.SwapItem(slot);
                }
                EventManager.OnNext(Message.OnUpdateInventory, MouseData.MouseHoveredSlot.ParentType);
            }
            // TODO: Disable until Design confirmed
            // else if (MouseData.MouseHoveredInventory == null)
            // {
            //     slot.ClearItem();
            // }
            EventManager.OnNext(Message.OnUpdateInventory, slot.ParentType);
        }
    }
}