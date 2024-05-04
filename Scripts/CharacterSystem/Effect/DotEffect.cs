using System.Collections;
using Alpha;
using CharacterSystem.Character;
using Core;
using Core.Interface;
using DataSystem;
using Event;
using Manager;
using UnityEngine;

namespace CharacterSystem.Effect
{
    public class DotEffect : Effect
    {
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
            foreach (var pair in EffectValues)
            {
                newEffect.EffectValues[pair.Key] = pair.Value;
            }
            return newEffect;
        }

        public override void EnableEffect(IAffectable target, EffectController controller)
        {
            base.EnableEffect(target, controller);
            
            if (!IsPermanent)
            {
                controller.StartCoroutine(StartDurationTimer());
            }
            
            controller.StartCoroutine(AffectDotEffect());
        }

        public override void DisableEffect()
        {
            base.DisableEffect();
            
            if (Target.CharacterType == CharacterType.Player)
            {
                EventManager.OnNext(Message.OnPlayerEffectChanged);
            }
        }

        private void AffectDot()
        {
            foreach (var pair in EffectValues)
            {
                Target.Affect(pair.Key, pair.Value.EffectedValue);
            }
        }

        private IEnumerator AffectDotEffect()
        {
            while (IsEnabled)
            {
                yield return _coolTimeSecond;
                AffectDot();
            }
        }
    }
}