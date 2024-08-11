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
        
        private ProjectilePoolController _controller;
        public override float Range => Constants.Character.ProjectileValidRange;

        public ProjectileAttackBehavior(Transform context, int damage = 10, float range = 3f,
            float attackDelay = 1f, float coolTime = 2f, string[] targetMaskStrings = null, int speed = 1,
            ProjectilePoolController projectileController = null)
            : base(context, attackDelay, coolTime, targetMaskStrings)
        {
            this.speed = speed;
            _controller = projectileController;
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
            _controller.Shoot(attackDirection, damage, speed, Effect, context);
        }
    }
}