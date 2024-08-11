using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public abstract class RangeAttackBehavior : AttackBehavior
    {
        public float range;
        public float angle;

        protected HitBox ClosestTarget;
        protected List<HitBox> Targets = new();

        public override float Range => range;

        protected RangeAttackBehavior(Transform context, float range = 3f, float angle = 45f
            , float attackDelay = 1, float coolTime = 2, string[] targetMaskStrings = null)
            : base(context, attackDelay, coolTime, targetMaskStrings)
        {
            this.range = range;
            this.angle = angle;
        }

        protected void DetectTargets(Vector3 attackDirection)
        {
            Targets.Clear();
            ClosestTarget = null;
            var nearestDistance = range;

            // Find Targets in ViewRadius
            var targetsInRadius = Physics2D.OverlapCircleAll(StartPoint, range, TargetMask);
            foreach (var targetCollider in targetsInRadius)
            {
                if (!targetCollider.transform.TryGetComponent(out HitBox target))
                {
                    continue;
                }
                
                // Check Target is in ViewAngle
                var direction = (target.transform.position - StartPoint).normalized;
                if (!(Vector3.Angle(attackDirection, direction) < angle / 2)) continue;

                // Check Target is in View(not obscured by obstacles)
                var distance = Vector3.Distance(StartPoint, target.transform.position);
                if (Physics2D.Raycast(StartPoint, direction, distance, ObstacleMask)) continue;

                Targets.Add(target);
                // Find nearest Target
                if (ClosestTarget != null && nearestDistance <= distance)
                {
                    continue;
                }

                ClosestTarget = target;
                nearestDistance = distance;
            }
        }
    }
}