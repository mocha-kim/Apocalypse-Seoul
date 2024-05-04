using System;
using Core.Interface;

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
            foreach (var pair in EffectValues)
            {
                newEffect.EffectValues[pair.Key] = pair.Value;
            }
            return newEffect;
        }

        public override void EnableEffect(IAffectable target, EffectController controller)
        {
            base.EnableEffect(target, controller);
            
            foreach (var type in EffectValues.Keys)
            {
                EffectValues[type].CalculateEffectValue(target.GetReferenceValue(type));
                Target.Affect(type, EffectValues[type].EffectedValue);
            }
            
            DisableEffect();
        }
    }
}