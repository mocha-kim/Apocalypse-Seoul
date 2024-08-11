using UnityEngine;

namespace CharacterSystem.Character.StateMachine
{
    public abstract class State<T>
    {
        protected StateMachine<T> StateMachine;
        protected T Context;
        protected Animator Animator = null;

        public State()
        {
        }

        public void Init(StateMachine<T> stateMachine, T context, Animator animator = null)
        {
            StateMachine = stateMachine;
            Context = context;
            Animator = animator;

            OnInitialized();
        }

        public virtual void OnInitialized()
        {
        }

        public virtual void OnEnter()
        {
        }

        public abstract void Update(float deltaTime);

        public virtual void FixedUpdate(float deltaTime)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}