using System;
using System.Collections;
using System.Collections.Generic;
using Alpha;
using CharacterSystem.Character;
using CharacterSystem.Stat;
using Core;
using Core.Interface;
using DataSystem;
using Event;
using Manager;
using UnityEngine;

namespace CharacterSystem.Effect
{
    [Serializable]
    public abstract class Effect : ICloneable
    {
        public readonly int ID;
        public readonly EffectType Type;
        public bool IsPermanent { get; protected set; }
        public bool IsEnabled { get; protected set; }

        public int duration;
        public readonly int coolTime;

        protected IAffectable Target;
        protected Dictionary<AttributeType, EffectValue> EffectValues = new();

        protected Effect(int id, EffectType type, int coolTime)
        {
            ID = id;
            Type = type;
            this.coolTime = coolTime;
        }

        protected class EffectValue
        {
            public float? CorrectionValue { get; private set; }
            public int EffectedValue { get; private set; }
            public bool IsFloat { get; private set; }

            // multiplication effect
            public EffectValue(float value)
            {
                CorrectionValue = value;
                EffectedValue = 0;
                IsFloat = true;
            }

            // sum effect
            public EffectValue(int value)
            {
                EffectedValue = value;
                IsFloat = false;
            }

            public void CalculateEffectValue(int referenceValue)
            {
                if (CorrectionValue == null)
                {
                    return;
                }

                EffectedValue = Mathf.RoundToInt((float)(referenceValue * CorrectionValue));
            }
        }

        public void AddEffect(AttributeType type, int value)
        {
            if (EffectValues.ContainsKey(type))
            {
                Debug.LogWarning($"{Type}: Already exist attribute({type}). It will be overwritten.");
            }

            EffectValues[type] = new EffectValue(value);
        }

        public void AddEffect(AttributeType type, float value)
        {
            if (EffectValues.ContainsKey(type))
            {
                Debug.LogWarning($"{Type}: Already exist attribute({type}). It will be overwritten.");
            }

            EffectValues[type] = new EffectValue(value);
        }

        public abstract object Clone();

        public virtual void EnableEffect(IAffectable target, EffectController controller)
        {
            if (target == null || EffectValues.Count < 1)
            {
                Debug.Log("Effect: Cannot enable effect");
                return;
            }

            Target = target;
            IsEnabled = true;

            if (Target.CharacterType == CharacterType.Player)
            {
                EventManager.OnNext(Message.OnPlayerEffectChanged);
            }
        }

        public virtual void DisableEffect()
        {
            if (Target == null)
            {
                return;
            }

            IsEnabled = false;
        }

        public List<KeyValuePair<AttributeType, string>> GetEffectInfo()
        {
            var list = new List<KeyValuePair<AttributeType, string>>();
            foreach (var (key, value) in EffectValues)
            {
                if (value.IsFloat)
                {
                    if (value.CorrectionValue == 0)
                    {
                        continue;
                    }

                    list.Add(new KeyValuePair<AttributeType, string>(
                        key,
                        (value.CorrectionValue > 0 ? " + " : " - ") + value.CorrectionValue + "%\n")
                    );
                }
                else
                {
                    if (value.EffectedValue == 0)
                    {
                        continue;
                    }

                    list.Add(new KeyValuePair<AttributeType, string>(
                        key,
                        (value.EffectedValue > 0 ? " + " : " - ") + value.EffectedValue + "%\n")
                    );
                }
            }

            return list;
        }

        protected IEnumerator StartDurationTimer()
        {
            var elapsedTime = 0f;
            // why WaitForSecond it not in use : duration can be changed while effect is enabled
            while (elapsedTime < Constants.Time.SecondsPerMinute * duration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            DisableEffect();
        }
    }
}