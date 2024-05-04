using System;
using System.Collections.Generic;
using DataSystem.FileIO;
using Settings.Scene;
using UnityEngine;

namespace DataSystem.Database
{
    public partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, MapData> Maps = new();
        
        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods
        
        public static MapData GetMapData(int index)
        {
            if (Maps.TryGetValue(index, out var data))
            {
                return data;
            }
            
            Debug.LogWarning($"[Database] GetSceneData(): Scene dose not exist. index:[{index}]");
            return null;
        }

        public static MapData GetMapData(string sceneName)
        {
            foreach (var pair in Maps)
            {
                if (pair.Value.scenePath == sceneName)
                {
                    return pair.Value;
                }
            }
            
            Debug.LogWarning($"[Database] GetSceneData(): Scene dose not exist. scene name:[{sceneName}]");
            return null;
        }

        #endregion
        
        
        // <Data Load Section>
        // * This methods load data from csv file
        #region Load Methods
        
        private static void LoadMapData()
        {
            var fileName = "Map_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadMapData(): CSV Read Failed");
                return;
            }

            try
            {
                Maps.Clear();

                foreach (var col in table)
                {
                    var id = int.Parse(col[0]);
                    var scene = new MapData(
                        id,
                        col[1],
                        col[2],
                        col[3]);
                    
                    Maps[id] = scene;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadMapData(): Language Data conversion failed: " + e);
            }
        }
        
        #endregion
    }
}