namespace ItemSystem.ItemBox
{
    public class SpawnData
    {
        public int internalId;
        public int itemId;
        public float spawnRate;
            
        public SpawnData(int internalId, int itemId, float spawnRate)
        {
            this.internalId = internalId;
            this.itemId = itemId;
            this.spawnRate = spawnRate;
        }
    }
}