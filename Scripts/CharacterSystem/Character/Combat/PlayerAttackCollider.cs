using EventSystem;

namespace CharacterSystem.Character.Combat
{
    public class PlayerAttackCollider : AttackCollider
    {
        protected override void OnTargetDamaged()
        {
            EventManager.OnNext(Message.OnEnemyDamaged);
        }
    }
}