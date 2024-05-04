using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;

namespace Core.Interface
{
    public interface IAttackable
    {
        public AttackBehavior CurrentAttackBehavior { get; }
        public void Attack();
    }
}