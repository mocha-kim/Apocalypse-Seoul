using System;
using CharacterSystem.Stat;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Effect
{
    [Serializable]
    public class InstantEffect : Effect
    {
        public InstantEffect(int id, EffectType type) : base(id, type, 0)
        {
            IsPermanent = false;
            IsEnabled = false;

            duration = 0;
        }

        public override object Clone()
        {
            var newEffect = new InstantEffect(ID, Type);
            newEffect.AttributeType = AttributeType;
            newEffect.AttributeValue = new EffectValue(AttributeValue);
            return newEffect;
        }

        public override void EnableEffect(IAffectable target, EffectController controller)
        {
            if (target == null || AttributeType == AttributeType.None)
            {
                Debug.Log($"[InstantEffect] EnableEffect(): Cannot enable effect {ID}, {Type}");
                return;
            }
            
            base.EnableEffect(target, controller);

            AttributeValue.CalculateEffectValue(target.GetReferenceValue(AttributeType));
            target.Affect(AttributeType, AttributeValue.EffectedValue);
            
            DisableEffect();
        }
    }
}