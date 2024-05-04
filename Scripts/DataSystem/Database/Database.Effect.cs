using System;
using System.Collections.Generic;
using CharacterSystem.Effect;
using CharacterSystem.Stat;
using DataSystem.FileIO;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, Effect> EffectData = new();

        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static Effect GetEffect(int effectId)
        {
            if (EffectData.TryGetValue(effectId, out var recipeList))
            {
                return recipeList;
            }

            Debug.LogWarning($"[Database] GetEffect(): Effect dose not exist. type:[{effectId}]");
            return null;
        }
        
        public static Effect GetEffect(EffectType type)
        {
            if (type == EffectType.ItemEffect)
            {
                Debug.LogWarning($"[Database] GetEffect(): Unable to find specific effect by type:[ItemType].");
                return null;
            }
            
            foreach (var effect in EffectData.Values)
            {
                if (effect.Type == type)
                {
                    return effect;
                }
            }

            Debug.LogWarning($"[Database] GetEffect(): Effect dose not exist. type:[{type}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods
        
        private static void LoadEffectData()
        {
            var fileName = "Effect_table";
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadEffectData(): CSV Read Failed");
                return;
            }
            
            try
            {
                EffectData.Clear();
                foreach (var col in table)
                {
                    var index = Convert.ToInt32(col[0]);
                    if (EffectData.ContainsKey(index))
                    {
                        continue;
                    }
                    
                    int id = int.Parse(col[0]);
                    int classType = int.Parse(col[2]);  // 0:Instant, 1:Dot, 2:Maintain
                    Effect data;
                    switch (classType)
                    {
                        case 0:
                        {
                            data = new InstantEffect(
                                id,
                                (EffectType)Enum.Parse(typeof(EffectType), col[1])
                            );
                        }
                            break;
                        case 1:
                        {
                            data = new DotEffect(
                                id,
                                (EffectType)Enum.Parse(typeof(EffectType), col[1]),
                                int.Parse(col[3]),
                                int.Parse(col[4])
                            );
                        }
                            break;
                        case 2:
                        {
                            data = new MaintainEffect(
                                id,
                                (EffectType)Enum.Parse(typeof(EffectType), col[1]),
                                int.Parse(col[3]),
                                int.Parse(col[4])
                            );
                        }
                            break;
                        default:
                            continue;
                    }
                    
                    for (var i = 5; i < col.Count; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }

                        var isFloat = float.Parse(col[i + 1]) % 1 != 0;
                        if (isFloat)
                        {
                            data.AddEffect((AttributeType)int.Parse(col[i]), float.Parse(col[i + 1]));
                        }
                        else
                        {
                            data.AddEffect((AttributeType)int.Parse(col[i]),int.Parse(col[i + 1]));
                        }
                    }
                    EffectData.Add(id, data);
                }
            }
            catch (Exception e)
            {
                    Debug.LogError("[Database] LoadEffectData(): Effect Object conversion failed: " + e);
            }
        }

        #endregion
    }
}