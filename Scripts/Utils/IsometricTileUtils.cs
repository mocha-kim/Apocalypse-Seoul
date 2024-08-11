using System;
using UnityEngine;

namespace Utils
{
    public static class IsometricTileUtils
    {
        public static float TileWidth { get; } = 2;
        public static float TileHeight { get; } = 1;
        
        private static readonly Vector2 Offset = new(0.25f, 0.25f);

        public static Vector2 TileToWorld(Vector2 tilePos)
        {
            tilePos += Offset;
            var worldX = (tilePos.x - tilePos.y) * (TileWidth / 2);
            var worldY = (tilePos.x + tilePos.y) * (TileHeight / 2);
            return new Vector2(worldX, worldY);
        }

        public static Vector2 WorldToTile(Vector2 worldPos)
        {
            var tileX = (worldPos.x / (TileWidth / 2) + worldPos.y / (TileHeight / 2)) / 2;
            var tileY = (worldPos.y / (TileHeight / 2) - worldPos.x / (TileWidth / 2)) / 2;
            return new Vector2(tileX, tileY) - Offset;
        }

        public static Vector2 ClampToCenterPosition(Vector2 position)
        { 
            return new Vector2(Round(position.x), Round(position.y));
        }
        
        private static float Round(float value)
        {
            var rounded = Mathf.Round(value * 2) / 2;
            return rounded;
        }
    }
}