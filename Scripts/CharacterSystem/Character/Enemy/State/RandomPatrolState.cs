using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace CharacterSystem.Character.Enemy.State
{
    public class RandomPatrolState : PatrolState
    {
        private float _range;
        private Vector3 _center;
        
        public RandomPatrolState(float range, Vector3 center)
        {
            _range = range;
            _center = center;
        }

        protected override void UpdateNextDestination()
        {
            var isValidDestination = false;
            var worldPosition = Context.transform.position;
            while (!isValidDestination)
            {
                worldPosition = new Vector2(
                    Random.Range(_center.x - _range, _center.x + _range)
                    , Random.Range(_center.y - _range, _center.y + _range)
                );
                var tilePosition = IsometricTileUtils.ClampToCenterPosition(IsometricTileUtils.WorldToTile(worldPosition));
                if (Mover.Pathfinder.IsPositionValid(tilePosition))
                {
                    isValidDestination = true;
                }
            }
            Destination = worldPosition;
        }
    }
}