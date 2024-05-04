using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemSystem.Produce
{
    [Serializable]
    public class Recipe
    {
        public int id;
        public List<Material> materials = new();

        public Recipe(int id)
        {
            this.id = id;
        }
        
        public virtual bool IsProducible()
        {
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
    }
}