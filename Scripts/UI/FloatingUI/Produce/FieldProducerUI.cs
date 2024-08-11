using System.Collections.Generic;
using DataSystem.Database;
using ItemSystem.Produce;

namespace UI.FloatingUI.Produce
{
    public class FieldProducerUI : ProducerUI
    {
        public override UIType GetUIType() => UIType.FieldProducerUI;
        protected override List<ItemRecipe> GetItemRecipeList() => Database.GetItemRecipeList(UIManager.OpenProducerId);
    }
}