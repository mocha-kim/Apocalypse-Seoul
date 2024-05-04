using System;
using System.Collections.Generic;
using AudioSystem;
using DataSystem.FileIO;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, ItemSystem.Item.Item> NormalItems = new();
        private static readonly Dictionary<int, ItemSystem.Item.ConsumeItem> ConsumeItems = new();

        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods
        
        public static ItemSystem.Item.Item GetItem(int index)
        {
            if (NormalItems.TryGetValue(index, out var normal))
            {
                return normal;
            }
            if (ConsumeItems.TryGetValue(index, out var consume))
            {
                return consume;
            }
            
            Debug.LogWarning($"[Database] GetItem() : Item dose not exist. index:[{index}]");
            return null;
        }

        public static ItemSystem.Item.Item GetNormalItem(int index)
        {
            if (NormalItems.TryGetValue(index, out var item))
            {
                return item;
            }

            Debug.LogWarning($"[Database] GetNormalItem() : Item dose not exist. index:[{index}]");
            return null;
        }

        public static ItemSystem.Item.ConsumeItem GetConsumeItem(int index)
        {
            if (ConsumeItems.TryGetValue(index, out var item))
            {
                return item;
            }

            Debug.LogWarning($"[Database] GetConsumeItem(): Item dose not exist. index:[{index}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region Load Methods
        
        private static void LoadNormalItem()
        {
            var fileName = "Item_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadNormalItem(): CSV Read Failed");
                return;
            }
            
            try
            {
                NormalItems.Clear();
                foreach (var col in table)
                {
                    var index = Convert.ToInt32(col[0]);
                    if (NormalItems.ContainsKey(index) == false)
                    {
                        var item = new ItemSystem.Item.Item(
                            index
                            , col[1]
                            , col[2]
                            , col[3]
                            , Convert.ToInt32(col[4])
                            , Convert.ToInt32(col[5])
                        );
                        NormalItems.Add(index, item);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadNormalItem(): Item Object conversion failed: " + e);
            }
        }

        private static void LoadConsumeItem()
        {
            var fileName = "Consume_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadConsumeItem(): CSV Read Failed");
                return;
            }
            
            try
            {
                ConsumeItems.Clear();
                foreach (var col in table)
                {
                    var index = int.Parse(col[0]);
                    if (ConsumeItems.ContainsKey(index))
                    {
                        continue;
                    }
                    var item = new ItemSystem.Item.ConsumeItem(
                        index
                        , col[1]
                        , col[2]
                        , col[3]
                        , int.Parse(col[4])
                        , int.Parse(col[5])
                        , int.Parse(col[6])
                        , int.Parse(col[12])
                        , Enum.Parse<SFXType>(col[13])
                    );
                    ConsumeItems.Add(index, item);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadConsumeItem(): Item Object conversion failed: " + e);
            }
        }

        #endregion
    }
}