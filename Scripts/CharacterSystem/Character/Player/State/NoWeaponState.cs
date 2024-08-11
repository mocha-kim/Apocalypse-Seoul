using System;
using CharacterSystem.Character.StateMachine;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Character.Player.State
{
    public class NoWeaponState : State<PlayerCharacter>
    {
        private const WeaponStateType WeaponStateType = State.WeaponStateType.Default;
        
        private int _hashAttackIndex;
        
        public override void OnInitialized()
        {
            _hashAttackIndex = Animator.StringToHash("AttackIndex");
        }

        public override void OnEnter()
        {
            Animator.SetInteger(_hashAttackIndex, (int)WeaponStateType);
        }
        
        public override void Update(float deltaTime)
        {
        }
    }
}