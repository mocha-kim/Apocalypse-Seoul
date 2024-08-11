using CharacterSystem.Character.Combat.PathFinder;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace EnvironmentSystem.Area
{
    public class Floor : Area
    {
        [SerializeField] private Floor _upstairs;
        [SerializeField] private Floor _downstairs;

        [SerializeField] private Stairs[] _stairsToUp;
        [SerializeField] private Stairs[] _stairsToDown;

        [SerializeField] private Tilemap _obstacleTilemap;
        private Node[,] _walkableMap;
        private Vector3Int _mapSize;
        private Vector3Int _mapOrigin;

        private static readonly Vector3Int Offset = new(1, 0, 0);

        protected override void Awake()
        {
            base.Awake();
            InitializeWalkableMap();
        }

        private void OnEnable()
        {
            foreach (var stairs in _stairsToUp)
            {
                stairs.OnFadeNextFloor += GoUpstairs;
            }
            foreach (var stairs in _stairsToDown)
            {
                stairs.OnFadeNextFloor += GoDownstairs;
            }
        }

        private void OnDisable()
        {
            foreach (var stairs in _stairsToUp)
            {
                stairs.OnFadeNextFloor -= GoUpstairs;
            }
            foreach (var stairs in _stairsToDown)
            {
                stairs.OnFadeNextFloor -= GoDownstairs;
            }
        }
        
        public Node[,] GetWalkableMap() => _walkableMap;

        private void GoUpstairs()
        {
            if (_upstairs == null)
            {
                return;
            }
            gameObject.SetActive(false);
            _upstairs.gameObject.SetActive(true);
        }

        private void GoDownstairs()
        {
            if (_downstairs == null)
            {
                return;
            }
            gameObject.SetActive(false);
            _downstairs.gameObject.SetActive(true);
        }
        
        private void InitializeWalkableMap()
        {
            _mapOrigin = _obstacleTilemap.origin;
            _mapSize = _obstacleTilemap.size;

            _walkableMap = new Node[_mapSize.x, _mapSize.y];
            for (var x = 0; x < _mapSize.x; x++)
            {
                for (var y = 0; y < _mapSize.y; y++)
                {
                    var tilePosition = new Vector3Int(_mapOrigin.x + x, _mapOrigin.y + y, 0) - Offset;
                    var worldPosition = _obstacleTilemap.CellToWorld(tilePosition) + _obstacleTilemap.tileAnchor;
                    _walkableMap[x, y] = new Node(
                        IsometricTileUtils.WorldToTile(worldPosition),
                        worldPosition,
                        !IsTileObstacle(tilePosition)
                    );
                }
            }
        }

        private bool IsTileObstacle(Vector3Int tilePosition)
        {
            var tile = _obstacleTilemap.GetTile(tilePosition + Offset);
            return tile != null;
        }
    }
}