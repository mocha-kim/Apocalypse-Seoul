namespace CharacterSystem.Character.StateMachine
{
    public abstract class State<T>
    {
        protected StateMachine<T> StateMachine;
        protected T Context;

        public State()
        {
        }

        public void SetStateMachineAndContext(StateMachine<T> stateMachine, T context)
        {
            StateMachine = stateMachine;
            Context = context;

            OnInitialized();
        }

        public virtual void OnInitialized()
        {
        }

        public virtual void OnEnter()
        {
        }

        public abstract void Update(float deltaTime);

        public virtual void FixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}