using Manager;

namespace DataSystem.Database
{
    public static partial class Database
    {
        public static void Init()
        {
            LoadNormalItem();
            LoadConsumeItem();
            LoadSpawnData();
            
            LoadProducerData();
            LoadItemRecipeData();
            
            LoadEffectData();
            
            LoadNPC();
            LoadNPCInventories();
            LoadScripts();

            LoadMapData();
            LoadLanguageData();
        }

        public static int GetFirstId(int prefix) => prefix * Constants.Database.DatabaseIdRange;
    }
}