using System;
using System.Collections.Generic;
using DataSystem.FileIO;
using ItemSystem.Produce;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, ItemRecipe> ItemRecipeData = new();
        
        //group DB.
        private static readonly Dictionary<ProducerType, List<ItemRecipe>> ItemRecipeListByType = new();
        private static readonly Dictionary<int, List<ItemRecipe>> ItemRecipeListById = new();
        
        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static ItemRecipe GetItemRecipe(int recipeId)
        {
            if (ItemRecipeData.TryGetValue(recipeId, out var recipe))
            {
                return recipe;
            }

            Debug.LogWarning($"[Database] GetItemRecipe(): Recipe dose not exist. id:[{recipeId}]");
            return null;
        }

        public static Recipe GetUpgradeRecipe(ProducerType type, int level)
        {
            if (ProducerLevelData.TryGetValue(type, out var dictionary))
            {
                if (dictionary.TryGetValue(level, out var data))
                {
                    return data.recipe;
                }
            }

            Debug.LogWarning($"[Database] GetUpgradeRecipe(): Recipe dose not exist. type:[{type}], level[{level}]");
            return null;
        }
        
        public static Recipe GetUpgradeRecipe(int producerId)
        {
            var producer = GetProducer(producerId);
            if (producer == null || producer.type == ProducerType.Field)
            {
                Debug.LogWarning($"[Database] GetUpgradeRecipe(): Invalid producer. id:[{producerId}]");
                return null;
            }
            
            if (ProducerLevelData.TryGetValue(producer.type, out var dictionary))
            {
                if (dictionary.TryGetValue(producer.level, out var data))
                {
                    return data.recipe;
                }
            }

            Debug.LogWarning($"[Database] GetUpgradeRecipe(): Recipe dose not exist. id:[{producerId}]");
            return null;
        }

        public static List<ItemRecipe> GetItemRecipeList(ProducerType type)
        {
            if (ItemRecipeListByType.TryGetValue(type, out var recipeList))
            {
                return recipeList;
            }

            Debug.LogWarning($"[Database] GetRecipeList(): Producer dose not exist. type:[{type}]");
            return null;
        }
        
        public static List<ItemRecipe> GetItemRecipeList(int producerId)
        {
            if (ItemRecipeListById.TryGetValue(producerId, out var recipeList))
            {
                return recipeList;
            }

            Debug.LogWarning($"[Database] GetRecipeList(): Producer dose not exist. producerId:[{producerId}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods

        private static void LoadItemRecipeData()
        {
            var fileName = "Recipe_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadItemRecipeData(): CSV Read Failed");
                return;
            }

            try
            {
                ItemRecipeData.Clear();
                ItemRecipeListByType.Clear();
                
                foreach (var col in table)
                {
                    var producerType = (ProducerType)Enum.Parse(typeof(ProducerType), col[2]);
                    var producerLevel = int.Parse(col[3]);
                    var data = new ItemRecipe(
                        int.Parse(col[0])
                        , bool.Parse(col[1])
                        , producerType
                        , producerLevel
                        , int.Parse(col[4])
                        ,int.Parse(col[5])
                    );
                    for (var i = 6; i < col.Count; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }

                        data.AddMaterial(int.Parse(col[i]), int.Parse(col[i + 1]));
                    }
                    
                    ItemRecipeData[data.id] = data;
                    
                    if (!ItemRecipeListByType.ContainsKey(data.producerType))
                    {
                        ItemRecipeListByType.Add(data.producerType, new List<ItemRecipe>());
                    }
                    ItemRecipeListByType[data.producerType].Add(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadItemRecipeData(): Product Data conversion failed: " + e);
            }
        }

        #endregion
    }
}