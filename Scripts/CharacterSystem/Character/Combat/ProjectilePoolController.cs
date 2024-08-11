using System;
using DataSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSystem.Character.Combat
{
    public class ProjectilePoolController : MonoBehaviour
    {
        private ObjectPool<Projectile> _projectilePool;
        private GameObject _bulletPrefab;

        private int _targetMask;
        
        private void Awake()
        {
            _projectilePool = new ObjectPool<Projectile>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 10);
        }

        private void Start()
        {
            _bulletPrefab = ResourceManager.GetPrefab("Bullet");
        }

        public void Init(int targetMask)
        {
            _targetMask = targetMask;
        }

        public void Shoot(Vector2 direction, int damage, int speed, Effect.Effect effect, Transform context)
        {
            var projectile = _projectilePool.Get();
            projectile.ShootProjectile(direction, damage, speed, effect, context);
        }

        private Projectile CreateBullet()
        {
            Projectile enemyProjectile = Instantiate(_bulletPrefab).GetComponent<Projectile>();
            enemyProjectile.Init(_projectilePool, _targetMask);
            return enemyProjectile;
        }

        private void OnGetBullet(Projectile enemyProjectile)
        {
            enemyProjectile.gameObject.SetActive(true);
        }

        private void OnReleaseBullet(Projectile enemyProjectile)
        {
            enemyProjectile.gameObject.SetActive(false);
        }

        private void OnDestroyBullet(Projectile enemyProjectile)
        {
            Destroy(enemyProjectile.gameObject);
        }
    }
}