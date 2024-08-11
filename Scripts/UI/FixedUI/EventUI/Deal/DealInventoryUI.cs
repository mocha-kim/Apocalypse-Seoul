using EventSystem;
using UI.FloatingUI.Inventory;
using UnityEngine;

namespace UI.FixedUI.EventUI.Deal
{
    public abstract class DealInventoryUI : InventoryUI<DealItemSlotUI>
    {
        protected abstract ItemSystem.Inventory.Inventory PairInventory { get; }

        private void Awake()
        {
            EventManager.Subscribe(gameObject, Message.OnDealUIInit, _ => Init());
            EventManager.Subscribe(gameObject, Message.OnDealUILink, _ => LinkSlots());
            EventManager.Subscribe(gameObject, Message.OnDoDeal, _ => DoDeal());
            EventManager.Subscribe(gameObject, Message.OnResetDeal, _ => ResetDeal());
        }

        public override void Init()
        {
            if (!IsInit)
            {
                base.Init();
            }
            
            foreach (var slotUI in SlotUIs)
            {
                slotUI.SetPairInventory(PairInventory);
            }
        }

        public override void Open()
        {
            // Cannot be opened independently, Do nothing
        }

        public override void Close()
        {
            // Cannot be closed independently, Do nothing
        }

        public virtual void ResetDeal()
        {
            
        }

        public virtual void DoDeal()
        {

        }
    }
}