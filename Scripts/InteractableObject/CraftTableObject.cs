using ItemSystem.Produce;
using Manager;
using UI;

namespace InteractableObject
{
    public class CraftTableObject : ProducerObject
    {
        protected override void Start()
        {
            base.Start();
            
            data.type = ProducerType.CraftingTable;
            data.level = DataManager.Stat.ProducerLevel[data.type];
        }

        protected override ProducerType GetProducerType() => ProducerType.CraftingTable;
        protected override UIType GetUItype() => UIType.CraftTableUI;
    }
}