using System.IO;
using Alpha;
using UnityEngine;

namespace DataSystem.FileIO
{
    public static class SystemPath
    {
        public static string GetPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return Application.persistentDataPath;      
                case RuntimePlatform.WindowsEditor:
                    return Application.dataPath;
                default:
                    return Application.dataPath;
            }
        }

        public static string GetSaveDataPath() => GetPath() + "/Saves/";
    }
}
