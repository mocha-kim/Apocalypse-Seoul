using System.Collections;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using DG.Tweening;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    public class DummyEnemyCharacter : EnemyCharacter, IDamageable
    {
        protected override void Start()
        {
            base.Start();

            InitEnemyStat(100, 1, 10, 0, 0, 0);

            SwitchToIdleFOV();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsAlive)
            {
                ChangeState<DeadState>();
            }
        }

        #region IDamagable

        public bool IsAlive => Stat.Attributes[AttributeType.Hp].ModifiedValue > 0;

        public void Damage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }
            
            Stat.AddAttributeValue(AttributeType.Hp, -damage);
            Debug.Log($"{gameObject.name} damaged: Remained hp({Stat.Attributes[AttributeType.Hp].ModifiedValue})");
            _render.DOColor(Color.red, Constants.Character.DamageEffectDuration)
                .SetLoops(9, LoopType.Yoyo)
                .OnComplete(() => _render.DOColor(Color.white, Constants.Character.DamageEffectDuration));
        }
        
        public void SetTarget(Transform trans) => FOV.SetTarget(trans);

        #endregion
    }
}