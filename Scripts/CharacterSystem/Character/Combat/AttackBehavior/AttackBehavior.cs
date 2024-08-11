using System;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Combat.AttackBehavior
{
    [Serializable]
    public abstract class AttackBehavior : IHasRange
    {
        public int animationIndex;
        public int priority;

        public Transform context;

        // seconds basis
        public float attackDelay = 1f;
        public float coolTime = 2f;
        
        protected LayerMask ObstacleMask = LayerMask.GetMask("Obstacle");
        protected LayerMask TargetMask;
        
        public abstract float Range { get; }
        protected Effect.Effect Effect = null;

        protected AttackBehavior(Transform context, float attackDelay = 1f, float coolTime = 2f,
            string[] targetMaskStrings = null)
        {
            this.context = context;

            this.attackDelay = attackDelay;
            this.coolTime = coolTime;
            if (targetMaskStrings != null)
            {
                TargetMask = LayerMask.GetMask(targetMaskStrings);
            }
        }

        public Vector3 StartPoint => context.position;

        public virtual void SetEffect(Effect.Effect effect)
        {
            Effect = effect;
        }

        public abstract void Attack(int damage, Vector3 attackDirection = default);
    }
}