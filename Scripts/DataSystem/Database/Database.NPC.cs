using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CharacterSystem.Character;
using DataSystem.FileIO;
using DialogSystem;
using ItemSystem.Inventory;
using Manager;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, NPC> NPCData = new ();
        
        //group DB.
        [Description("ScriptData[npcId][scriptId]")]
        private static readonly Dictionary<int, Dictionary<int, ScriptList>> ScriptData = new();
        
        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static NPC GetNPC(int index)
        {
            if (NPCData.TryGetValue(index, out var data))
            {
                return data;
            }
            
            Debug.LogWarning($"[Database] GetNPC(): NPC data dose not exist. index:[{index}]");
            return null;
        }

        public static ScriptList GetScriptList(int npcIndex, int step)
        {
            if (ScriptData.TryGetValue(npcIndex, out var dictionary))
            {
                if (dictionary.Values.Count > step)
                {
                    return dictionary.OrderBy(pair => pair.Key).ToArray()[step].Value;
                }
                return dictionary.OrderBy(pair => pair.Key).ToArray().Last().Value;
            }
            
            Debug.LogWarning($"[Database] GetScript(): Script data dose not exist. NPC index:[{npcIndex}], Step:[{step}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods

        private static void LoadNPC()
        {
            var fileName = "NPC_table";
            
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadNPC(): CSV Read Failed");
                return;
            }

            try
            {
                NPCData.Clear();
                
                foreach (var col in table)
                {
                    var id = int.Parse(col[0]);
                    var data = new NPC(
                        id,
                        (CharacterType)Enum.Parse(typeof(CharacterType), col[1]),
                        col[2],
                        col[3],
                        col[4],
                        int.Parse(col[5])
                    );

                    NPCData[id] = data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadNPC(): NPC Data conversion failed: " + e);
            }
            
            // Load 900000(Debug, Player) Init
            NPCData[GetFirstId(Constants.Database.NPCPrefix)].Init();
        }

        private static void LoadNPCInventories()
        {
            var fileName = "Merchant_Inventory_table";
            
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadNPCInventories(): CSV Read Failed");
                return;
            }

            try
            {
                var inventoryData = DataManager.MerchantInventories;
                inventoryData.Clear();

                foreach (var col in table)
                {
                    var id = int.Parse(col[0]);

                    if (!inventoryData.ContainsKey(id))
                    {
                        inventoryData.Add(id, new Inventory(Constants.Inventory.MaxInventorySize, InventoryType.Merchant));
                    }

                    inventoryData[id].AddItem(int.Parse(col[2]), int.Parse(col[3]));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadNPCInventories(): Script Data conversion failed: " + e);
            }
        }

        private static void LoadScripts()
        {
            var fileName = "Script_table";
            
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadScripts(): CSV Read Failed");
                return;
            }

            try
            {
                ScriptData.Clear();

                foreach (var col in table)
                {
                    var scriptId = int.Parse(col[0]);
                    var innerId = int.Parse(col[1]);
                    var scriptType = (ScriptType)Enum.Parse(typeof(ScriptType), col[2]);
                    var npcId = int.Parse(col[3]);
                    var data = scriptType switch
                    {
                        ScriptType.Dialog => new Script(
                            innerId,
                            scriptType,
                            int.Parse(col[7]),
                            int.Parse(col[4]),
                            bool.Parse(col[5]),
                            col[6]
                        ),
                        _ => new Script(innerId, scriptType, int.Parse(col[7]))
                    };

                    for (var i = 8; ; i += 2)
                    {
                        if (col[i].Length == 0)
                        {
                            break;
                        }
                        data.AddAnswer(col[i], int.Parse(col[i + 1]));
                    }

                    if (!ScriptData.ContainsKey(npcId))
                    {
                        ScriptData.Add(npcId, new Dictionary<int, ScriptList>());
                    }

                    if (!ScriptData[npcId].ContainsKey(scriptId))
                    {
                        ScriptData[npcId].Add(scriptId, new ScriptList(scriptId));
                    }

                    ScriptData[npcId][scriptId].AddScript(innerId, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadScripts(): Script Data conversion failed: " + e);
            }
        }

        #endregion
    }
}