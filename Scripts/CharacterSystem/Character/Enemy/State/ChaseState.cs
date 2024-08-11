using System.Collections;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.StateMachine;
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

        private Coroutine _updateCoroutine = null;
        private static readonly WaitForSeconds Wait = new (1f);
        
        private readonly int _hashIsMoving = Animator.StringToHash("IsMoving");
        private readonly int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");
        
        public override void OnInitialized()
        {
            _mover = Context.GetComponent<EnemyMover>() ?? Context.AddComponent<EnemyMover>();
            _attackable = Context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {            
            _isAttackable = StateMachine.HasState<AttackState>();
            _updateCoroutine = Context.StartCoroutine(UpdateDestination());
            
            Context.SwitchToDetectionFov();

            if (Animator == null)
            {
                return;
            }
            Animator.SetBool(_hashIsMoving, true);
        }

        public override void Update(float deltaTime)
        {
            if (Context.Target)
            {
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

        public override void FixedUpdate(float deltaTime)
        {
            _mover.Move(deltaTime);
        }

        public override void OnExit()
        {
            if (_updateCoroutine != null)
            {
                Context.StopCoroutine(_updateCoroutine);
            }
            _updateCoroutine = null;
            _mover.ClearDestination();
            
            Animator.SetFloat(_hashMoveSpeed, 0f);
            Animator.SetBool(_hashIsMoving, false);
        }
        
        private IEnumerator UpdateDestination()
        {
            while (true)
            {
                if (Context.Target)
                {
                    _mover.SetDestination(Context.Target.position);
                }
                yield return Wait;
            }
        }
    }
}