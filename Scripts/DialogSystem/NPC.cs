using System;
using CharacterSystem.Character;
using Manager;
using UnityEngine;

namespace DialogSystem
{
    [Serializable]
    public class NPC
    {
        public int id = -1;
        public CharacterType type;
        public string name = "";

        public string spritePath;
        public string portraitPath;
        public Sprite portraitImage;

        public int inventoryID;

        public NPC(int id, CharacterType type, string name, string spritePath, string portraitPath, int inventoryID = -1)
        {
            this.id = id;
            this.type = type;
            this.name = name;

            this.spritePath = spritePath;
            this.portraitPath = portraitPath;

            this.inventoryID = inventoryID;
        }

        public void Init()
        {
            portraitImage = ResourceManager.GetSprite(portraitPath);
        }
    }
}