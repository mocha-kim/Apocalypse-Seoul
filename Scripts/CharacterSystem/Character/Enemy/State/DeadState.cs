using CharacterSystem.Character.StateMachine;
using UnityEngine;

namespace CharacterSystem.Character.Enemy.State
{
    public class DeadState : State<EnemyCharacter>
    {
        private Animator _animator;

        private int _hashIsAlive = Animator.StringToHash("IsAlive");

        public override void OnInitialized()
        {
            _animator = Context.Animator;
        }

        public override void OnEnter()
        {
            if (_animator == null)
            {
                return;
            }
            _animator.SetBool(_hashIsAlive, false);
        }

        public override void Update(float deltaTime)
        {
            // wait until animation playing
            if (StateMachine.ElapsedTime > 3.0f)
            {
                Context.gameObject.SetActive(false);
            }
        }
    }
}