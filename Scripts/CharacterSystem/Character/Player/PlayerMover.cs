using AudioSystem;
using CharacterSystem.Effect;
using Core.Interface;
using DataSystem;
using EventSystem;
using InputSystem;
using InputSystem.InputTrigger;
using InputSystem.UserActionBind;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace CharacterSystem.Character.Player
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(PlayerCharacter))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        private IMovable _movableContext;
        
        private PlayerEffectController _effectController;

        private Vector2 _pressDir = Vector2.zero;

        private bool _isMovable = true;
        private bool _isPressSpeedUp = false;

        private float _lastInputTime = 0f;
        private Vector2 _lastDir = Vector2.zero;

        private int _keyIsMoving;
        private int _keyIsRunning;
        private int _keyMoveX;
        private int _keyMoveY;
        private int _keyLastMoveX;
        private int _keyLastMoveY;

        private float _footStepInterval;
        private float _maxFootStepInterval = 0.33333f;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _movableContext = GetComponent<IMovable>();
            
            _effectController = GetComponent<PlayerEffectController>();

            EventManager.Subscribe(gameObject, Message.OnCharacterBusy, _ => StopMoving());
            EventManager.Subscribe(gameObject, Message.OnCharacterFree, _ => StartMoving());
            
            EventManager.Subscribe(gameObject, Message.OnPlayerAimStart, _ => StopMoving());
            EventManager.Subscribe(gameObject, Message.OnPlayerAimEnd, _ => StartMoving());
            
            EventManager.Subscribe(gameObject, Message.OnCancelRunning, _ => _isPressSpeedUp = false);

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
                    _pressDir = DirUtils.ConvertDirToVector2(dir);
                    _animator.SetBool(_keyIsMoving, _pressDir != Vector2.zero);
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
                _rigidbody2D.MovePosition(_rigidbody2D.position);
                return;
            }

            var movePosition = Time.fixedDeltaTime * _movableContext.MoveSpeed * _pressDir;
            if (_isPressSpeedUp)
            {
                _rigidbody2D.MovePosition(_rigidbody2D.position + movePosition * Constants.Character.RunSpeedFactor);
            }
            else
            {
                _rigidbody2D.MovePosition(_rigidbody2D.position + movePosition);
            }

            if (_pressDir != Vector2.zero)
            {
                _footStepInterval += Time.deltaTime;
                if (_footStepInterval >= _maxFootStepInterval)
                {
                    _footStepInterval = 0f;
                    AudioManager.Instance.PlaySFX(SFXType.PlayerFootstep, gameObject);
                }
            }
        }

        private void StopMoving()
        {
            StopRunning();

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

            AudioManager.Instance.SetWwiseParameter(SFXParameter.PlayerSpeed, 2);
        }

        private void StopRunning()
        {
            _isPressSpeedUp = false;
            _animator.SetBool(_keyIsRunning, _isPressSpeedUp);
            _effectController.DisableEffect(EffectType.Running);

            AudioManager.Instance.SetWwiseParameter(SFXParameter.PlayerSpeed, 1);
        }

        private void UpdateLaseMove()
        {
            _animator.SetFloat(_keyLastMoveX, _lastDir.x);
            _animator.SetFloat(_keyLastMoveY, _lastDir.y);
        }

    }
}