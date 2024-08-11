using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using DG.Tweening;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    public partial class DummyEnemyCharacter : EnemyCharacter
    {
        protected override void Start()
        {
            base.Start();

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
    }

    // StateMachine
    public partial class DummyEnemyCharacter
    {
        protected override void InitStateMachine()
        {
            base.InitStateMachine();
        }
    }

    public partial class DummyEnemyCharacter : IDamageable
    {
        public bool IsAlive => Stat.Attributes[AttributeType.Hp].ModifiedValue > 0;

        public void Damage(int damage)
        {
            if (!IsAlive)
            {
                _render.DOComplete();
                return;
            }

            damage = Mathf.Max(1, (int)(damage * (0.75f) + 0.1f * Random.Range(0, 5)));

            Stat.AddAttributeValue(AttributeType.Hp,
                -Mathf.Max(0, damage - Stat.GetAttributeValue(AttributeType.Defense))
            );
            Debug.Log($"{gameObject.name} damaged: Remained hp({Stat.Attributes[AttributeType.Hp].ModifiedValue})");
            _render.DOColor(Color.red, Constants.Character.DamageEffectDuration)
                .SetLoops(9, LoopType.Yoyo)
                .OnComplete(() => _render.DOColor(Color.white, Constants.Character.DamageEffectDuration));
        }

        public void SetTarget(Transform trans) => FOV.SetTarget(trans);
    }
}