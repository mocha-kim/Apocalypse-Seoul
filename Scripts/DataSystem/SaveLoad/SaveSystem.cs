using System;
using System.IO;
using System.Reflection;
using DataSystem.FileIO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Exception = System.Exception;

namespace DataSystem.SaveLoad
{
    public static class SaveSystem
    {
        private static string SavePath => SystemPath.GetSaveDataPath();

        public static void SaveData<T>(T data, string saveFileName)
        {
            string fullPath = SavePath + saveFileName + ".json";

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            string encodedJson = System.Convert.ToBase64String(bytes);

            File.WriteAllText(fullPath, encodedJson);
        }

        public static T LoadData<T>(string loadFileName, T data)
        {
            string fullPath = SavePath + loadFileName + ".json";

            if (!File.Exists(fullPath))
            {
                Debug.LogError("[SaveSystem] LoadData(): No such saveFile exists");
            }

            string json = DecodeFile(fullPath);

            T loadData = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

            return loadData;
        }

        public static void ResetData()
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(SavePath))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (IOException)
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Directory.Delete(directory, true);
                    }
                }

                try
                {
                    Directory.Delete(SavePath, true);
                }
                catch (IOException)
                {
                    Directory.Delete(SavePath, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(SavePath, true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[SaveSystem] ResetData(): Data Reset Failed.\n" + e);
            }
        }

        private static string DecodeFile(string path)
        {
            var loadJson = File.ReadAllText(path);
            var bytes = System.Convert.FromBase64String(loadJson);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public class PrivateResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    var hasPrivateSetter = property?.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }

                return prop;
            }
        }
    }
}