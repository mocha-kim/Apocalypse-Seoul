using System;
using UnityEditor;
using UnityEngine;
using Utils;

namespace EnvironmentSystem.Area.Editor
{
    [CustomEditor(typeof(Floor))]
    public class FloorEditor : UnityEditor.Editor
    {
        private static readonly Color Green = new(0f, 1f, 0f, 0.5f);
        private static readonly Color Red = new(1f, 0f, 0f, 0.5f);
        
        private void OnSceneGUI()
        {
            var floor = (Floor)target;
            var map = floor.GetWalkableMap();
            
            if (map == null)
                return;

            foreach (var node in map)
            {
                var position = (Vector3)IsometricTileUtils.TileToWorld(node.TilePosition);
            
                Handles.color = node.IsWalkable ? Green : Red;
                Handles.DrawSolidDisc(position, Vector3.forward , 0.1f);
                Handles.Label(position, $"[{node.TilePosition.x}, {node.TilePosition.y}]");
            }
        }
    }
}