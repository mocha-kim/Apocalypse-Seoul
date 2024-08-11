using Core.Interface;
using DataSystem;
using EnvironmentSystem.Camera;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace CharacterSystem.Character.Player
{
    //[RequireComponent(typeof(LineRenderer))]
    public sealed class PlayerAttackPainter : MonoBehaviour
    {
        private Transform _context;
        private IAttackable _attackable;

        private LayerMask _targetLayer;

        private RaycastHit2D _aimHit;
        private LineRenderer _aimLineRenderer;

        [SerializeField] private GameObject _rangePainter;

        private Vector3 MousePosition => MainCamera.GetMouseWorldPosition();

        private void Awake()
        {
            _context = transform.parent;
            _attackable = _context.GetComponent<IAttackable>();

            _aimLineRenderer = GetComponentInChildren<LineRenderer>();
        }

        public void Init(string[] targetMaskStrings)
        {
            _targetLayer = LayerMask.GetMask(targetMaskStrings) | (1 << Constants.Layer.Obstacle);
        }

        public void DrawAim()
        {
            var startPoint = _attackable.CurrentAttackBehavior.StartPoint;
            var direction = MousePosition - startPoint;
            var distance = Vector2.Distance(startPoint, MousePosition);
            var range = _attackable.CurrentAttackBehavior.Range;

            _aimHit = Physics2D.Raycast(startPoint, direction, Mathf.Min(range, distance), _targetLayer);
            if (_aimHit)
            {
                _aimLineRenderer.SetPosition(1, _aimHit.point);
            }
            else
            {
                var clampedPosition = (Vector3)((Vector2)direction).normalized * range + startPoint;
                _aimLineRenderer.SetPosition(1, distance < range ? (Vector2)MousePosition : clampedPosition);
            }

            _aimLineRenderer.SetPosition(0, startPoint);
        }

        public void StartDrawAim(bool istrue)
        {
            _aimLineRenderer.enabled = istrue;
        }

        public void StartDrawRange(bool istrue)
        {
            _rangePainter.SetActive(istrue);
        }

        public void DrawCircularSector()
        {

        }

        public void DrawRange()
        {
            Vector2 NormalizedMouseVector = (MousePosition - _context.position).normalized;
            _rangePainter.transform.rotation = Quaternion.Euler(0, 0,
                Quaternion.FromToRotation(Vector3.up, NormalizedMouseVector).eulerAngles.z);
        }
    }
}