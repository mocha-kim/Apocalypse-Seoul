using System;
using UnityEngine;

namespace CharacterSystem.Character.Combat.PathFinder
{
    public class Node : IComparable<Node>
    {
        public bool IsWalkable;
        public Vector2 TilePosition;
        public Vector2 WorldPosition;
        
        public float GCost = float.MaxValue;
        public float HCost = float.MaxValue;
        public float FCost => GCost + HCost;
        
        public Node Parent = null;

        public Node(Vector2 tilePosition, Vector2 worldPosition, bool isWalkable = true)
        {
            TilePosition = tilePosition;
            WorldPosition = worldPosition;
            IsWalkable = isWalkable;
        }
        
        public Node(Vector2 tilePosition, Vector2 worldPosition, float gCost = float.MaxValue, float hCost = float.MaxValue, Node parent = null, bool isWalkable = true)
        {
            TilePosition = tilePosition;
            WorldPosition = worldPosition;
            GCost = gCost;
            HCost = hCost;
            Parent = parent;
            IsWalkable = isWalkable;
        }

        public int CompareTo(Node other)
        {
            var compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }
            return compare;
        }

        public override string ToString()
        {
            return $"Node: IsWalkable({IsWalkable}), TilePosition({TilePosition}), GCost({GCost}), HCost({HCost}), Cost({FCost}), Parent({Parent != null})";
        }
    }
}