using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSystem.Character;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using EventSystem;
using UnityEngine;

namespace CharacterSystem.Effect
{
    [Serializable]
    public abstract class Effect : ICloneable
    {
        public readonly int ID;
        public readonly EffectType Type;
        public bool IsPermanent { get; protected set; } = false;
        public bool IsEnabled { get; protected set; } = false;

        public int duration;
        public readonly int coolTime;

        protected IAffectable Target;
        protected AttributeType AttributeType = AttributeType.None;
        protected EffectValue AttributeValue = null;

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
                CorrectionValue = null;
                EffectedValue = value;
                IsFloat = false;
            }

            public EffectValue(EffectValue other)
            {
                IsFloat = other.IsFloat;
                CorrectionValue = other.CorrectionValue;
                EffectedValue = other.EffectedValue;
            }

            public void CalculateEffectValue(int referenceValue)
            {
                if (!IsFloat || CorrectionValue == null)
                {
                    return;
                }

                EffectedValue = Mathf.RoundToInt((float)(referenceValue * CorrectionValue));
            }

            public override string ToString()
            {
                return $"EffectValue: isFloat({IsFloat}), correctionValue({CorrectionValue}), effectedValue({EffectedValue})";
            }
        }

        public void SetEffect(AttributeType type, int value)
        {
            if (AttributeType != AttributeType.None)
            {
                Debug.LogWarning($"[Effect] SetEffect(): This effect(id: {ID}) already set. It will be overwritten.");
            }

            AttributeType = type;
            AttributeValue = new EffectValue(value);
        }

        public void SetEffect(AttributeType type, float value)
        {
            if (AttributeType != AttributeType.None)
            {
                Debug.LogWarning($"[Effect] SetEffect(): This effect(id: {ID}) already set. It will be overwritten.");
            }

            AttributeType = type;
            AttributeValue = new EffectValue(value);
        }

        public abstract object Clone();

        public virtual void EnableEffect(IAffectable target, EffectController controller)
        {
            if (target == null || AttributeType == AttributeType.None)
            {
                Debug.Log($"[Effect] EnableEffect(): Cannot enable effect {ID}, {Type}");
                return;
            }
            
            Target = target;
            IsEnabled = true;

            if (Target?.CharacterType == CharacterType.Player)
            {
                EventManager.OnNext(Message.OnPlayerEffectChanged);
            }
        }

        public virtual void DisableEffect()
        {
            if (Target == null || !IsEnabled)
            {
                return;
            }

            if (Target.CharacterType == CharacterType.Player)
            {
                EventManager.OnNext(Message.OnPlayerEffectChanged);
            }
            
            Target = null;
            IsEnabled = false;
        }

        public KeyValuePair<AttributeType, string> GetEffectInfo()
        {
            if (AttributeValue.IsFloat)
            {
                return new KeyValuePair<AttributeType, string>(
                    AttributeType,
                    (AttributeValue.CorrectionValue >= 0 ? " + " : " - ") + AttributeValue.CorrectionValue + "%\n"
                    );
            }

            return new KeyValuePair<AttributeType, string>(
                AttributeType,
                (AttributeValue.EffectedValue >= 0 ? " + " : " - ") + AttributeValue.EffectedValue + "\n"
            );
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

        public override string ToString()
        {
            return $"Effect: id({ID}), type({Type}), isPermanent({IsPermanent}), isEnabled({IsEnabled}), " +
                   $"duration({duration}), coolTime({coolTime}), " +
                   $"target({Target}), attribute({AttributeType}, value({AttributeValue})";
        }
    }
}