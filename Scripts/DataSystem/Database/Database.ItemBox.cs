using System;
using System.Collections.Generic;
using DataSystem.FileIO;
using ItemSystem.ItemBox;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //group DB.
        private static readonly Dictionary<int, ItemBox> ItemBoxData = new();
        private static readonly Dictionary<int, List<SpawnData>> ItemBoxSpawnData = new();

        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static ItemBox CreateItemBox(int id)
        {
            if (ItemBoxData.TryGetValue(id, out var data))
            {
                return new ItemBox(data);
            }
            
            Debug.LogWarning($"[Database] GetConsumeItem(): ItemBox dose not exist. index:[{id}]");
            return null;
        }
        
        public static List<SpawnData> GetSpawnDataList(int id)
        {
            if (ItemBoxSpawnData.TryGetValue(id, out var data))
            {
                return data;
            }

            Debug.LogWarning($"[Database] GetSpawnData(): ItemBox dose not exist. Index:[{id}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods
        
        private static void LoadSpawnData()
        {
            var fileName = "Item_Box_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadSpawnData(): CSV Read Failed");
                return;
            }

            try
            {
                ItemBoxSpawnData.Clear();

                foreach (var col in table)
                {
                    var id = Convert.ToInt32(col[0]);
                    var internalId = Convert.ToInt32(col[1]);
                    if (internalId == 0)
                    {
                        // create list to add another item box data
                        if (!ItemBoxData.ContainsKey(id))
                        {
                            ItemBoxData[id] = new ItemBox(
                                id,
                                int.Parse(col[2]),
                                int.Parse(col[3]),
                                col[4]);
                        }
                        if (!ItemBoxSpawnData.ContainsKey(id))
                        {
                            ItemBoxSpawnData[id] = new List<SpawnData>();
                        }
                    }
                    
                    var data = new SpawnData(
                        internalId,
                        int.Parse(col[5]),
                        float.Parse(col[6])
                    );
                    ItemBoxSpawnData[id].Add(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadSpawnData(): Spawn Data conversion failed: " + e);
            }
        }

        #endregion
    }
}