using System;
using DataSystem;

namespace ItemSystem.Produce
{
    [Serializable]
    public class Material
    {
        public int id;
        public int billAmount;

        public Material(int id, int amount)
        {
            this.id = id;
            billAmount = amount;
        }

        public bool IsEnoughInStorage()
        {
            var curAmount = DataManager.Storage.GetTotalAmount(id);
            return curAmount >= billAmount;
        }

        public override string ToString()
        {
            return $"Material: id({id}), bill amount({billAmount})";
        }
    }
}