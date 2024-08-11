using CharacterSystem.Character.Enemy.State;
using DataSystem;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    public class PatrolEnemyCharacter : DefaultEnemyCharacter
    {
        [SerializeField] private Vector3[] _patrolPoints;

        protected override void InitStateMachine()
        {
            base.InitStateMachine();
            
            if (_patrolPoints.Length > 0)
            {
                StateMachine.AddState(new PointPatrolState(_patrolPoints));
            }
            else
            {
                StateMachine.AddState(new RandomPatrolState(Constants.Character.PatrolRange, transform.position));
            }
        }
    }
}