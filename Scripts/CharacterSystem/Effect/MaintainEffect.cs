using CharacterSystem.Character;
using Core.Interface;
using Event;
using Manager;

namespace CharacterSystem.Effect
{
    public class MaintainEffect : Effect
    {
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

            foreach (var type in EffectValues.Keys)
            {
                EffectValues[type].CalculateEffectValue(Target.GetReferenceValue(type));
                Target.Affect(type, EffectValues[type].EffectedValue);
            }
        }

        public override void DisableEffect()
        {
            base.DisableEffect();

            foreach (var pair in EffectValues)
            {
                Target.Affect(pair.Key, -pair.Value.EffectedValue);
            }

            if (Target.CharacterType == CharacterType.Player)
            {
                EventManager.OnNext(Message.OnPlayerEffectChanged);
            }
        }
    }
}