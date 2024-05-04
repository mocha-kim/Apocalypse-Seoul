using CharacterSystem.Character.StateMachine;
using CharacterSystem.Stat;
using DataSystem;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterSystem.Character.Enemy.State
{
    public class PatrolState : State<EnemyCharacter>
    {
        private EnemyMover _mover;

        private float _range;
        private Vector3 _center;
        private Vector3 _destination;
        
        private bool _isChaseable;

        private Animator _animator;
        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public PatrolState(float range, Vector3 center)
        {
            _range = range;
            _center = center;
        }
        
        public override void OnInitialized()
        {
            _animator = Context.Animator;
            _mover = Context.GetComponent<EnemyMover>() ?? Context.AddComponent<EnemyMover>();
        }

        public override void OnEnter()
        {
            _isChaseable = StateMachine.HasState<ChaseState>();
            
            _destination = new Vector2(
                Random.Range(_center.x - _range, _center.x + _range)
                , Random.Range(_center.y - _range, _center.y + _range)
            );
            
            var speed = Context.Stat.GetAttributeValue(AttributeType.Speed);
            _animator.SetFloat(_hashMoveSpeed, speed * Constants.Animation.SpeedFactor);
            _mover.SetDestination(_destination, Context.Stat.GetAttributeValue(AttributeType.Speed));
            Context.FaceTo(_destination);
                
            if (_animator == null)
            {
                return;
            }
            _animator.SetBool(_hashIsMoving, true);
        }

        public override void Update(float deltaTime)
        {
            if (_isChaseable && Context.Target)
            {
                StateMachine.ChangeState<ChaseState>();
            }
            
            if (Vector3.Distance(Context.transform.position, _destination) < Constants.Tolerance)
            {
                StateMachine.ChangeState<IdleState>();
            }
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