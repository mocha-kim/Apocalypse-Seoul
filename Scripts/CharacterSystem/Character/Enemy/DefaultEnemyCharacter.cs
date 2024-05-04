using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Stat;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    public abstract class DefaultEnemyCharacter : DummyEnemyCharacter, IAttackable
    {
        protected override void Start()
        {
            base.Start();

            StateMachine.AddState(new ChaseState());
            StateMachine.AddState(new AttackState());
            StateMachine.Refresh();
            
            SwitchToIdleFOV();
        }

        #region IAttackable

        public AttackBehavior CurrentAttackBehavior { get; protected set; }
        [SerializeField] protected string[] targetMaskStrings = { "Player" };

        public void Attack()
        {
            if (CurrentAttackBehavior == null || FOV.ClosestTarget == null)
            {
                return;
            }

            CurrentAttackBehavior.Attack(
                Stat.GetAttributeValue(AttributeType.Attack)
                , FOV.ClosestTarget.position - transform.position
            );
        }

        #endregion
    }
}