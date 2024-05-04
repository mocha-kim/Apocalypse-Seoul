using CharacterSystem.Character.Combat.AttackBehavior;
using Core.Interface;
using DataSystem;
using EnvironmentSystem.Camera;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace CharacterSystem.Character.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class PlayerAttackPainter : MonoBehaviour
    {
        private RaycastHit2D _aimHit;

        private IAttackable _playerAttackable;
        private LayerMask _targetLayer;

        private LineRenderer _aimLineRenderer;

        private Transform _context;
        private Vector3 MousePosition => MainCamera.GetMouseWorldPosition();

        private void Awake()
        {
            _context = transform.parent;

            _playerAttackable = _context.GetComponent<IAttackable>();
            _aimLineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            switch (_playerAttackable.CurrentAttackBehavior)
            {
                case HitScanSingleAttackBehavior attackBehavior:
                    DrawAim(attackBehavior);
                    break;
                case ColliderAttackBehavior:
                    DrawCircularSector();
                    break;
            }
        }

        public void Init(string[] targetMaskStrings)
        {
            _targetLayer = LayerMask.GetMask(targetMaskStrings) | (1 << Constants.Layer.Obstacle);
        }

        private void DrawAim(HitScanSingleAttackBehavior attackBehavior)
        {
            var startPoint = attackBehavior.StartPoint;
            var range = attackBehavior.range;
            _aimHit = Physics2D.Raycast(startPoint, MousePosition - startPoint, range, _targetLayer);
            if (_aimHit)
            {
                _aimLineRenderer.SetPosition(1, _aimHit.point);
            }
            else
            {
                var distance = Vector2.Distance(startPoint, MousePosition);
                var direction = MousePosition - startPoint;
                var clampedPosition = (Vector3)((Vector2)direction).normalized * attackBehavior.range + startPoint;
                _aimLineRenderer.SetPosition(1, distance < range ? (Vector2)MousePosition : clampedPosition);
            }
            _aimLineRenderer.SetPosition(0, startPoint);
        }

        private void DrawCircularSector()
        {

        }
    }
}