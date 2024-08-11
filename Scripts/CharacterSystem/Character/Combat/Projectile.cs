using Core.Interface;
using DataSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSystem.Character.Combat
{
    public class Projectile : MonoBehaviour
    {
        private IObjectPool<Projectile> _managedPool;

        private Rigidbody2D _rigidbody2D;
        private Vector2 _direction;

        private int _damage;
        private int _speed;
        private Effect.Effect _effect = null;
        private Transform _context;

        private LayerMask _targetMask;
        private LayerMask _obstacleMask;

        private bool _isInitialized;
        private bool _isReleased;
        private float _destroyTime;

        #region MonoBehaviour
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _obstacleMask = Constants.Layer.Obstacle;
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                gameObject.SetActive(false);
            }

            _isReleased = false;
        }

        private void Update()
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + _direction * (Time.fixedDeltaTime * _speed));
            _destroyTime -= Time.fixedDeltaTime;

            if (_destroyTime <= 0)
            {
                Release();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == _obstacleMask)
            {
                Release();
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
            
            if (_effect == null || target.Affectable == null)
            {
                return;
            }
            target.Affectable.Affect(_effect);
            Release();
        }
        
        #endregion

        public void ShootProjectile(Vector2 direction, int damage, int speed, Effect.Effect effect, Transform context)
        {
            _direction = direction;
            _damage = damage;
            _speed = speed;
            _effect = effect;
            _context = context;

            transform.position = context.position;
            _destroyTime = Constants.Character.ProjectileValidTime;
        }

        public void Init(IObjectPool<Projectile> pool, int targetMask)
        {
            _managedPool = pool;
            _targetMask = targetMask;

            _isInitialized = true;
        }

        private void Release()
        {
            if (_isReleased)
            {
                return;
            }

            _isReleased = true;
            _managedPool.Release(this);
        }
    }
}