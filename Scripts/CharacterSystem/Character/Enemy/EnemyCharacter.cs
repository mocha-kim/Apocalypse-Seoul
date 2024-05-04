using System.Collections.Generic;
using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Character.StateMachine;
using CharacterSystem.Stat;
using Core.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterSystem.Character.Enemy
{
    [RequireComponent(typeof(FieldOfView))]
    public abstract class EnemyCharacter : Character, IHasLogicalForward
    {
        protected StateMachine<EnemyCharacter> StateMachine;
        protected FieldOfView FOV;

        private int hashDirX;
        private int hashDirY;

        public override CharacterType CharacterType => CharacterType.Enemy;
        public Vector3 Forward { get; protected set; } = Vector3.down;

        public EnemyStat Stat { get; protected set; }
        public Animator Animator { get; protected set; }
        public Transform Target => FOV.ClosestTarget;
        public float DistanceToTarget => FOV.DistanceToTarget;

        protected List<AttackBehavior> _attackBehaviors = new();

        protected virtual void Awake()
        {
            hashDirX = Animator.StringToHash("DirX");
            hashDirY = Animator.StringToHash("DirY");
        }

        protected virtual void Start()
        {
            FOV = GetComponent<FieldOfView>();

            Animator = GetComponentInChildren<Animator>();
            Animator.SetFloat(hashDirX, Forward.x);
            Animator.SetFloat(hashDirY, Forward.y);
            
            StateMachine = new StateMachine<EnemyCharacter>(this, new IdleState());
            StateMachine.AddState(new DeadState());
        }

        protected virtual void Update()
        {
            StateMachine.Update(Time.deltaTime);
            if (StateMachine.Currentstate is ChaseState)
            {
                FaceTarget();
            }
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        public void InitEnemyStat(int hp, int speed, int attack, int defense, int attackSpeed, int attackRange)
        {
            Stat = new EnemyStat(hp, speed, attack, defense, attackSpeed, attackRange);
        }

        public void ChangeState<TState>() where TState : State<EnemyCharacter>
        {
            StateMachine.ChangeState<TState>();
        }

        private void FaceTarget()
        {
            if (!Target)
            {
                return;
            }

            Forward = (Target.position - transform.position).normalized;

            Animator.SetFloat(hashDirX, Forward.x);
            Animator.SetFloat(hashDirY, Forward.y);
        }

        public void FaceTo(Vector3 destination)
        {
            Forward = (destination - transform.position).normalized;

            Animator.SetFloat(hashDirX, Forward.x);
            Animator.SetFloat(hashDirY, Forward.y);
        }

        public virtual void SwitchToIdleFOV()
        {
        }

        public virtual void SwitchToDetectionFov()
        {
        }
    }
}