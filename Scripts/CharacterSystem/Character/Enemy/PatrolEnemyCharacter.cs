using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Stat;
using DataSystem;

namespace CharacterSystem.Character.Enemy
{
    public class PatrolEnemyCharacter : DefaultEnemyCharacter
    {
        protected override void Start()
        {
            base.Start();

            StateMachine.AddState(
                new PatrolState(Constants.Character.PatrolRange, transform.position
            ));
            StateMachine.Refresh();
        }
    }
}