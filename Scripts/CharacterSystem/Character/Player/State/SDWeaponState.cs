using System;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.StateMachine;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Character.Player.State
{
    public class SDWeaponState : State<PlayerCharacter>
    {
        private const WeaponStateType WeaponStateType = State.WeaponStateType.ShortDistance;

        private int _hashAttackIndex;


        private PlayerAttackPainter _attackPainter = null;

        public override void OnInitialized()
        {
            _hashAttackIndex = Animator.StringToHash("AttackIndex");

            _attackPainter = Context.GetComponentInChildren<PlayerAttackPainter>();
            if (_attackPainter != null)
            {
                _attackPainter.StartDrawRange(false);
            }

            EventManager.Subscribe(Context.gameObject, Message.OnLeftMouseDown, _ => Attack());
        }

        public override void OnEnter()
        {
            Animator.SetInteger(_hashAttackIndex, (int)WeaponStateType);
            _attackPainter.DrawRange();
            _attackPainter.StartDrawRange(true);
        }

        public override void Update(float deltaTime)
        {
            _attackPainter.DrawRange();
        }

        private void Attack()
        {
            if (!(Context.CurrentAttackBehavior is ColliderAttackBehavior))
            {
                return;
            }

            if (Context.IsBusy || !Context.CanAttack)
            {
                return;
            }

            Context.Attack();
        }

        public override void OnExit()
        {
            _attackPainter.StartDrawRange(false);
        }
    }
}