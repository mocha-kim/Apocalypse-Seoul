using System;
using System.Collections.Generic;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Stat;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    public partial class DefaultEnemyCharacter : DummyEnemyCharacter
    {
        protected override void Start()
        {
            base.Start();
            
            SwitchToIdleFOV();
            StateMachine.Refresh();
            
            InitAttackBehaviour();
        }
        
        public override void SwitchToIdleFOV() => FOV.SetIdleViewFactors(AttackType, _targetMask);
        public override void SwitchToDetectionFov() => FOV.SetDetectViewFactors(AttackType, _targetMask);
    }

    // StateMachine
    public partial class DefaultEnemyCharacter
    {
        protected override void InitStateMachine()
        {
            base.InitStateMachine();
            
            StateMachine.AddState(new ChaseState());
            StateMachine.AddState(new AttackState());
        }
    }
    
    public partial class DefaultEnemyCharacter : IAttackable
    {
        protected readonly List<AttackBehavior> AttackBehaviors = new();
        
        public AttackBehavior CurrentAttackBehavior { get; protected set; }
        [SerializeField] protected string[] targetMaskStrings = { "Player" };
        private int _targetMask;
        
        private ProjectilePoolController _bulletPool = null;

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

        private void InitAttackBehaviour()
        {
            _targetMask = LayerMask.GetMask(targetMaskStrings);
            switch (AttackType)
            {
                case AttackType.RangeSingle:
                    AttackBehaviors.Add(
                        new RangeSingleAttackBehavior(transform
                            , range: Stat.GetAttributeValue(AttributeType.AttackRange)
                            , targetMaskStrings: targetMaskStrings)
                    );
                    break;
                case AttackType.Projectile:
                    _bulletPool = GetComponentInChildren<ProjectilePoolController>();
                    if (_bulletPool == null)
                    {
                        Debug.LogError("[DefaultEnemyCharacter] InitAttackBehaviour(): " +
                                       "There is no ProjectilePoolController in ProjectileAttackBehaviour enemy. " +
                                       "This must be fixed.");
                        return;
                    }
                    _bulletPool.Init(LayerMask.GetMask(targetMaskStrings));
                    AttackBehaviors.Add(
                        new ProjectileAttackBehavior(transform
                            , Stat.GetAttributeValue(AttributeType.Attack)
                            , Stat.GetAttributeValue(AttributeType.AttackRange)
                            , 0f
                            , 2f / Stat.GetAttributeValue(AttributeType.AttackSpeed)
                            , targetMaskStrings
                            , 5,
                            _bulletPool)
                    );
                    break;
                default:
                    return;
            }
            
            CurrentAttackBehavior = AttackBehaviors[0];
        }
    }
}