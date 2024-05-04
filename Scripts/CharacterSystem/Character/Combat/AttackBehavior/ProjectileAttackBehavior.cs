using System;
using DataSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public class ProjectileAttackBehavior : AttackBehavior
    {
        public int speed;
        
        private IObjectPool<Projectile> _projectilePool;
        public override float Range => Constants.Character.ProjectileValidRange;

        public ProjectileAttackBehavior(Transform context, int damage = 10, float range = 3f,
            float attackDelay = 1f, float coolTime = 2f, string[] targetMaskStrings = null, int speed = 1,
            IObjectPool<Projectile> enemyProjectilePool = null)
            : base(context, attackDelay, coolTime, targetMaskStrings)
        {
            this.speed = speed;
            _projectilePool = enemyProjectilePool;
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
            var bulletInstance = _projectilePool.Get();
            bulletInstance.ShootProjectile(attackDirection, damage, speed, context);
            bulletInstance.transform.position = StartPoint;
        }
    }
}