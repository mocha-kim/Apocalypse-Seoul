using CharacterSystem.Character.Combat.AttackBehavior;

namespace CharacterSystem.Character.Enemy
{
    public class RangeAttackEnemyCharacter : PatrolEnemyCharacter
    {
        protected override void Start()
        {
            base.Start();

            _attackBehaviors.Add(
                new RangeSingleAttackBehavior(transform
                    , range: 2f
                    , targetMaskStrings: targetMaskStrings)
                );
            CurrentAttackBehavior = _attackBehaviors[0];

            InitEnemyStat(100, 1, 10, 0, 0, 3);
        }

        public override void SwitchToIdleFOV() => FOV.SetViewFactors(3f, 120f, targetMaskStrings);
        public override void SwitchToDetectionFov() => FOV.SetViewFactors(6f, 360f, targetMaskStrings);
    }
}