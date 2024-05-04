using System.Collections;
using System.Collections.Generic;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Combat
{
    public class FieldOfView : MonoBehaviour
    {
        private WaitForSeconds _waitDelay = new(0.2f);

        private int _targetMask;
        private int _obstacleMask;

        private IHasLogicalForward _self;

        [field: Range(0, 360)]
        public float ViewAngle { get; private set; } = 60f;
        public float ViewRadius { get; private set; } = 3f;
        public Vector3 Forward => _self?.Forward ?? Vector3.down;
        
        public float DistanceToTarget { get; private set; }
        public Transform ClosestTarget { get; private set; }

        private void Start()
        {
            _targetMask = LayerMask.GetMask("Player");
            _obstacleMask = LayerMask.GetMask("Obstacle");

            _self = GetComponent<IHasLogicalForward>();

            DistanceToTarget = ViewRadius;
            
            StartCoroutine(FindTargetsWithDelay());
        }

        public void SetViewFactors(float viewRadius, float viewAngle, string[] targetLayerNames)
        {
            ViewRadius = viewRadius;
            ViewAngle = viewAngle;
            _targetMask = LayerMask.GetMask(targetLayerNames);
        }

        private IEnumerator FindTargetsWithDelay()
        {
            while (true)
            {
                yield return _waitDelay;
                FindTarget();
            }
        }

        private void FindTarget()
        {
            ClosestTarget = null;
            DistanceToTarget = ViewRadius;

            // Find Targets in ViewRadius
            var targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, ViewRadius, _targetMask);
            foreach (var targetCollider in targetsInViewRadius)
            {            
                var target = targetCollider.transform;
                // Check Target is in ViewAngle
                var direction = (target.position - transform.position).normalized;
                if (!(Vector3.Angle(Forward, direction) < ViewAngle / 2))
                {
                    continue;
                }
                
                // Check Target is in View(not obscured by obstacles)
                var distance = Vector3.Distance(transform.position, target.position);
                if (Physics2D.Raycast(transform.position, direction, distance, _obstacleMask))
                {
                    continue;
                }
                
                // Find nearest Target
                if (ClosestTarget != null && DistanceToTarget <= distance)
                {
                    continue;
                }
                
                ClosestTarget = target;
                DistanceToTarget = distance;
            }
        }

        public void SetTarget(Transform targetTrans)
        {
            ClosestTarget = targetTrans;
        }

        public Vector3 GetDirectionFromAngle(float angleInDeg, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDeg += transform.eulerAngles.y;
            }

            var rad = angleInDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0);
        }
    }
}