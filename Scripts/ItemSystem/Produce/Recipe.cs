using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemSystem.Produce
{
    [Serializable]
    public class Recipe
    {
        public int id;
        public bool isActive = false;
        public List<Material> materials = new();

        public Recipe(int id, bool isActive)
        {
            this.id = id;
            this.isActive = isActive;
        }
        
        public virtual bool IsProducible()
        {
            if (!isActive)
            {
                return false;
            }
            
            var hasEnoughItem = true;
            foreach (var material in materials)
            {
                hasEnoughItem = hasEnoughItem && material.IsEnoughInStorage();
            }
            return hasEnoughItem;
        }
        
        public void AddMaterial(int materialId, int amount)
        {
            foreach (var mat in materials.Where(mat => mat.id == materialId))
            {
                mat.billAmount += amount;
                return;
            }
            materials.Add(new Material(materialId, amount));
        }

        public override string ToString()
        {
            var str = $"Recipe: id({id}), isActive({isActive})";
            foreach (var material in materials)
            {
                str += "\n> " + material;
            }
            return str;
        }
    }
}