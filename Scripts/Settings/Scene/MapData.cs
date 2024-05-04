using System;

namespace Settings.Scene
{
    [Serializable]
    public class MapData
    {
        public int id;
        public string name;
        public string description;
        public string scenePath;

        public MapData(int id, string name, string description, string scenePath)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.scenePath = scenePath;
        }
    }
}