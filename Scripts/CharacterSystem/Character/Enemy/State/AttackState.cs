using System.Collections;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.StateMachine;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Enemy.State
{
    public class AttackState : State<EnemyCharacter>
    {
        private IAttackable _attackable;
        private AttackBehavior AttackBehavior => _attackable.CurrentAttackBehavior;
        
        private float _coolTime;
        private float _attackDelay;
        
        private bool _isReadyToAttack;
        private Coroutine _waitCoolTime;
        private Coroutine _waitDelay;
        
        private bool _isChaseable;
        
        private int _hashAttack = Animator.StringToHash("Attack");
        private int _hashAttackIndex = Animator.StringToHash("AttackIndex"); // Prepared for multiple attack animations

        public override void OnInitialized()
        {
            _attackable = Context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {
            if (_attackable == null)
            {
                Debug.LogWarning("There is no IAttackable in " + Context);
                StateMachine.ChangeState<IdleState>();
                return;
            }
            
            _isChaseable = StateMachine.HasState<ChaseState>();
            
            if (Animator == null)
            {
                return;
            }
            Animator.SetInteger(_hashAttackIndex, _attackable.CurrentAttackBehavior.animationIndex);
            Animator.SetTrigger(_hashAttack);

            _coolTime = _attackable.CurrentAttackBehavior.coolTime;
            _attackDelay = _attackable.CurrentAttackBehavior.attackDelay;

            _waitDelay = Context.StartCoroutine(WaitToAttack(_attackDelay));
        }

        public override void Update(float deltaTime)
        {
            if (_isReadyToAttack)
            {
                _attackable.Attack();
                _waitCoolTime = Context.StartCoroutine(WaitToAttack(_coolTime));
            }
            
            if (Context.Target == null)
            {
                StateMachine.ChangeState<IdleState>();
                return;
            }
            
            if (_isChaseable && Context.DistanceToTarget > AttackBehavior.Range)
            {
                StateMachine.ChangeState<ChaseState>();
            }
        }

        public override void OnExit()
        {
            if (_waitDelay == null)
            {
                return;
            }
            Context.StopCoroutine(_waitDelay);
        }

        private IEnumerator WaitToAttack(float seconds)
        {
            _isReadyToAttack = false;
            
            var elapsedTime = 0f;
            while (elapsedTime < seconds)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isReadyToAttack = true;
        }
    }
}