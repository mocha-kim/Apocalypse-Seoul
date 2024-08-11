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
            
            if (ClosestTarget.Damageable == null)
            {
                return;
            }
            ClosestTarget.Damageable.Damage(damage);
            ClosestTarget.Damageable.SetTarget(context);
            
            if (Effect == null || ClosestTarget.Affectable == null)
            {
                return;
            }
            ClosestTarget.Affectable.Affect(Effect);
        }
    }
}