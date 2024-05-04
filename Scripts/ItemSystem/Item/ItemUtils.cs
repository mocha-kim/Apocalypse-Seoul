using Alpha;
using Core;
using DataSystem;
using Random = UnityEngine.Random;

namespace ItemSystem.Item
{
    public static class ItemUtils
    {
        public static int GetCorrectedBoxValue(int value)
            => (int)(Random.Range(Constants.Item.MinBoxValueCorrector, Constants.Item.MaxBoxValueCorrector) * value);
    }
}