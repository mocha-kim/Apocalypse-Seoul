using System;
using System.Collections;
using Core.Interface;
using EnvironmentSystem.Camera;
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

        private bool _hasTriggered;
        private bool _isInitialized;

        private BoxCollider2D _collider;
        public Vector2 Size => _collider.size;

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
            StartCoroutine(RotateDuringDelay());
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isSingleAttack && _hasTriggered)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & _targetMask) <= 0)
            {
                return;
            }

            var damageable = other.gameObject.GetComponent<IDamageable>() ?? other.transform.parent.GetComponent<IDamageable>();
            if (damageable == null)
            {
                return;
            }
            damageable.Damage(_damage);
            damageable.SetTarget(_context);
            _hasTriggered = true;
        }

        private void OnDisable()
        {
            _hasTriggered = false;
            StopAllCoroutines();
        }

        public void Init(Transform context, float attackAngle, float attackDelay, string[] targetMaskStrings)
        {
            _context = context;
            _attackAngle = attackAngle;
            _attackDelay = attackDelay;
            _targetMask = LayerMask.GetMask(targetMaskStrings);

            _isInitialized = true;
        }

        public void Attack(int damage, Vector3 attackDirection)
        {
            _damage = damage;
            
            var degree = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg + _attackAngle / 2;
            transform.localRotation = Quaternion.Euler(0f, 0f, degree - 90f);
        }

        private IEnumerator RotateDuringDelay()
        {
            var elapsedTime = 0f;
            while (elapsedTime < _attackDelay)
            {
                elapsedTime += Time.deltaTime;
                transform.Rotate( Vector3.forward, -_attackAngle / _attackDelay * Time.deltaTime);
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}