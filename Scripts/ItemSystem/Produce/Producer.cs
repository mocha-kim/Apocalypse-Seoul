using System;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using DataSystem.Database;
using EventSystem;
using UnityEngine;
using UnityEngine.Serialization;

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

        public List<int> recipeIDs = new();

        public Producer(int id, ProducerType type, string name, string description, string spritePath, int level)
        {
            this.id = id;
            this.level = level;
            this.type = type;

            this.name = name;
            this.description = description;
            this.spritePath = spritePath;
        }

        public virtual void Produce(int recipeId)
        {
            var i = recipeIDs.FirstOrDefault(value => value == recipeId);
            if (i == 0)
            {
                return;
            }

            var recipe = Database.GetItemRecipe(i);
            if (recipe == null || !recipe.isActive || !recipe.IsProducible())
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
            var recipe = Database.GetUpgradeRecipe(type, level);
            Debug.Log(recipe);
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
            var str = $"Producer: id({id}), type({type}), name({name}), level({level}), description({description}) sprite path({spritePath})";
            if (recipeIDs.Count > 0)
            {
                str += "\n> recipe Ids(";
                foreach (var i in recipeIDs)
                {
                    str += i + " ";
                }
                str += ")";
            }
            return str;
        }
    }
}