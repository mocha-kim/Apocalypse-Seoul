using System;
using CharacterSystem.Effect;
using CharacterSystem.Stat;
using DataSystem;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Character.Player
{
    public class PlayerEffectController : EffectController
    {
        protected override void Awake()
        {
            base.Awake();
            
            EventManager.Subscribe(gameObject, Message.OnPlayerAttributeChanged, _ => TracePlayerAttribute());
            EventManager.Subscribe(gameObject, Message.OnPlayerAffected, Affect);
        }

        private void Start()
        {
            EnableEffect(EffectType.DefaultHunger);
            EnableEffect(EffectType.DefaultThirst);
            
            TracePlayerAttribute();
        }

        private void TracePlayerAttribute()
        {
            TraceHunger();
            TraceThirst();
        }

        private void TraceHunger()
        {
            var thirst = DataManager.Stat.Attributes[AttributeType.Thirst].ModifiedValue;
            var hunger = DataManager.Stat.Attributes[AttributeType.Hunger].ModifiedValue;
            switch (hunger)
            {
                case >= Constants.Effect.FullReferenceValue:
                    if (thirst <= Constants.Effect.ThirstyReferenceValue)
                    {
                        break;
                    }
                    DisableEffect(EffectType.Starving);
                    EnableEffect(EffectType.Full);
                    EnableEffect(EffectType.Moderate);
                    break;
                case >= Constants.Effect.ModerateReferenceValue:
                    if (thirst <= Constants.Effect.ThirstyReferenceValue)
                    {
                        break;
                    }
                    DisableEffect(EffectType.Full);
                    DisableEffect(EffectType.Starving);
                    EnableEffect(EffectType.Moderate);
                    break;
                case <= Constants.Effect.StarvingReferenceValue:
                    DisableEffect(EffectType.Full);
                    DisableEffect(EffectType.Moderate);
                    EnableEffect(EffectType.Starving);
                    break;
                default:
                    DisableEffect(EffectType.Full);
                    DisableEffect(EffectType.Moderate);
                    DisableEffect(EffectType.Starving);
                    break;
            }
        }

        private void TraceThirst()
        {
            switch (DataManager.Stat.Attributes[AttributeType.Thirst].ModifiedValue)
            {
                case >= Constants.Effect.HydratedReferenceValue:
                    DisableEffect(EffectType.Thirsty);
                    EnableEffect(EffectType.Hydrated);
                    break;
                case <= Constants.Effect.ThirstyReferenceValue:
                    DisableEffect(EffectType.Hydrated);
                    EnableEffect(EffectType.Thirsty);
                    break;
                default:
                    DisableEffect(EffectType.Hydrated);
                    DisableEffect(EffectType.Thirsty);
                    break;
            }
        }

        private void Affect(EventManager.Event e)
        {
            Effect.Effect effect;
            try
            {
                effect = (Effect.Effect)e.Args[0];
            }
            catch
            {
                Debug.LogError("[PlayerEffectController] Affect(): Invalid Event Argument");
                return;
            }
            
            EnableEffect(effect);
        }
    }
}