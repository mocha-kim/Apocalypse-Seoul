using System;
using System.Collections.Generic;
using AudioSystem;

namespace ItemSystem.Item
{
    [Serializable]
    public class ConsumeItem : Item
    {
        public List<int> effectIds;
        public int effectDelay;

        public SFXType soundType;

        public ConsumeItem(int id, string name, string description, string iconPath
            , int maxStack, int value, SFXType soundType, int effectDelay)
            : base(id, name, description, iconPath, maxStack, value)
        {
            type = ItemType.Consumable;
            
            this.soundType = soundType;
            this.effectDelay = effectDelay;

            effectIds = new List<int>();
        }
    }
}