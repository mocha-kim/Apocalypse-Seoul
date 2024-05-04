using System;
using DataSystem.Database;
using ItemSystem.Item;
using Manager;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    public class DoorObject : InteractableObject
    {
        [SerializeField] private int _requiredItemId = -1;
        private Item _requiredItem;
        private int _curItemAmount;
        
        [SerializeField] private GameObject _render;
        [SerializeField] private Collider2D _collider;

        private void Start()
        {
            if (_requiredItemId == -1)
            {
                return;
            }
            
            _requiredItem = Database.GetItem(_requiredItemId);
            if (_requiredItem == null)
            {
                gameObject.SetActive(false);
            }
        }

        public override void Interact()
        {
            if (_requiredItemId == -1)
            {
                OpenDoor();
                return;
            }

            if (!IsUnlockable())
            {
                NotifyInteractCondition();
                return;
            }
            
            OpenDoor();
            DataManager.PlayerInventory.RemoveItem(_requiredItemId, 1);
        }

        protected override void SetConditionData(out string iconPath, out string text)
        {
            iconPath = _requiredItem.iconPath;
            text = _curItemAmount + "/1";
        }

        private void OpenDoor()
        {
            gameObject.SetActive(false);
        }

        private bool IsUnlockable()
        {
            if (_requiredItemId == -1)
            {
                return true;
            }
            
            _curItemAmount = DataManager.PlayerInventory.GetTotalAmount(_requiredItemId);
            return _curItemAmount > 0;
        }
    }
}