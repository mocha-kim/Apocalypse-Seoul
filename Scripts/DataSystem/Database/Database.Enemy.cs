using System;
using System.Collections.Generic;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Enemy;
using DataSystem.FileIO;
using UnityEngine;

namespace DataSystem.Database
{
    public static partial class Database
    {
        //default DB.
        private static readonly Dictionary<int, Enemy> EnemyData = new ();
        
        // <Getter Section>
        // * This methods returns data or copy data
        #region GetMethods

        public static Enemy GetEnemy(int index)
        {
            if (EnemyData.TryGetValue(index, out var data))
            {
                return data;
            }
            
            Debug.LogWarning($"[Database] GetEnemy(): Enemy data dose not exist. index:[{index}]");
            return null;
        }

        #endregion

        // <Data Load Section>
        // * This methods load data from csv file
        #region LoadMethods

        private static void LoadEnemyData()
        {
            var fileName = "Enemy_table";
            
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadEnemy(): CSV Read Failed");
                return;
            }

            try
            {
                EnemyData.Clear();
                
                foreach (var col in table)
                {
                    var id = int.Parse(col[0]);
                    var data = new Enemy(
                        id,
                        col[1],
                        (AttackType)int.Parse(col[2]),
                        col[3],
                        int.Parse(col[4]),
                        int.Parse(col[5]),
                        int.Parse(col[6]),
                        int.Parse(col[7]),
                        int.Parse(col[8]),
                        int.Parse(col[9])
                    );

                    EnemyData[id] = data;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadNPC(): NPC Data conversion failed: " + e);
            }
        }

        #endregion
    }
}