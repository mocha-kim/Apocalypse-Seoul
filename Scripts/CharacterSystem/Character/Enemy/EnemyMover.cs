using System.Collections.Generic;
using CharacterSystem.Character.Combat.PathFinder;
using Core.Interface;
using DataSystem;
using EnvironmentSystem.Area;
using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(EnemyCharacter))]
    public class EnemyMover : MonoBehaviour
    {
        private bool _hasDestination = false;
        private Vector2 _destination;
        private Vector2 _nextPosition;
        private Rigidbody2D _rigidbody2D;
        private IMovable _movableContext;
        
        private int _currentPathIndex;
        private List<Vector2> _currentPath = new();
        public AStarPathfinder Pathfinder { get; private set; }

        private Vector2 Position => _rigidbody2D.position;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _movableContext = GetComponent<IMovable>();
            Pathfinder = new AStarPathfinder();
        }

        private void Start()
        {
            Pathfinder.Init(GetComponentInParent<Floor>().GetWalkableMap());
        }

        private void OnDrawGizmosSelected()
        {
            if (!_hasDestination || _currentPathIndex >= _currentPath.Count)
            {
                return;
            }
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_destination, 0.2f);
                
            if (_currentPathIndex >= _currentPath.Count)
            {
                return;
            }
            Gizmos.color = Color.white;
            foreach (var node in _currentPath)
            {
                Gizmos.DrawSphere(node, 0.1f);
            }
        }

        public void SetDestination(Vector2 destination)
        {
            _destination = destination;
            _hasDestination = true;
            _currentPath.Clear();

            UpdatePath();
        }

        public void ClearDestination()
        {
            _hasDestination = false;
        }

        public void Move(float deltaTime)
        {
            if (!_hasDestination || _currentPathIndex >= _currentPath.Count)
            {
                _hasDestination = false;
                _currentPath.Clear();
                return;
            }

            _rigidbody2D.MovePosition(Vector2.MoveTowards(Position, _nextPosition, deltaTime * _movableContext.MoveSpeed));
            if (Vector2.Distance(Position, _nextPosition) < Constants.Tolerance)
            {
                _currentPathIndex++;
                if (_currentPathIndex >= _currentPath.Count)
                {
                    _hasDestination = false;
                    _currentPath.Clear();
                    return;
                }
                _nextPosition = _currentPath[_currentPathIndex];
            }
        }
        
        private void UpdatePath()
        {
            if (!_hasDestination)
            {
                return;
            }
            Pathfinder.FindPath(transform.position, _destination, ref _currentPath);
            if (_currentPath.Count == 0)
            {
                Debug.Log($"[EnemyMover] UpdatePath(): Cannot find path to {_destination}");
                return;
            }
            // Debug.Log($"Found path({_currentPath.Count}"
            //           + _currentPath.Aggregate("): ", (current, pos) => current + $"({pos.x}, {pos.y})")
            //           + $" -> Destination({_destination.x}, {_destination.y})");
            _currentPathIndex = 0;
            _nextPosition = _currentPath[_currentPathIndex];
        }
    }
}