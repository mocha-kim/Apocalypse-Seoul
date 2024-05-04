using CharacterSystem.Effect;
using DataSystem;
using Event;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UserActionBind;

namespace CharacterSystem.Character.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        private PlayerEffectController _effectController;
        
        public float moveSpeed = 4.0f;
        public float runSpeed = 8.0f;

        private Vector2 _pressDir = Vector2.zero;

        private bool _isMovable;
        private bool _isPressSpeedUp;

        private float _lastInputTime;
        private Vector2 _lastDir = Vector2.zero;

        private int _keyIsMoving;
        private int _keyIsRunning;
        private int _keyMoveX;
        private int _keyMoveY;
        private int _keyLastMoveX;
        private int _keyLastMoveY;

        private void Awake()
        {
            _effectController = GetComponent<PlayerEffectController>();
            _isMovable = true;

            EventManager.Subscribe(gameObject, Message.OnCharacterBusy, _ => StopMoving());
            EventManager.Subscribe(gameObject, Message.OnCharacterFree, _ => StartMoving());

            _keyMoveX = Animator.StringToHash("MoveX");
            _keyMoveY = Animator.StringToHash("MoveY");
            _keyLastMoveX = Animator.StringToHash("LastMoveX");
            _keyLastMoveY = Animator.StringToHash("LastMoveY");
            _keyIsMoving = Animator.StringToHash("IsMoving");
            _keyIsRunning = Animator.StringToHash("IsRunning");
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => _isMovable)
                .Where(_ => Input.anyKey)
                .Subscribe(_ =>
                {
                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.Run]))
                    {
                        StartRunning();
                    }
                }).AddTo(gameObject);
                
            this.Press8DirObservable()
                .Where(_ => _isMovable)
                .DistinctUntilChanged()
                .Subscribe(dir =>
                {
                    _pressDir = ConvertDirToVector2(dir);
                    _animator.SetFloat(_keyMoveX, _pressDir.x);
                    _animator.SetFloat(_keyMoveY, _pressDir.y);

                    var now = Time.time;
                    if (now - _lastInputTime > Constants.Time.MinInputGap)
                    {
                        UpdateLaseMove();
                    }
                    _lastInputTime = now;
                    _lastDir = _pressDir;
                }).AddTo(gameObject);
        }
        
        private void FixedUpdate()
        {
            if (_isMovable == false)
            {
                return;
            }

            if (_isPressSpeedUp)
            {
                _rigidbody2D.MovePosition(_rigidbody2D.position + Time.fixedDeltaTime * runSpeed * _pressDir);
            }
            else
            {
                _rigidbody2D.MovePosition(_rigidbody2D.position + Time.fixedDeltaTime * moveSpeed * _pressDir);
            }
        }

        private void StopMoving()
        {
            StopRunning();
            UpdateLaseMove();
            
            _isMovable = false;
            _animator.SetBool(_keyIsMoving, false);
        }

        private void StartMoving()
        {
            _isMovable = true;
            if (_pressDir != Vector2.zero)
            {
                _animator.SetBool(_keyIsMoving, true);
            }
        }

        private void StartRunning()
        {
            _isPressSpeedUp = true;
            _animator.SetBool(_keyIsRunning, _isPressSpeedUp);
            _effectController.EnableEffect(EffectType.Running);
            
            InputManager.StartDetectingRelease(InputBinding.Bindings[UserAction.Run], StopRunning);
        }

        private void StopRunning()
        {
            _isPressSpeedUp = false;
            _animator.SetBool(_keyIsRunning, _isPressSpeedUp);
            _effectController.DisableEffect(EffectType.Running);
        }

        private void UpdateLaseMove()
        {
            _animator.SetFloat(_keyLastMoveX, _lastDir.x);
            _animator.SetFloat(_keyLastMoveY, _lastDir.y);
        }

        private Vector2 ConvertDirToVector2(MoveDir moveDir)
        {
            var vector = moveDir switch
            {
                MoveDir.None => Vector2.zero,
                MoveDir.Up => Vector2.up,
                MoveDir.Down => Vector2.down,
                MoveDir.Left => Vector2.left,
                MoveDir.Right => Vector2.right,
                MoveDir.UpLeft => new Vector2(-2, 1).normalized,
                MoveDir.UpRight => new Vector2(2, 1).normalized,
                MoveDir.DownLeft => new Vector2(-2, -1).normalized,
                MoveDir.DownRight => new Vector2(2, -1).normalized,
                _ => Vector2.zero,
            };
            _animator.SetBool(_keyIsMoving, vector != Vector2.zero);
            return vector;
        }
    }
}