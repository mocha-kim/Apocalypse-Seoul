using System.Collections;
using System.Collections.Generic;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using DataSystem.Database;
using DG.Tweening;
using EnvironmentSystem.Camera;
using Event;
using Manager;
using UI;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace CharacterSystem.Character.Player
{
    public sealed partial class PlayerCharacter : Character
    {
        private PlayerStat Stat => DataManager.Stat;
        public override CharacterType CharacterType => CharacterType.Player;

        public Vector3 PlayerPosition => transform.position;
        public Vector2 NormalizedMouseVector { get; private set; } = Vector2.down;

        [SerializeField] private AttackCollider _attackCollider;
        private bool _isBusy = false;
            
        [SerializeField] private Animator _animator;
        private int _hashLastMoveX;
        private int _hashLastMoveY;
        private int _hashAttackIndex;
        private int _hashIsConsuming;
        private int _hashConsumeItemId;

        #region MonoBehaviour

        private void Awake()
        {
            _hashLastMoveX = Animator.StringToHash("LastMoveX");
            _hashLastMoveY = Animator.StringToHash("LastMoveY");
            _hashAttackIndex = Animator.StringToHash("AttackIndex");
            _hashIsConsuming = Animator.StringToHash("IsConsuming");
            _hashConsumeItemId = Animator.StringToHash("ConsumeItemId");

            EventManager.Subscribe(gameObject, Message.OnCharacterBusy, _ => _isBusy = true);
            EventManager.Subscribe(gameObject, Message.OnCharacterFree, _ => _isBusy = false);

            EventManager.Subscribe(gameObject, Message.OnLeftMouseDown, _ => OnLeftMouseDown());
            EventManager.Subscribe(gameObject, Message.OnPressChangeWeapon, _ => ChangeWeapon());

            EventManager.Subscribe(gameObject, Message.OnRightMouseDown, _ => StartAiming());
            EventManager.Subscribe(gameObject, Message.OnRightMouseUp, _ => EndAiming());

            EventManager.Subscribe(gameObject, Message.OnTryItemUse, OnTryItemUse);
        }

        private void Start()
        {
            InitAttackBehavior();
            InitPainter();
        }

        private void Update()
        {
            if (!_isAiming)
            {
                return;
            }

            SetAnimationDirByMouseVector();
        }

        #endregion

        private void ChangeWeapon()
        {
            if (_isBusy)
            {
                return;
            }

            _attackIndex = (_attackIndex + 1) % 3;
            _animator.SetInteger(_hashAttackIndex, _attackIndex);

            CurrentAttackBehavior = _attackBehaviors[_attackIndex];
            Debug.Log(CurrentAttackBehavior);
        }

        private void OnLeftMouseDown()
        {
            Attack();
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

    public sealed partial class PlayerCharacter : IAttackable
    {
        private int _attackIndex;
        private List<AttackBehavior> _attackBehaviors = new();
        private static string[] _targetMaskString = { Constants.Layer.EnemyString };

        [SerializeField] private GameObject _playerAttackPainter;

        private bool _isAiming = false;

        #region IAttackble

        public AttackBehavior CurrentAttackBehavior { get; private set; }

        public void Attack()
        {
            switch (CurrentAttackBehavior)
            {
                case null:
                case DefaultAttackBehavior:
                case ColliderAttackBehavior when _isBusy:
                case HitScanSingleAttackBehavior when _isBusy:
                    return;
                default:
                    AttackWithDelay();
                    break;
            }
        }

        #endregion
        
        private void InitAttackBehavior()
        {
            _attackBehaviors.Add(new DefaultAttackBehavior());
            _attackBehaviors.Add(
                new ColliderAttackBehavior(transform
                    , _attackCollider
                    , Constants.Character.PlayerColliderAttackAngle
                    , Constants.Character.PlayerColliderAttackDelay
                    , Constants.Character.PlayerColliderAttackCoolTime
                    , _targetMaskString)
            );
            _attackBehaviors.Add(
                new HitScanSingleAttackBehavior(transform
                    , Constants.Character.PlayerHitScanAttackRange
                    , Constants.Character.PlayerHitScanAttackDelay
                    , Constants.Character.PlayerHitScanAttackCoolTime
                    , _targetMaskString)
            );

            CurrentAttackBehavior = _attackBehaviors[0];
        }

        private void InitPainter()
        {
            _playerAttackPainter.GetComponent<PlayerAttackPainter>().Init(_targetMaskString);
            _playerAttackPainter.SetActive(false);
        }

        private void StartAiming()
        {
            if (_isBusy || CurrentAttackBehavior is not HitScanSingleAttackBehavior)
            {
                return;
            }
            _isAiming = true;
            EventManager.OnNext(Message.OnCharacterBusy);
            _playerAttackPainter.SetActive(true);
        }

        private void EndAiming()
        {
            if (CurrentAttackBehavior is not HitScanSingleAttackBehavior)
            {
                return;
            }
            _isAiming = false;
            EventManager.OnNext(Message.OnCharacterFree);
            _playerAttackPainter.SetActive(false);
        }
        
        private void SetAnimationDirByMouseVector()
        {
            if (MouseData.MouseHoveredInventory != null)
            {
                return;
            }
            
            NormalizedMouseVector = (MainCamera.GetMouseWorldPosition() - PlayerPosition).normalized;
            
            _animator.SetFloat(_hashLastMoveX, NormalizedMouseVector.x);
            _animator.SetFloat(_hashLastMoveY, NormalizedMouseVector.y);
        }

        private void AttackWithDelay()
        {
            NormalizedMouseVector = (MainCamera.GetMouseWorldPosition() - PlayerPosition).normalized;

            _animator.SetFloat(_hashLastMoveX, NormalizedMouseVector.x);
            _animator.SetFloat(_hashLastMoveY, NormalizedMouseVector.y);

            CurrentAttackBehavior.Attack(
                Stat.GetAttributeValue(AttributeType.Attack)
                , (MainCamera.GetMouseWorldPosition() - transform.position).normalized
            );
        }
    }

    public sealed partial class PlayerCharacter : IDamageable
    {
        private bool _canDamage = true;
        private readonly WaitForSeconds _spriteWait = new(0.5f);

        #region IDamagable

        public bool IsAlive => Stat.Attributes[AttributeType.Hp].ModifiedValue > 0;

        public void Damage(int damage)
        {
            if (!IsAlive || !_canDamage)
            {
                return;
            }
            Stat.AddAttributeValue(AttributeType.Hp, -damage);
            _render.DOColor(Color.red, Constants.Character.DamageEffectDuration)
                .SetLoops(9, LoopType.Yoyo)
                .OnComplete(() => _render.DOColor(Color.white, Constants.Character.DamageEffectDuration));
        }
        
        #endregion
    }

    public sealed partial class PlayerCharacter : IAffectable
    {
        #region IAffectable

        public void Affect(AttributeType type, int value)
        {
            Stat.AddAttributeValue(type, value);
        }

        public int GetReferenceValue(AttributeType type)
        {
            return Stat.Attributes[type].BaseValue;
        }

        #endregion
    }
}