using System;
using CharacterSystem.Character.StateMachine;
using DataSystem;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Character.Player.State
{
    public class DeadState : State<PlayerCharacter>
    {
        private bool _isInDeadProcess = false;
        private float _deadAnimationDelay = Constants.Delay.DeadAnimationDelay;
        
        private int _hashIsDead;

        public override void OnInitialized()
        {
            _hashIsDead = Animator.StringToHash("IsDead");
        }

        public override void OnEnter()
        {
            _isInDeadProcess = false;
            _deadAnimationDelay = Constants.Delay.DeadAnimationDelay;
            
            Animator.SetBool(_hashIsDead, true);
            
            EventManager.OnNext(Message.OnPlayerDead);
        }

        public override void Update(float deltaTime)
        {
            if (_isInDeadProcess)
            {
                return;
            }
            
            _deadAnimationDelay -= deltaTime;
            if (_deadAnimationDelay <= 0f)
            {
                _isInDeadProcess = true;
                EventManager.OnNext(Message.OnPlayerDead);
            }
        }
    }
}