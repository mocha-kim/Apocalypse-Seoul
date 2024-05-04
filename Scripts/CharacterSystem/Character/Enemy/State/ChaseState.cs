using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.StateMachine;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace CharacterSystem.Character.Enemy.State
{
    public class ChaseState : State<EnemyCharacter>
    {
        private EnemyMover _mover;
        
        private IAttackable _attackable;
        private AttackBehavior AttackBehavior => _attackable.CurrentAttackBehavior;
        
        private bool _isAttackable;
        private float _waitTime = Constants.Delay.AfterChaseDelay;

        private Animator _animator;
        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            _animator = Context.Animator;
            _mover = Context.GetComponent<EnemyMover>() ?? Context.AddComponent<EnemyMover>();
            _attackable = Context.GetComponent<IAttackable>();
            
        }

        public override void OnEnter()
        {            
            _isAttackable = StateMachine.HasState<AttackState>();
            
            Context.SwitchToDetectionFov();

            if (_animator == null)
            {
                return;
            }
            _animator.SetBool(_hashIsMoving, true);
        }

        public override void Update(float deltaTime)
        {
            if (Context.Target)
            {
                var speed = Context.Stat.GetAttributeValue(AttributeType.Speed);
                _mover.SetDestination(Context.Target.position, speed);
                _animator.SetFloat(_hashMoveSpeed, speed * Constants.Animation.SpeedFactor);
                
                if (_isAttackable && AttackBehavior != null && Context.DistanceToTarget <= AttackBehavior.Range)
                {
                    StateMachine.ChangeState<AttackState>();
                }
                return;
            }

            _waitTime -= deltaTime;
            if (_waitTime > 0f)
            {
                return;
            }
            StateMachine.ChangeState<IdleState>();
        }

        public override void FixedUpdate()
        {
            _mover.Move();
        }

        public override void OnExit()
        {
            _animator.SetFloat(_hashMoveSpeed, 0f);
            _animator.SetBool(_hashIsMoving, false);
        }
    }
}