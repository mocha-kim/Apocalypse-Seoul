using System;
using Alpha;
using Core;

namespace CharacterSystem.Stat
{
    [Serializable]
    public class EnemyStat : Stat
    {
        private static AttributeType[] _attributeTypes =
        {
            AttributeType.Hp, 
            AttributeType.Speed, 
            AttributeType.Attack, 
            AttributeType.Defense,
            AttributeType.AttackSpeed,
            AttributeType.AttackRange,
        };

        public EnemyStat(int hp, int speed, int attack, int defense, int attackSpeed, int attackRange)
        {
            foreach (var type in _attributeTypes)
            {
                Attributes[type] = new ModifiableInt();
            }

            Attributes[AttributeType.Hp].InitValue(hp);
            Attributes[AttributeType.Speed].InitValue(speed);
            Attributes[AttributeType.Attack].InitValue(attack);
            Attributes[AttributeType.Defense].InitValue(defense);
            Attributes[AttributeType.AttackSpeed].InitValue(attackSpeed);
            Attributes[AttributeType.AttackRange].InitValue(attackRange);
        }

        public override Stat Clone()
        {
            return new EnemyStat(
                Attributes[AttributeType.Hp].BaseValue,
                Attributes[AttributeType.Speed].BaseValue,
                Attributes[AttributeType.Attack].BaseValue,
                Attributes[AttributeType.Defense].BaseValue,
                Attributes[AttributeType.AttackSpeed].BaseValue,
                Attributes[AttributeType.AttackRange].BaseValue
            );
        }
    }
}