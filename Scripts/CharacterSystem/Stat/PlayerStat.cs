using System;
using System.Collections.Generic;
using Core;
using DataSystem;
using EventSystem;
using ItemSystem.Produce;
using UnityEngine;

namespace CharacterSystem.Stat
{
    [Serializable]
    public class PlayerStat : Stat
    {
        public int activatedInventorySize = 8;
        public Dictionary<ProducerType, int> ProducerLevel = new();

        public PlayerStat()
        {
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                if (type < 0)
                {
                    continue;
                }
                Attributes[type] = new ModifiableInt();
                Attributes[type].InitValue(Constants.Character.DefaultValue[type]);
            }
            foreach (ProducerType type in Enum.GetValues(typeof(ProducerType)))
            {
                ProducerLevel[type] = 0;
            }
            Debug.Log("Created Player Stat");
            Debug.Log(GetStatInfo());
        }

        public override void SetAttributeValue(AttributeType type, int value)
        {
            base.SetAttributeValue(type, value);

            EventManager.OnNext(Message.OnPlayerAttributeChanged);
        }

        public override void AddAttributeValue(AttributeType type, int value)
        {
            base.AddAttributeValue(type, value);
            
            EventManager.OnNext(Message.OnPlayerAttributeChanged);
        }

        public string GetStatInfo()
        {
            var str = "[Information] Player Stats\nPlayerStat.GetStatInfo()\n";
            str += "\n===== Attributes Value =====\n";
            foreach (var pair in Attributes)
            {
                str += pair.Key + ": " + pair.Value.ModifiedValue + "/" + pair.Value.BaseValue + "\n";
            }
            str += "\n====== Producer Level ======\n";
            foreach (ProducerType type in Enum.GetValues(typeof(ProducerType)))
            {
                str += type + ": " + ProducerLevel[type] + "\n";
            }

            return str;
        }
        
        public override Stat Clone()
        {
            return new PlayerStat();
        }
    }
}