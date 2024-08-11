using System.Collections;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using UnityEngine;

namespace CharacterSystem.Effect
{
    public class DotEffect : Effect
    {
        private EffectController _controller;
        private Coroutine _durationCoroutine;
        private Coroutine _affectCoroutine;
        private readonly WaitForSeconds _coolTimeSecond;
        
        public DotEffect(int id, EffectType type, int duration, int coolTime) : base(id, type, coolTime)
        {
            IsPermanent = duration <= 0;
            IsEnabled = false;

            this.duration = IsPermanent ? 0 : duration;
            _coolTimeSecond = new WaitForSeconds(Constants.Time.SecondsPerMinute * coolTime);
        }

        public DotEffect(int id, EffectType type, int coolTime) : base(id, type, coolTime)
        {
            IsPermanent = true;
            IsEnabled = false;

            duration = 0;
            _coolTimeSecond = new WaitForSeconds(Constants.Time.SecondsPerMinute * coolTime);
        }
        
        public override object Clone()
        {
            var newEffect = IsPermanent
                ? new DotEffect(ID, Type, coolTime)
                : new DotEffect(ID, Type, duration, coolTime);
            newEffect.AttributeType = AttributeType;
            newEffect.AttributeValue = new EffectValue(AttributeValue);
            return newEffect;
        }

        public override void EnableEffect(IAffectable target, EffectController controller)
        {
            if (target == null || AttributeType == AttributeType.None)
            {
                Debug.Log($"[DotEffect] EnableEffect(): Cannot enable effect {ID}, {Type}");
                return;
            }

            _controller = controller;
            base.EnableEffect(target, controller);
            
            if (!IsPermanent)
            {
                _durationCoroutine = controller.StartCoroutine(StartDurationTimer());
            }
            _affectCoroutine = controller.StartCoroutine(AffectDotEffect());
        }

        public override void DisableEffect()
        {
            if (Target == null || !IsEnabled)
            {
                return;
            }
            
            base.DisableEffect();
            
            if (!IsPermanent)
            {
                _controller.StopCoroutine(_durationCoroutine);
            }
            _controller.StopCoroutine(_affectCoroutine);
            _controller = null;
        }
        
        private IEnumerator AffectDotEffect()
        {
            while (IsEnabled)
            {
                yield return _coolTimeSecond;
                
                AttributeValue.CalculateEffectValue(Target.GetReferenceValue(AttributeType));
                Target.Affect(AttributeType, AttributeValue.EffectedValue);
            }
        }
    }
}