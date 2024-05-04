using System;
using System.Collections.Generic;
using CharacterSystem.Stat;
using DataSystem.FileIO;
using ItemSystem.Item;
using ItemSystem.ItemBox;
using UnityEngine;

namespace DataSystem.Database
{
    public partial class Database
    {
        //default DB.
        private static readonly Dictionary<string, string> LanguagePack = new();
        
        public enum Key
        {
            ItemTypeNormal,
            ItemTypeConsume,
        }

        // <Getter Section>
        // * This methods returns data or copy data
        #region Get Methods

        public static string ToString<T>(T type) where T : Enum
        {
            return LanguagePack.TryGetValue(type.ToString(), out var value) ? value : "Not Found";
        }
        
        #endregion
        
        // <Data Load Section>
        // * This methods load data from csv file
        #region Load Methods
        
        private static void LoadLanguageData()
        {
            var fileName = "Lan_" + Constants.Database.CurrentLanguage;
            var table = CSVReader.ReadFile(fileName);
            if (table.Count == 0)
            {
                Debug.LogError("[Database] LoadLanguageData(): CSV Read Failed");
                return;
            }

            try
            {
                LanguagePack.Clear();

                foreach (var col in table)
                {
                    LanguagePack.Add(col[0], col[1]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[Database] LoadLanguageData(): Language Data conversion failed: " + e);
            }
        }
        
        #endregion
    }
}