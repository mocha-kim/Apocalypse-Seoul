using System.Collections;
using System.Collections.Generic;
using Alpha;
using Core.Interface;
using DataSystem;
using DataSystem.Database;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Effect
{
    public class EffectController : MonoBehaviour
    {
        private IAffectable _self;
        private Dictionary<EffectType, Effect> _effects = new();

        protected virtual void Awake()
        {
            _self = GetComponent<IAffectable>();

            EventManager.Subscribe(gameObject, Message.OnItemUsed, OnUseItem);
        }

        public void EnableEffect(Effect newEffect)
        {
            switch (newEffect)
            {
                case null:
                    Debug.LogWarning("[EffectController] EnableEffect(): newEffect is null");
                    return;
                case InstantEffect:
                    newEffect.EnableEffect(_self, this);
                    return;
                default:
                {
                    if (_effects.TryGetValue(newEffect.Type, out var effect))
                    {
                        if (effect.IsEnabled)
                        {
                            effect.duration += newEffect.duration;
                        }
                        else
                        {
                            effect.EnableEffect(_self, this);
                            effect.duration = newEffect.duration;
                        }
                        return;
                    }

                    _effects.Add(newEffect.Type, (Effect)newEffect.Clone());
                    effect = _effects[newEffect.Type];
                    effect.EnableEffect(_self, this);
                    return;
                }
            }
        }

        public void EnableEffect(EffectType type)
        {
            if (type == EffectType.ItemEffect)
            {
                Debug.LogWarning("[EffectController] EnableEffect(): Enable effect by type(ItemEffect) is impossible");
                return;
            }
            
            EnableEffect(Database.GetEffect(type));
        }

        public void DisableEffect(EffectType type)
        {
            if (!_effects.TryGetValue(type, out var effect) || !effect.IsEnabled)
            {
                return;
            }
            
            effect.DisableEffect();
        }

        private void OnUseItem(EventManager.Event e)
        {
            List<int> effectIDs = null;
            try
            {
                effectIDs = (List<int>)e.Args[0];
            }
            catch
            {
                Debug.Log("Invalid Event Argument");
            }

            if (effectIDs == null)
            {
                return;
            }
            foreach (var id in effectIDs)
            {
                EnableEffect(Database.GetEffect(id));
            }
        }
    }
}