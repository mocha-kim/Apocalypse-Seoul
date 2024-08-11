using ItemSystem.Inventory;
using UI;
using UnityEngine;

namespace InputSystem
{
    public static class MouseData
    {
        public static GameObject DraggingItem;
        public static RectTransform DraggingItemTransform;
        public static ItemSlot DragBeginSlot;
        
        public static ItemSlot MouseHoveredSlot;
        public static Inventory MouseHoveredInventory;

        private static UIBase _focusedUI;
        public static UIBase FocusedUI
        {
            get => _focusedUI;
            set
            {
                _focusedUI = value;
                _focusedUI.transform.SetAsLastSibling();
            }
        }
    }
}