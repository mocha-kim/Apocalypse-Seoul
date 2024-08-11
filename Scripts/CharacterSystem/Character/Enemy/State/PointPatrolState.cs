using UnityEngine;

namespace CharacterSystem.Character.Enemy.State
{
    public class PointPatrolState : PatrolState
    {
        private Vector3[] _patrolPoints;
        private int _pointNum = -1;

        public PointPatrolState(Vector3[] patrolPoints)
        {
            _patrolPoints = patrolPoints;
        }

        protected override void UpdateNextDestination()
        {
            _pointNum = (_pointNum + 1) % _patrolPoints.Length;
            Destination = _patrolPoints[_pointNum];
        }
    }
}