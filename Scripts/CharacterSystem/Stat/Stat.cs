using System.Collections.Generic;
using Alpha;
using Core;
using Event;
using Manager;
using UnityEngine;

namespace CharacterSystem.Stat
{
    public abstract class Stat
    {
        private Dictionary<AttributeType, ModifiableInt> _attributes = new();

        public Dictionary<AttributeType, ModifiableInt> Attributes
        {
            get => _attributes;
            protected set => _attributes = value;
        }

        public int GetAttributeValue(AttributeType type)
        {
            if (_attributes.TryGetValue(type, out var mValue))
            {
                return mValue.ModifiedValue;
            }
            Debug.LogWarning($"[Stat] SetAttributeValue(): There no type({type})");
            return -1;
        }

        public virtual void SetAttributeValue(AttributeType type, int value)
        {
            if (!_attributes.TryGetValue(type, out var mValue))
            {
                Debug.LogWarning($"[Stat] SetAttributeValue(): There no type({type})");
                return;
            }
            mValue.ModifiedValue = value;
            
            // HP, SP, Hunger, Thirst values' max value == base value
            if (type is AttributeType.Hp or AttributeType.Sp or AttributeType.Hunger or AttributeType.Thirst)
            {
                var att = _attributes[type];
                att.ModifiedValue = att.ModifiedValue > att.BaseValue ? att.BaseValue : att.ModifiedValue;
            }
        }
        
        public virtual void AddAttributeValue(AttributeType type, int value)
        {
            if (!_attributes.TryGetValue(type, out var mValue))
            {
                Debug.LogWarning($"[Stat] AddAttributeValue(): There no type({type})");
                return;
            }
            mValue.ModifiedValue += value;
            
            // HP, SP, Hunger, Thirst values' max value == base value
            if (type is AttributeType.Hp or AttributeType.Sp or AttributeType.Hunger or AttributeType.Thirst)
            {
                var att = _attributes[type];
                att.ModifiedValue = att.ModifiedValue > att.BaseValue ? att.BaseValue : att.ModifiedValue;
            }
        }

        public void LevelUp()
        {
            // TODO: change base value by level
            EventManager.OnNext(Message.OnPlayerAttributeChanged);
        }
    }
}