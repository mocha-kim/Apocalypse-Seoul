using System;
using System.Collections.Generic;
using Alpha;
using Core;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Stat
{
    [Serializable]
    public abstract class Stat
    {
        public Dictionary<AttributeType, ModifiableInt> Attributes = new();

        public int GetAttributeValue(AttributeType type)
        {
            if (Attributes.TryGetValue(type, out var mValue))
            {
                return mValue.ModifiedValue;
            }
            Debug.LogWarning($"[Stat] SetAttributeValue(): There no type({type})");
            return -1;
        }

        public virtual void SetAttributeValue(AttributeType type, int value)
        {
            if (!Attributes.TryGetValue(type, out var mValue))
            {
                Debug.LogWarning($"[Stat] SetAttributeValue(): There no type({type})");
                return;
            }
            mValue.ModifiedValue = value;
            
            // HP, SP, Hunger, Thirst values' max value == base value
            if (type is AttributeType.Hp or AttributeType.Sp or AttributeType.Hunger or AttributeType.Thirst)
            {
                var att = Attributes[type];
                att.ModifiedValue = att.ModifiedValue > att.BaseValue ? att.BaseValue : att.ModifiedValue;
            }
        }
        
        public virtual void AddAttributeValue(AttributeType type, int value)
        {
            if (!Attributes.TryGetValue(type, out var attribute))
            {
                Debug.LogWarning($"[Stat] AddAttributeValue(): There no type({type})");
                return;
            }
            attribute.ModifiedValue += value;
            attribute.ModifiedValue = Mathf.Max(0, attribute.ModifiedValue);
            
            // HP, SP, Hunger, Thirst values' max value == base value
            if (type is AttributeType.Hp or AttributeType.Sp or AttributeType.Hunger or AttributeType.Thirst)
            {
                attribute.ModifiedValue = Mathf.Min(attribute.ModifiedValue, attribute.BaseValue);
            }
        }

        public void LevelUp()
        {
            // TODO: change base value by level
            EventManager.OnNext(Message.OnPlayerAttributeChanged);
        }

        public abstract Stat Clone();
    }
}