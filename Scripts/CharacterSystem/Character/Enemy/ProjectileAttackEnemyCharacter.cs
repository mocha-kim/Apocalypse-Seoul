using CharacterSystem.Character.Combat.AttackBehavior;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSystem.Character.Enemy
{
    public class ProjectileAttackEnemyCharacter : PatrolEnemyCharacter
    {
        [SerializeField] private GameObject _bulletPrefab;
        private IObjectPool<Projectile> _bulletPool;

        protected override void Start()
        {
            base.Start();

            _bulletPool = new ObjectPool<Projectile>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, maxSize: 100);
            _attackBehaviors.Add(
                new ProjectileAttackBehavior(transform
                    , 10
                    , 10f
                    , 0f
                    , 2f
                    , targetMaskStrings
                    , 5,
                    _bulletPool)
            );
            CurrentAttackBehavior = _attackBehaviors[0];

            InitEnemyStat(100, 1, 10, 0, 0, 10);
        }

        public override void SwitchToIdleFOV() => FOV.SetViewFactors(4f, 120f, targetMaskStrings);
        public override void SwitchToDetectionFov() => FOV.SetViewFactors(8f, 360f, targetMaskStrings);

        #region Pooling

        private Projectile CreateBullet()
        {
            Projectile enemyProjectile = Instantiate(_bulletPrefab).GetComponent<Projectile>();
            enemyProjectile.Init(_bulletPool, targetMaskStrings);
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

        #endregion
    }
}