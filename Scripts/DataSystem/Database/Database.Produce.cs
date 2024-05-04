using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataSystem.FileIO;
using ItemSystem.Produce;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, Producer> ProducerData = new ();
        
        //group DB.
        [Description("ProducerLevelData[producerType][producerLevel]")]
        private static readonly Dictionary<ProducerType, Dictionary<int, ProducerAndRecipe>> ProducerLevelData = new();
        private static readonly Dictionary<ProducerType, List<ItemRecipe>> ItemRecipeData = new();
        
        [Serializable]
        private class ProducerAndRecipe
        {
            public Producer producer;
            public Recipe recipe;

            public ProducerAndRecipe(Producer producer, Recipe recipe)
            {
                this.producer = producer;
                this.recipe = recipe;
            }
        }
        
        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static ItemRecipe GetItemRecipe(ProducerType type, int recipeId)
        {
            if (ItemRecipeData.TryGetValue(type, out var recipeList))
            {
                foreach (var recipe in recipeList)
                {
                    if (recipe.id == recipeId)
                    {
                        return recipe;
                    }
                }
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
        
        public static Producer GetProducer(ProducerType type, int level)
        {
            if (ProducerLevelData.TryGetValue(type, out var dictionary))
            {
                if (dictionary.TryGetValue(level, out var data))
                {
                    return data.producer;
                }
            }

            Debug.LogWarning($"[Database] GetProducer(): Recipe dose not exist. type:[{type}], level[{level}]");
            return null;
        }
        
        public static Producer GetProducer(int id)
        {
            if (ProducerData.TryGetValue(id, out var producer))
            {
                return producer;
            }

            Debug.LogWarning($"[Database] GetProducer(): Recipe dose not exist. id:[{id}]");
            return null;
        }

        public static List<ItemRecipe> GetItemRecipeList(ProducerType type)
        {
            if (ItemRecipeData.TryGetValue(type, out var recipeList))
            {
                return recipeList;
            }

            Debug.LogWarning($"[Database] GetRecipeList(): Producer dose not exist. type:[{type}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods
        
        private static void LoadProducerData()
        {
            var fileName = "Producer_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadProducerData(): CSV Read Failed");
                return;
            }

            try
            {
                ProducerData.Clear();
                ProducerLevelData.Clear();
                
                foreach (var col in table)
                {
                    var id = int.Parse(col[0]);
                    var producerType = (ProducerType)Enum.Parse(typeof(ProducerType), col[1]);
                    var data = new Producer(
                        id,
                        producerType
                        , col[2]
                        , col[3]
                        ,col[4]
                        , int.Parse(col[5])
                    );
                    var recipe = new Recipe(id);
                    for (var i = 6; i < col.Count; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }

                        recipe.AddMaterial(int.Parse(col[i]), int.Parse(col[i + 1]));
                    }

                    ProducerData[id] = data;
                    
                    if (!ProducerLevelData.ContainsKey(producerType))
                    {
                        ProducerLevelData.Add(producerType, new Dictionary<int, ProducerAndRecipe>());
                    }
                    ProducerLevelData[producerType][data.level] = new ProducerAndRecipe(data, recipe);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadProducerData(): Product Data conversion failed: " + e);
            }
        }

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
                
                foreach (var col in table)
                {
                    var producerType = (ProducerType)Enum.Parse(typeof(ProducerType), col[1]);
                    var data = new ItemRecipe(
                        int.Parse(col[0]),
                        producerType
                        , int.Parse(col[2])
                        , int.Parse(col[3])
                        ,int.Parse(col[4])
                    );
                    for (var i = 5; i < col.Count; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }

                        data.AddMaterial(int.Parse(col[i]), int.Parse(col[i + 1]));
                    }
                    
                    if (ItemRecipeData.ContainsKey(producerType) == false)
                    {
                        ItemRecipeData.Add(producerType, new List<ItemRecipe>());
                    }
                    ItemRecipeData[producerType].Add(data);
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