using CharacterSystem.Character.StateMachine;
using DataSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace CharacterSystem.Character.Enemy.State
{
    public abstract class PatrolState : State<EnemyCharacter>
    {
        protected EnemyMover Mover;
        
        protected Vector3 Destination;
        private bool _isChaseable;

        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            Mover = Context.GetComponent<EnemyMover>() ?? Context.AddComponent<EnemyMover>();
            
            UpdateNextDestination();
        }

        public override void OnEnter()
        {
            _isChaseable = StateMachine.HasState<ChaseState>();

            var speed = Context.MoveSpeed;
            Animator.SetFloat(_hashMoveSpeed, speed * Constants.Animation.MoveSpeedFactor);
            
            Mover.SetDestination(Destination);
            Context.FaceTo(Destination);

            if (Animator == null)
            {
                return;
            }
            Animator.SetBool(_hashIsMoving, true);
        }

        public override void Update(float deltaTime)
        {
            if (_isChaseable && Context.Target)
            {
                StateMachine.ChangeState<ChaseState>();
            }

            if (Vector3.Distance(Context.transform.position, Destination) < Constants.IsometricPositionTolerance)
            {
                UpdateNextDestination();
                StateMachine.ChangeState<IdleState>();
            }
        }

        public override void FixedUpdate(float deltaTime)
        {
            Mover.Move(deltaTime);
        }

        public override void OnExit()
        {
            Mover.ClearDestination();
            Animator.SetFloat(_hashMoveSpeed, 0f);
            Animator.SetBool(_hashIsMoving, false);
        }

        protected abstract void UpdateNextDestination();
    }
}