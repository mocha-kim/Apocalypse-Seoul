using UnityEngine;

namespace Core.Interface
{
    public interface IDamageable
    {
        public bool IsAlive { get; }
        public void Damage(int damage);

        public void SetTarget(Transform trans)
        {
        }
    }
}