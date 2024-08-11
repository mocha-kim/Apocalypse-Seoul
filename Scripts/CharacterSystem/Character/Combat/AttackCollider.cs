using System.Collections;
using UnityEngine;

namespace CharacterSystem.Character.Combat
{
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private bool _isSingleAttack = true;
        private Transform _context;
        private int _targetMask;

        private float _attackAngle;
        private float _attackDelay;
        private int _damage;
        private Effect.Effect _effect = null;

        private int _attackNumber;
        private int _maxAttackNumber = 1;

        private bool _hasTriggered;
        private bool _isInitialized;

        private BoxCollider2D _collider;
        public Vector2 Size => _collider.size;

        #region MonoBehaviour

        private void Awake()
        {
            _collider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            if (!_isInitialized || _collider == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _hasTriggered = false;
            _attackNumber = _maxAttackNumber;
            StartCoroutine(RotateDuringDelay());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isSingleAttack && _hasTriggered)
            {
                return;
            }

            if (_attackNumber <= 0)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & _targetMask) <= 0)
            {
                return;
            }

            if (!other.TryGetComponent(out HitBox target))
            {
                return;
            }

            if (target.Damageable == null)
            {
                return;
            }

            target.Damageable.Damage(_damage);
            target.Damageable.SetTarget(_context);
            OnTargetDamaged();
            _attackNumber -= 1;

            if (_effect == null || target.Affectable == null)
            {
                return;
            }

            target.Affectable.Affect(_effect);
            _hasTriggered = true;
        }

        private void OnDisable()
        {
            _hasTriggered = false;
            StopAllCoroutines();
        }

        #endregion

        public void Init(Transform context, float attackAngle, float attackDelay, string[] targetMaskStrings)
        {
            _context = context;
            _attackAngle = attackAngle;
            _attackDelay = attackDelay;
            _targetMask = LayerMask.GetMask(targetMaskStrings);

            _isInitialized = true;
        }

        public void SetEffect(Effect.Effect effect)
        {
            _effect = effect;
        }

        public void Attack(int damage, Vector3 attackDirection)
        {
            _damage = damage;

            var degree = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, degree - 90f);
        }

        protected virtual void OnTargetDamaged()
        {
        }

        private IEnumerator RotateDuringDelay()
        {
            var elapsedTime = 0f;
            while (elapsedTime < _attackDelay)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}