using System;
using AudioSystem;

namespace ItemSystem.Item
{
    [Serializable]
    public class ConsumeItem : Item
    {
        public int effectId;
        public int effectDelay;

        public SFXType soundType;

        public ConsumeItem(int id, string name, string description, string iconPath
            , int maxStack, int value
            , int effectId, int effectDelay, SFXType soundType)
            : base(id, name, description, iconPath, maxStack, value)
        {
            type = ItemType.Consumable;
            
            this.effectId = effectId;
            this.effectDelay = effectDelay;

            this.soundType = soundType;
        }
    }
}