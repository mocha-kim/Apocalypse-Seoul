using System;
using System.Collections.Generic;
using EnvironmentSystem.Camera;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public abstract class HitScanAttackBehavior : AttackBehavior
    {
        public float range;

        protected HitBox ClosestTarget;
        protected List<HitBox> Targets = new();

        public override float Range => range;

        protected HitScanAttackBehavior(Transform context, float range = 10f
            , float attackDelay = 1, float coolTime = 2, string[] targetMaskStrings = null)
            : base(context, attackDelay, coolTime, targetMaskStrings)
        {
            this.range = range;
        }

        protected void DetectTarget()
        {
            Targets.Clear();
            ClosestTarget = null;
            var nearestDistance = range;
            var attackDirection = (MainCamera.GetMouseWorldPosition() - StartPoint).normalized;

            // Find Targets in Radius

            //var targetsInRadius = Physics2D.RaycastAll(StartPoint, attackDirection, range, TargetMask);
            var targetsInRadius = Physics2D.RaycastAll(StartPoint, attackDirection, range, TargetMask);
            foreach (var targetCollider in targetsInRadius)
            {
                if (!targetCollider.transform.TryGetComponent(out HitBox target))
                {
                    target = targetCollider.transform.GetComponentInChildren<HitBox>();
                    if (target == null)
                    {
                        continue;
                    }
                }

                // Check Target is in View(not obscured by obstacles)
                var distance = Vector3.Distance(StartPoint, target.transform.position);
                if (Physics2D.Raycast(StartPoint, attackDirection, distance, ObstacleMask))
                {
                    continue;
                }

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