using System;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public class DefaultAttackBehavior : AttackBehavior
    {
        public override float Range => 0f;
        
        public DefaultAttackBehavior()
            : base(null)
        {
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
        }
    }
}