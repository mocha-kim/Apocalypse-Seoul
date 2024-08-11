using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace CharacterSystem.Character.Combat.PathFinder
{
    public class AStarPathfinder
    {
        private readonly SortedSet<Node> _openSet = new ();
        private readonly HashSet<Node> _closedSet = new ();
        private readonly Dictionary<Vector2, Node> _allNodes = new();

        private static readonly Vector2[] Directions =
        {
            new(0.5f, 0), new(-0.5f, 0), new(0, 0.5f), new(0, -0.5f),
            new(0.5f, 0.5f), new(0.5f, -0.5f), new(-0.5f, 0.5f), new(-0.5f, -0.5f)
        };

        private Node[,] _walkableMap;

        public void Init(Node[,] walkableMap)
        {
            _walkableMap = walkableMap;
            _allNodes.Clear();
            
            for (var x = 0; x < _walkableMap.GetLength(0); x++)
            {
                for (var y = 0; y < _walkableMap.GetLength(1); y++)
                {
                    var node = _walkableMap[x, y];
                    var newNode = new Node(node.TilePosition, node.WorldPosition, isWalkable: node.IsWalkable);
                    _allNodes[_walkableMap[x, y].TilePosition] = newNode;
                }
            }
        }

        public void FindPath(Vector2 start, Vector2 end, ref List<Vector2> path)
        {
            var startTile = IsometricTileUtils.ClampToCenterPosition(IsometricTileUtils.WorldToTile(start));
            var endTile = IsometricTileUtils.ClampToCenterPosition(IsometricTileUtils.WorldToTile(end));
            // Debug.Log($"Start: {startTile}({start}), End: {endTile}({end})");
            
            var startNode = _allNodes.GetValueOrDefault(startTile);
            if (startNode == null)
            {
                path.Clear();
                return;
            }
            var endNode = _allNodes.GetValueOrDefault(endTile);
            if (endNode == null)
            {
                path.Clear();
                return;
            }

            _openSet.Clear();
            _closedSet.Clear();
            
            startNode.GCost = 0;
            startNode.HCost = Vector2.Distance(startNode.WorldPosition, endNode.WorldPosition);
            _openSet.Add(startNode);
            while (_openSet.Count > 0)
            {
                var currentNode = _openSet.Min;
                _openSet.Remove(currentNode);
                _closedSet.Add(currentNode);

                if (currentNode.TilePosition == endTile)
                {
                    ReconstructPath(currentNode, ref path);
                    return;
                }

                foreach (var direction in Directions)
                {
                    var neighborTile = currentNode.TilePosition + direction;
                    if (!IsPositionValid(neighborTile) || _closedSet.Contains(_allNodes.GetValueOrDefault(neighborTile)))
                    {
                        continue;
                    }

                    var neighborNode = _allNodes.GetValueOrDefault(neighborTile);
                    var tentativeGCost = currentNode.GCost + Vector2.Distance(currentNode.WorldPosition, neighborNode.WorldPosition);
                    if (tentativeGCost >= neighborNode.GCost || _openSet.Contains(neighborNode))
                    {
                        continue;
                    }

                    neighborNode.GCost = tentativeGCost;
                    neighborNode.HCost = Vector2.Distance(neighborNode.WorldPosition, endNode.WorldPosition);
                    neighborNode.Parent = currentNode;
                    _openSet.Add(neighborNode);
                }
            }

            path.Clear();
            ClearAllNodesCosts();
        }

        public bool IsPositionValid(Vector2 tilePos)
        {
            var node = _allNodes.GetValueOrDefault(tilePos);
            return node == null || node.IsWalkable;
        }

        private void ClearAllNodesCosts()
        {
            foreach (var pair in _allNodes)
            {
                pair.Value.GCost = float.MaxValue;
                pair.Value.HCost = float.MaxValue;
                pair.Value.Parent = null;
            }
        }
        
        private void ReconstructPath(Node endNode, ref List<Vector2> path)
        {
            path.Clear();
            
            var currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(IsometricTileUtils.TileToWorld(currentNode.TilePosition));
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            ClearAllNodesCosts();
        }
    }
}