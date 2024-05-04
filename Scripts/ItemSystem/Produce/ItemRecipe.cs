using System;
using DataSystem.Database;
using Manager;

namespace ItemSystem.Produce
{
    [Serializable]
    public class ItemRecipe : Recipe
    {
        public ProducerType producerType;
        public int producerLevel;
        
        public int productId;
        public int productAmount;
        public string productName;

        private Item.Item _product;

        public ItemRecipe(int id, ProducerType producerType, int producerLevel, int productId, int productAmount)
            : base(id)
        {
            this.producerType = producerType;
            this.producerLevel = producerLevel;
            
            this.productId = productId;
            this.productAmount = productAmount;
            _product = Database.GetItem(productId);
            productName = _product?.name ?? "";
        }
        
        public override bool IsProducible()
        {
            var hasEnoughItem = true;  
            var requiredSlotCount = productAmount / _product.maxStackCount
                                    + (productAmount % _product.maxStackCount > 0 ? 1 : 0);
            var toBeEmptiedSlotCount = 0;
            foreach (var material in materials)
            {
                hasEnoughItem = hasEnoughItem && material.IsEnoughInStorage();
                
                var materialItem = Database.GetItem(material.id);
                toBeEmptiedSlotCount += material.billAmount / materialItem.maxStackCount;
            }
            var hasEnoughSpace = DataManager.Storage.GetEmptySlotCount() + toBeEmptiedSlotCount > requiredSlotCount;
            return hasEnoughItem && hasEnoughSpace;
        }

        public override string ToString()
        {
            var str = "Item Recipe: id(" + id + ")\n";
            str += "Producer: type(" + producerType + "), level(" + producerLevel + ")\n";
            str += "Product: id" + productId + "), name(" + productName + "), amount(" + productAmount + ")\n";
            foreach (var material in materials)
            {
                str += "Material: id(" + material.id + "), bill amount(" + material.billAmount + ")\n";
            }

            return str;
        }
    }
}