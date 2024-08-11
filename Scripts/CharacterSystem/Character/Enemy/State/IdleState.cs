using CharacterSystem.Character.StateMachine;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSystem.Character.Enemy.State
{
    public class IdleState : State<EnemyCharacter>
    {
        private bool _isChaseable;
        private bool _isPatrollable;

        private float _idleTime;
        private readonly RangeFloat _idleTimeRange = new(1f, 5f);

        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnEnter()
        {
            _isChaseable = StateMachine.HasState<ChaseState>();
            _isPatrollable = StateMachine.HasState<PointPatrolState>() || StateMachine.HasState<RandomPatrolState>();

            _idleTime = Random.Range(_idleTimeRange.start, _idleTimeRange.end);
            Context.SwitchToIdleFOV();

            if (Animator == null)
            {
                return;
            }

            Animator.SetBool(_hashIsMoving, false);
            Animator.SetFloat(_hashMoveSpeed, 0);
        }

        public override void Update(float deltaTime)
        {
            _idleTime -= deltaTime;

            if (Context.Target && _isChaseable)
            {
                StateMachine.ChangeState<ChaseState>();
                return;
            }

            if (_idleTime <= 0 && _isPatrollable)
            {
                if (StateMachine.HasState<PointPatrolState>())
                {
                    StateMachine.ChangeState<PointPatrolState>();
                    return;
                }

                if (StateMachine.HasState<RandomPatrolState>())
                {
                    StateMachine.ChangeState<RandomPatrolState>();
                    return;
                }
            }
        }
    }
}