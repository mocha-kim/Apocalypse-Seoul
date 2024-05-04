using System;
using System.Collections;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    public sealed class ColliderAttackBehavior : AttackBehavior
    {
        private float _angle;
        
        private AttackCollider _attackCollider;
        private GameObject _colliderObject;

        public override float Range => _attackCollider.Size.y;

        public ColliderAttackBehavior(Transform context, AttackCollider attackCollider, float angle
            , float attackDelay = 1f, float coolTime = 2f, string[] targetMaskStrings = null)
            : base(context, attackDelay, coolTime, targetMaskStrings)
        {
            _angle = angle;
            _attackCollider = attackCollider;
            _attackCollider.Init(context, angle, attackDelay, targetMaskStrings);
            _colliderObject = _attackCollider.gameObject;
        }

        public override void Attack(int damage, Vector3 attackDirection = default)
        {
            _attackCollider.Attack(damage, attackDirection);
            _colliderObject.SetActive(true);
        }
    }
}