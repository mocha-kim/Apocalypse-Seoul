using System;
using DataSystem.Database;
using Event;
using Manager;

namespace ItemSystem.Produce
{
    [Serializable]
    public class Producer
    {
        public int id;
        public int level;
        public ProducerType type;
        
        public string name;
        public string description;
        public string spritePath;

        public Producer(int id, ProducerType type, string name, string description, string spritePath, int level)
        {
            this.id = id;
            this.level = level;
            this.type = type;

            this.name = name;
            this.description = description;
            this.spritePath = spritePath;
        }

        public void Produce(int recipeId)
        {
            var recipe = Database.GetItemRecipe(type, recipeId);
            if (recipe == null || recipe.producerLevel > level || !recipe.IsProducible())
            {
                return;
            }
            
            foreach (var material in recipe.materials)
            {
                DataManager.Storage.UseItem(material.id, material.billAmount);
            }
            DataManager.Storage.AddItem(recipe.productId, recipe.productAmount);
        }

        public void Upgrade()
        {
            var recipe = Database.GetUpgradeRecipe(id);
            if (recipe == null || !recipe.IsProducible())
            {
                return;
            }
            
            foreach (var material in recipe.materials)
            {
                DataManager.Storage.UseItem(material.id, material.billAmount);
            }
            level++;
            DataManager.Stat.ProducerLevel[type]++;
            
            EventManager.OnNext(Message.OnProducerUpgraded, type);
        }

        public override string ToString()
        {
            return "Producer: id(" + id + "type(" + type + "), name(" + name + "), level(" + level +
                   "), description(" + description + ") sprite path(" + spritePath + ")";
        }
    }
}