using System;
using CharacterSystem.Character.StateMachine;
using Core;
using Unity.VisualScripting.FullSerializer;
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

        private Animator _animator;
        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");
        
        public override void OnInitialized()
        {
            _animator = Context.Animator;
        }

        public override void OnEnter()
        {
            _isChaseable = StateMachine.HasState<ChaseState>();
            _isPatrollable = StateMachine.HasState<PatrolState>();
            
            _idleTime = Random.Range(_idleTimeRange.start, _idleTimeRange.end);
            Context.SwitchToIdleFOV();

            if (_animator == null)
            {
                return;
            }
            _animator.SetBool(_hashIsMoving, false);
            _animator.SetFloat(_hashMoveSpeed, 0);
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
                StateMachine.ChangeState<PatrolState>();
            }
        }
    }
}