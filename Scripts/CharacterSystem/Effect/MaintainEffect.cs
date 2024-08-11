using CharacterSystem.Stat;
using Core.Interface;
using UnityEngine;

namespace CharacterSystem.Effect
{
    public class MaintainEffect : Effect
    {
        private EffectController _controller;
        private Coroutine _durationCoroutine;
        
        public MaintainEffect(int id, EffectType type, int duration, int coolTime) : base(id, type, coolTime)
        {
            IsPermanent = duration <= 0;
            IsEnabled = false;

            this.duration = IsPermanent ? 0 : duration;
        }

        public MaintainEffect(int id, EffectType type, int coolTime) : base(id, type, coolTime)
        {
            IsPermanent = true;
            IsEnabled = false;

            duration = 0;
        }

        public override object Clone()
        {
            var newEffect = IsPermanent
                ? new MaintainEffect(ID, Type, coolTime)
                : new MaintainEffect(ID, Type, duration, coolTime);
            newEffect.AttributeType = AttributeType;
            newEffect.AttributeValue = new EffectValue(AttributeValue);

            return newEffect;
        }

        public override void EnableEffect(IAffectable target, EffectController controller)
        {
            if (target == null || AttributeType == AttributeType.None)
            {
                Debug.Log($"[MaintainEffect] EnableEffect(): Cannot enable effect {ID}, {Type}");
                return;
            }

            _controller = controller;
            base.EnableEffect(target, controller);
            
            if (!IsPermanent)
            {
                _durationCoroutine = controller.StartCoroutine(StartDurationTimer());
            }
            
            AttributeValue.CalculateEffectValue(target.GetReferenceValue(AttributeType));
            target.Affect(AttributeType, AttributeValue.EffectedValue);
        }

        public override void DisableEffect()
        {
            if (Target == null || !IsEnabled)
            {
                return;
            }

            var target = Target;
            base.DisableEffect();
            
            if (!IsPermanent)
            {
                _controller.StopCoroutine(_durationCoroutine);
            }
            _controller = null;
            
            target.Affect(AttributeType, -AttributeValue.EffectedValue);
        }
    }
}