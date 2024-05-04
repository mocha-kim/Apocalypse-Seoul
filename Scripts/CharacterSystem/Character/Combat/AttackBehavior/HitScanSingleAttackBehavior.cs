using System;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public class HitScanSingleAttackBehavior : HitScanAttackBehavior
    {
        public HitScanSingleAttackBehavior(Transform context, float range = 10
            , float attackDelay = 1, float coolTime = 2, string[] targetMaskStrings = null)
            : base(context, range, attackDelay, coolTime, targetMaskStrings)
        {
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
            DetectTarget();

            if (ClosestTarget == null)
            {
                return;
            }
            var damageable = ClosestTarget.gameObject.GetComponent<IDamageable>() ?? ClosestTarget.transform.parent.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }
            damageable.Damage(damage);
            damageable.SetTarget(context);
        }
    }
}