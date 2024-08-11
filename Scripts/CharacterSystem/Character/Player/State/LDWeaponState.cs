using System;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.StateMachine;
using DataSystem;
using EnvironmentSystem.Camera;
using EventSystem;
using InputSystem;
using UI;
using UnityEngine;
using Utils;

namespace CharacterSystem.Character.Player.State
{
    public class LDWeaponState : State<PlayerCharacter>
    {
        private const WeaponStateType WeaponStateType = State.WeaponStateType.LongDistance;

        private bool _isAiming = false;
        private Vector2 _normalizedMouseVector;

        private int _hashAttackIndex;
        private int _hashLastMoveX;
        private int _hashLastMoveY;

        private PlayerAttackPainter _attackPainter = null;

        public override void OnInitialized()
        {
            _hashAttackIndex = Animator.StringToHash("AttackIndex");
            _hashLastMoveX = Animator.StringToHash("LastMoveX");
            _hashLastMoveY = Animator.StringToHash("LastMoveY");

            _attackPainter = Context.GetComponentInChildren<PlayerAttackPainter>();
            if (_attackPainter != null)
            {
                _attackPainter.GetComponent<PlayerAttackPainter>().Init(PlayerCharacter.TargetMaskString);
                _attackPainter.StartDrawAim(false);
            }

            EventManager.Subscribe(Context.gameObject, Message.OnCharacterBusy, _ => StopAiming());

            EventManager.Subscribe(Context.gameObject, Message.OnRightMouseDown, _ => StartAiming());
            EventManager.Subscribe(Context.gameObject, Message.OnRightMouseUp, _ => StopAiming());

            EventManager.Subscribe(Context.gameObject, Message.OnLeftMouseDown, _ => Attack());
        }

        public override void OnEnter()
        {
            Animator.SetInteger(_hashAttackIndex, (int)WeaponStateType);
        }

        public override void Update(float deltaTime)
        {
            if (_isAiming)
            {
                _attackPainter.DrawAim();
                SetAnimationDirByMouseVector();
            }
        }

        private void SetAnimationDirByMouseVector()
        {
            if (MouseData.MouseHoveredInventory != null)
            {
                return;
            }

            _normalizedMouseVector = (MainCamera.GetMouseWorldPosition() - Context.PlayerPosition).normalized;
            var dir = DirUtils.ConvertVector2ToDir(_normalizedMouseVector);
            Animator.SetFloat(_hashLastMoveX, dir.x);
            Animator.SetFloat(_hashLastMoveY, dir.y);
        }

        private void StartAiming()
        {
            if (Context.IsBusy || Context.CurrentAttackBehavior is not HitScanSingleAttackBehavior)
            {
                return;
            }

            EventManager.OnNext(Message.OnPlayerAimStart);

            _isAiming = true;
            _attackPainter.StartDrawAim(true);
        }

        private void StopAiming()
        {
            if (Context.CurrentAttackBehavior is not HitScanSingleAttackBehavior)
            {
                return;
            }

            EventManager.OnNext(Message.OnPlayerAimEnd);

            _isAiming = false;
            _attackPainter.StartDrawAim(false);
        }

        private void Attack()
        {
            if (!(Context.CurrentAttackBehavior is HitScanSingleAttackBehavior))
            {
                return;
            }

            if (Context.IsBusy || !Context.CanAttack)
            {
                return;
            }

            if (!DataManager.PlayerInventory.UseItem(100013, 1))
            {
                Debug.Log("No Ammo");
                return;
            }

            Context.Attack();
        }

        public override void OnExit()
        {
            StopAiming();
        }
    }
}