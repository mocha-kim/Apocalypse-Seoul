using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataSystem.FileIO;
using ItemSystem.Produce;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, Producer> ProducerData = new ();
        
        //group DB.
        [Description("ProducerLevelData[producerType][producerLevel]")]
        private static readonly Dictionary<ProducerType, Dictionary<int, ProducerAndRecipe>> ProducerLevelData = new();
        
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

                    ProducerData[id] = data;

                    if (producerType == ProducerType.Field)
                    {
                        for (var i = 6; i < col.Count; i++)
                        {
                            if (col[i].Length == 0)
                            {
                                continue;
                            }

                            data.recipeIDs.Add(int.Parse(col[i]));
                        }
                        continue;
                    }

                    var recipe = new Recipe(id, true);
                    for (var i = 6; i < col.Count; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }

                        recipe.AddMaterial(int.Parse(col[i]), int.Parse(col[i + 1]));
                    }
                    
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

        private static void InitProducerData()
        {
            foreach (var recipe in ItemRecipeData.Values)
            {
                if (recipe.producerType == ProducerType.Field)
                {
                    continue;
                }

                var producer = GetProducer(recipe.producerType, recipe.producerLevel);
                if (producer == null)
                {
                    continue;
                }
                producer.recipeIDs.Add(recipe.id);
            }

            ItemRecipeListById.Clear();
            foreach (var producer in ProducerData.Values)
            {
                ItemRecipeListById.Add(producer.id, new List<ItemRecipe>());
                var list = ItemRecipeListById[producer.id];
                list.AddRange(producer.recipeIDs.Select(GetItemRecipe));
            }
        }

        #endregion
    }
}