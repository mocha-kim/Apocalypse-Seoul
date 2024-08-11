using System.Collections;
using System.Collections.Generic;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.Player.State;
using CharacterSystem.Character.StateMachine;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using DataSystem.Database;
using DG.Tweening;
using EnvironmentSystem.Camera;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Character.Player
{
    public sealed partial class PlayerCharacter : Character
    {
        private static PlayerStat Stat => DataManager.Stat;
        private StateMachine<PlayerCharacter> _stateMachine;

        public override CharacterType CharacterType => CharacterType.Player;

        public Vector3 PlayerPosition => transform.position;
        public Vector2 NormalizedMouseVector { get; private set; } = Vector2.down;

        [SerializeField] private AttackCollider _attackCollider;

        public bool IsBusy { get; private set; }

        [SerializeField] private Animator _animator;
        private int _hashLastMoveX;
        private int _hashLastMoveY;

        private int _hashIsConsuming;
        private int _hashConsumeItemId;

        public bool CanAttack = true;

        #region MonoBehaviour

        private void Awake()
        {
            _hashLastMoveX = Animator.StringToHash("LastMoveX");
            _hashLastMoveY = Animator.StringToHash("LastMoveY");

            _hashIsConsuming = Animator.StringToHash("IsConsuming");
            _hashConsumeItemId = Animator.StringToHash("ConsumeItemId");

            EventManager.Subscribe(gameObject, Message.OnCharacterBusy, _ => OnCharacterBusy());
            EventManager.Subscribe(gameObject, Message.OnCharacterFree, _ => OnCharacterFree());

            EventManager.Subscribe(gameObject, Message.OnPressChangeWeapon, _ => ChangeWeapon());
            EventManager.Subscribe(gameObject, Message.OnEnemyDamaged, _ => OnAttackSucceeded());

            EventManager.Subscribe(gameObject, Message.OnTryItemUse, OnTryItemUse);
        }

        private void Start()
        {
            InitWeaponStates();
            InitStateMachine();
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        #endregion

        private void InitStateMachine()
        {
            _stateMachine =
                new StateMachine<PlayerCharacter>(this, _weaponStates[WeaponStateType.Default].State, _animator);
            _stateMachine.AddState(_weaponStates[WeaponStateType.ShortDistance].State);
            _stateMachine.AddState(_weaponStates[WeaponStateType.LongDistance].State);
            _stateMachine.AddState(new DeadState());
        }

        private void OnCharacterBusy()
        {
            IsBusy = true;
        }

        private void OnCharacterFree()
        {
            IsBusy = false;
        }

        private void OnTryItemUse(EventManager.Event e)
        {
            try
            {
                var itemId = (int)e.Args[0];
                _animator.SetBool(_hashIsConsuming, true);
                StartCoroutine(WaitAndUseItem(itemId));
            }
            catch
            {
                Debug.Log("OnTryItemUse(): Invalid Event Argument");
            }
        }

        private IEnumerator WaitAndUseItem(int itemId)
        {
            var elapsedTime = 0f;
            var delay = Database.GetConsumeItem(itemId).effectDelay;
            while (elapsedTime < delay)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _animator.SetBool(_hashIsConsuming, false);
            DataManager.PlayerInventory.UseItem(itemId, 1);
        }
    }

    public sealed partial class PlayerCharacter : IMovable
    {
        public float MoveSpeed => Stat.GetAttributeValue(AttributeType.Speed) * Constants.Character.MoveSpeedFactor;

        public void Move()
        {
            Debug.LogWarning(
                "[PlayerCharacter] Move(): Player character moves on its own mover. This method do nothing");
        }
    }

    public sealed partial class PlayerCharacter : IAttackable
    {
        private WeaponStateType _currentWeaponType = WeaponStateType.Default;
        private readonly Dictionary<WeaponStateType, WeaponState> _weaponStates = new();
        public static readonly string[] TargetMaskString = { Constants.Layer.EnemyString };

        private class WeaponState
        {
            public AttackBehavior AttackBehavior;
            public State<PlayerCharacter> State;
        }

        #region IAttackble

        public AttackBehavior CurrentAttackBehavior { get; private set; }

        public void Attack()
        {
            if (Stat.GetAttributeValue(AttributeType.Durability) <= 10)
            {
                return;
            }

            switch (CurrentAttackBehavior)
            {
                case null:
                case DefaultAttackBehavior:
                case ColliderAttackBehavior when IsBusy:
                case HitScanSingleAttackBehavior when IsBusy:
                    return;
                default:
                    StartCoroutine(AttackCoroutine());
                    break;
            }
        }

        private IEnumerator AttackCoroutine()
        {
            EventManager.OnNext(Message.OnCharacterBusy);
            EventManager.OnNext(Message.OnCancelRunning);
            CanAttack = false;

            NormalizedMouseVector = (MainCamera.GetMouseWorldPosition() - PlayerPosition).normalized;
            CurrentAttackBehavior.Attack(
                Stat.GetAttributeValue(AttributeType.Attack) * Stat.GetAttributeValue(AttributeType.Durability)
                , (MainCamera.GetMouseWorldPosition() - transform.position).normalized
            );

            _animator.SetFloat(_hashLastMoveX, NormalizedMouseVector.x);
            _animator.SetFloat(_hashLastMoveY, NormalizedMouseVector.y);

            var elapsedTime = 0f;
            while (elapsedTime < CurrentAttackBehavior.attackDelay)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            EventManager.OnNext(Message.OnCharacterFree);

            elapsedTime = 0f;
            while (elapsedTime < CurrentAttackBehavior.coolTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            CanAttack = true;

        }

        #endregion

        private void InitWeaponStates()
        {
            _weaponStates[WeaponStateType.Default] = new WeaponState
            {
                State = new NoWeaponState(),
                AttackBehavior = new DefaultAttackBehavior()
            };

            _weaponStates[WeaponStateType.ShortDistance] = new WeaponState
            {
                State = new SDWeaponState(),
                AttackBehavior = new ColliderAttackBehavior(transform
                    , _attackCollider
                    , Constants.Character.PlayerColliderAttackAngle
                    , Constants.Character.PlayerColliderAttackDelay
                    , Constants.Character.PlayerColliderAttackCoolTime
                    , TargetMaskString
                )
            };

            _weaponStates[WeaponStateType.LongDistance] = new WeaponState
            {
                State = new LDWeaponState(),
                AttackBehavior = new HitScanSingleAttackBehavior(transform
                    , Constants.Character.PlayerHitScanAttackRange
                    , Constants.Character.PlayerHitScanAttackDelay
                    , Constants.Character.PlayerHitScanAttackCoolTime
                    , TargetMaskString
                )
            };
        }

        private void ChangeWeapon()
        {
            if (IsBusy)
            {
                return;
            }

            _currentWeaponType = (WeaponStateType)(((int)_currentWeaponType + 1) % 3);
            CurrentAttackBehavior = _weaponStates[_currentWeaponType].AttackBehavior;
            _stateMachine.ChangeState(_weaponStates[_currentWeaponType].State.GetType());
            Debug.Log($"[PlayerCharacter] ChangeWeapon(): current weapon is {_currentWeaponType}");
        }

        private void OnAttackSucceeded()
        {
            Stat.AddAttributeValue(AttributeType.Durability, -1);
        }
    }

    public sealed partial class PlayerCharacter : IDamageable
    {
        private bool _canDamage = true;
        private readonly WaitForSeconds _spriteWait = new(0.5f);

        public bool IsAlive => Stat.Attributes[AttributeType.Hp].ModifiedValue > 0;

        public void Damage(int damage)
        {
            if (!IsAlive || !_canDamage)
            {
                return;
            }

            Stat.AddAttributeValue(
                AttributeType.Hp
                , -Mathf.Max(0, damage - Stat.GetAttributeValue(AttributeType.Defense))
            );
            EventManager.OnNext(Message.OnCancelRunning);

            if (!IsAlive)
            {
                _render.DOComplete();
                _stateMachine.ChangeState<DeadState>();
                return;
            }

            _render.DOColor(Color.red, Constants.Character.DamageEffectDuration)
                .SetLoops(9, LoopType.Yoyo)
                .OnComplete(() => _render.DOColor(Color.white, Constants.Character.DamageEffectDuration));
        }
    }

    public sealed partial class PlayerCharacter : IAffectable
    {
        public void Affect(AttributeType type, int value)
        {
            Stat.AddAttributeValue(type, value);
        }

        public void Affect(Effect.Effect effect)
        {
            EventManager.OnNext(Message.OnPlayerAffected, effect);
        }

        public int GetReferenceValue(AttributeType type)
        {
            return Stat.Attributes[type].BaseValue;
        }
    }
}