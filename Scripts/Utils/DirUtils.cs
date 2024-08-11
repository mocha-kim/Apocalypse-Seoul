using CharacterSystem.Character;
using DataSystem;
using InputSystem;
using UnityEngine;

namespace Utils
{
    public static class DirUtils
    {
        public static Vector2 ConvertDirToVector2(MoveDir moveDir)
        {
            var vector = moveDir switch
            {
                MoveDir.None => Constants.MoveDir.None,
                MoveDir.Up => Constants.MoveDir.Up,
                MoveDir.Down => Constants.MoveDir.Down,
                MoveDir.Left => Constants.MoveDir.Left,
                MoveDir.Right => Constants.MoveDir.Right,
                MoveDir.UpLeft => Constants.MoveDir.UpLeft,
                MoveDir.UpRight => Constants.MoveDir.UpRight,
                MoveDir.DownLeft => Constants.MoveDir.DownLeft,
                MoveDir.DownRight => Constants.MoveDir.DownRight,
                _ => Vector2.zero,
            };
            return vector;
        }
        
        public static Vector2 ConvertVector2ToDir(Vector2 vector)
        {
            var angle = Quaternion.FromToRotation(Vector3.down, vector).eulerAngles.z;
            var dir = angle switch
            {
                < 22.5f => Constants.MoveDir.Down,
                >= 22.5f and < 67.5f => Constants.MoveDir.DownRight,
                >= 67.5f and < 112.5f => Constants.MoveDir.Right,
                >= 112.5f and < 157.5f => Constants.MoveDir.UpRight,
                >= 157.5f and < 202.5f => Constants.MoveDir.Up,
                >= 202.5f and < 247.5f => Constants.MoveDir.UpLeft,
                >= 247.5f and < 292.5f => Constants.MoveDir.Left,
                >= 292.5f and < 337.5f => Constants.MoveDir.DownLeft,
                >= 337.5f => Constants.MoveDir.Down,
                _ => Vector2.zero
            };
            return dir;
        }
    }
}
