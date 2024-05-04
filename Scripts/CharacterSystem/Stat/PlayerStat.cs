using System;
using System.Collections.Generic;
using Core;
using DataSystem;
using Event;
using ItemSystem.Produce;
using Manager;
using UnityEngine;

namespace CharacterSystem.Stat
{
    public class PlayerStat : Stat
    {
        public int ActivatedInventorySize = 8;
        public Dictionary<ProducerType, int> ProducerLevel = new();

        public PlayerStat()
        {
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
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
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                str += type + ": " + Attributes[type].ModifiedValue + "/" + Attributes[type].BaseValue + "\n";
            }
            str += "\n====== Producer Level ======\n";
            foreach (ProducerType type in Enum.GetValues(typeof(ProducerType)))
            {
                str += type + ": " + ProducerLevel[type] + "\n";
            }

            return str;
        }
    }
}