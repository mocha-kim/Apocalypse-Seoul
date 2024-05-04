using UnityEngine;

namespace CharacterSystem.Character.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMover : MonoBehaviour
    {
        private Vector2 _destination;
        private Rigidbody2D _rigidbody2D;
        private int _speed;

        private Vector2 Position => _rigidbody2D.position;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void SetDestination(Vector2 destination, int speed)
        {
            _destination = destination;
            _speed = speed;
        }

        public void Move()
        {
            _rigidbody2D.MovePosition(Position + (_destination - Position).normalized * (Time.fixedDeltaTime * _speed));
        }
    }
}