using System;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public class RangeSingleAttackBehavior : RangeAttackBehavior
    {
        public RangeSingleAttackBehavior(Transform attackTrans, float range = 3f, float angle = 90f
            , float attackDelay = 1f, float coolTime = 2f, string[] targetMaskStrings = null)
            : base(attackTrans, range, angle, attackDelay, coolTime, targetMaskStrings)
        {
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
            DetectTargets(attackDirection);

            if (ClosestTarget == null)
            {
                return;
            }
            var targetDamageable = ClosestTarget.parent.GetComponent<IDamageable>();
            if (targetDamageable == null)
            {
                return;
            }
            targetDamageable.Damage(damage);
            targetDamageable.SetTarget(context);
        }
    }
}