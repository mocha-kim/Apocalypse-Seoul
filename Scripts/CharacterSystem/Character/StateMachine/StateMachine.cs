using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystem.Character.StateMachine
{
    public sealed class StateMachine<T>
    {
        private readonly T _context;
        private Animator _animator;
        
        private Dictionary<System.Type, State<T>> _states = new();

        public State<T> Currentstate { get; private set; }
        public State<T> PriviousState { get; private set; }
        public float ElapsedTime { get; private set; } = 0.0f;


        public StateMachine(T context, State<T> initialState, Animator animator = null)
        {
            _context = context;
            _animator = animator;

            AddState(initialState);
            Currentstate = initialState;
            Currentstate.OnEnter();
        }

        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;

            Currentstate.Update(deltaTime);
        }
        
        public void FixedUpdate()
        {
            Currentstate.FixedUpdate(Time.fixedDeltaTime);
        }

        public bool HasState<TState>() where TState : State<T>
        {
            var type = typeof(TState);
            return _states.ContainsKey(type);
        }

        public void AddState(State<T> state)
        {
            state.Init(this, _context, _animator);
            _states[state.GetType()] = state;
        }

        public void ChangeState<TState>() where TState : State<T>
        {
            var newType = typeof(TState);
            ChangeState(newType);
        }
        
        public void ChangeState(Type newType)
        {
            if (!_states.ContainsKey(newType))
            {
                Debug.LogError($"[StateMachine] ChangeState(): State of type {newType} has not been added to the state machine");
                return;
            }
            
            if (Currentstate.GetType() == newType)
            {
                return;
            }

            if (Currentstate != null)
            {
                Currentstate.OnExit();
            }

            PriviousState = Currentstate;
            Currentstate = _states[newType];
            Currentstate.OnEnter();

            ElapsedTime = 0.0f;
        }

        public void Refresh()
        {
            Currentstate.OnExit();
            Currentstate.OnEnter();
        }
    }
}