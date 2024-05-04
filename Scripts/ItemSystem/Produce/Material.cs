using Manager;

namespace ItemSystem.Produce
{
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
    }
}