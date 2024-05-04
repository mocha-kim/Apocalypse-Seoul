using System;
using Newtonsoft.Json;

namespace ItemSystem.Item
{
    [Serializable]
    public class Item
    {
        public int id;
        public ItemType type;
        
        public string name;
        public string description;
        public string iconPath;
        
        public int maxStackCount;
        public int value;

        public Item(int id, string name, string description, string iconPath
            , int maxStack, int value)
        {
            this.id = id;
            type = ItemType.Normal;
            
            this.name = name;
            this.description = description;
            this.iconPath = iconPath;
            
            maxStackCount = maxStack;
            this.value = value;
        }
        
        [JsonConstructor]
        public Item(){}
    }
}