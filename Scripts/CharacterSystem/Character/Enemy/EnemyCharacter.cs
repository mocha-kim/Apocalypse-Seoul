using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Enemy.State;
using CharacterSystem.Character.StateMachine;
using CharacterSystem.Stat;
using Core.Interface;
using DataSystem;
using DataSystem.Database;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace CharacterSystem.Character.Enemy
{
    [RequireComponent(typeof(FieldOfView))]
    public abstract partial class EnemyCharacter : Character, IHasLogicalForward
    {
        [SerializeField] protected int id;
        protected AttackType AttackType;
        
        protected EnemyStat Stat { get; private set; }
        protected StateMachine<EnemyCharacter> StateMachine;
        protected FieldOfView FOV;

        public override CharacterType CharacterType => CharacterType.Enemy;
        
        public Vector3 Forward { get; protected set; } = Vector3.down;
        public Transform Target => FOV.ClosestTarget;
        public float DistanceToTarget => FOV.DistanceToTarget;

        private Animator _animator;
        private int _hashDirX;
        private int _hashDirY;

        protected virtual void Awake()
        {
            _hashDirX = Animator.StringToHash("DirX");
            _hashDirY = Animator.StringToHash("DirY");
        }

        protected virtual void Start()
        {
            var data = Database.GetEnemy(id);
            Stat = data.stat.Clone() as EnemyStat;
            if (Stat == null)
            {
                Debug.LogError("[EnemyCharacter] Start(): Stat data is invalid. This must be fixed.");
                gameObject.SetActive(false);
                return;
            }

            AttackType = data.attacktype;
            GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset = ResourceManager.GetSpriteLibrary(data.spritePath);
            
            FOV = GetComponent<FieldOfView>();

            _animator = GetComponentInChildren<Animator>();
            _animator.SetFloat(_hashDirX, Forward.x);
            _animator.SetFloat(_hashDirY, Forward.y);
            
            InitStateMachine();
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
        
        public virtual void SwitchToIdleFOV()
        {
        }

        public virtual void SwitchToDetectionFov()
        {
        }
        
        public void FaceTo(Vector3 destination)
        {
            Forward = (destination - transform.position).normalized;

            _animator.SetFloat(_hashDirX, Forward.x);
            _animator.SetFloat(_hashDirY, Forward.y);
        }

        private void FaceTarget()
        {
            if (!Target)
            {
                return;
            }

            Forward = (Target.position - transform.position).normalized;

            _animator.SetFloat(_hashDirX, Forward.x);
            _animator.SetFloat(_hashDirY, Forward.y);
        }
    }

    // StateMachine
    public abstract partial class EnemyCharacter
    {
        protected virtual void InitStateMachine()
        {
            StateMachine = new StateMachine<EnemyCharacter>(this, new IdleState(), _animator);
            StateMachine.AddState(new DeadState());
        }
        
        protected void ChangeState<TState>() where TState : State<EnemyCharacter>
        {
            StateMachine.ChangeState<TState>();
        }
    }
    
    public abstract partial class EnemyCharacter : IMovable
    {
        public float MoveSpeed => Stat.GetAttributeValue(AttributeType.Speed) * Constants.Character.MoveSpeedFactor;
        
        public void Move()
        {
            Debug.LogWarning("[DefaultEnemyCharacter] Move(): Enemy character moves on its own mover. This method do nothing");
        }
    }
}