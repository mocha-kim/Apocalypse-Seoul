using System.Collections.Generic;
using System.ComponentModel;
using CharacterSystem.Character;
using CharacterSystem.Character.Combat;
using CharacterSystem.Stat;
using UnityEngine;

namespace DataSystem
{
    public static class Constants
    {
        public const string UndefinedString = "undefined";
        
        public static class Scene
        {
            public const int Home = 000000;
        }

        public static class Path
        {
            public const string PrefabPath = "Prefabs";
            public static readonly string[] PrefabSubfolders =
            {
                "/UI",
                "/NPC",
                "/Enemy"
            };

            public const string SpritePath = "Sprites";
            public static readonly string[] SpriteSubfolders =
            {
                "/Cursor",
                "/Dummy",
                "/Item/Item01",
                "/Item/Item02",
                "/ItemBox",
                "/Map",
                "/Player",
                "/ItemBox",
            };
            
            public const string SpriteLibraryPath = "SpriteLibrary";
            public static readonly string[] SpriteLibrarySubfolders =
            {
                "/Enemy",
                "/NPC",
            };

            public static readonly Dictionary<AttributeType, string> AttributeIconPath = new()
            {
                { AttributeType.Hp, "Icon_HP" },
                { AttributeType.Sp, "Icon_SP" },
                { AttributeType.Hunger, "Icon_Hunger" },
                { AttributeType.Thirst, "Icon_Thirst" },
                { AttributeType.Speed, "Icon_Speed" },
                { AttributeType.Dexterity, "Icon_Dexterity" },
                { AttributeType.Attack, "Icon_Attack" },
                { AttributeType.Defense, "Icon_Defense" },
                { AttributeType.AttackSpeed, "Icon_AttackSpeed" },
                { AttributeType.AttackRange, "Icon_AttackRange" },
            };

            public const string CSVPath = "CSV";

            public const string DefaultIconPath = "Loading";
            public const string EmptyPrefabPath = "EmptyObject";
        }

        public static class Layer
        {
            public const string PlayerString = "Player";
            public const string EnemyString = "Enemy";
            public const string PlayerColliderString = "PlayerCollider";
            public const string ObstacleString = "Obstacle";
            public const string WallString = "Wall";
            
            public static readonly int Player = LayerMask.NameToLayer(PlayerString);
            public static readonly int PlayerCollider = LayerMask.NameToLayer(PlayerColliderString);
            public static readonly int Enemy = LayerMask.NameToLayer(EnemyString);
            public static readonly int Obstacle = LayerMask.NameToLayer(ObstacleString);
            public static readonly int Wall = LayerMask.NameToLayer(WallString);

            public static int GetCharacterMask(CharacterType type)
            {
                return type switch
                {
                    CharacterType.Player => Player,
                    CharacterType.Enemy => Enemy,
                    _ => Obstacle
                };
            }
        }

        public static class Input
        {
            public const int RightMouse = 1;
            public const int LeftMouse = 0;
        }

        public static class Database
        {
            public const int DatabaseIdRange = 10000;
            public const int IndividualIdRange = 1000;

            public const int NormalItemPrefix = 10;
            public const int ConsumableItemPrefix = 20;
            public const int ItemBoxPrefix = 30;
            public const int NPCPrefix = 90;

            public static string CurrentLanguage = LanguageType.Kor.ToString();
        }
        
        public static class Item
        {
            public const float MinBoxValueCorrector = 0.4f;
            public const float MaxBoxValueCorrector = 1f;
        }

        public static class Inventory
        {
            public const int QuickSlotSize = 3;
            public const int DefaultInventorySize = 8;
            public const int MaxInventorySize = 24;
            public const int MaxStorageSize = 48;
        }

        public static class Character
        {
            public static readonly Dictionary<AttributeType, int> DefaultValue = new()
            {
                { AttributeType.Hp, 100 },
                { AttributeType.Sp, 100 },
                { AttributeType.Hunger, 100 },
                { AttributeType.Thirst, 100 },
                { AttributeType.Speed, 1 },
                { AttributeType.Dexterity, 10 },
                { AttributeType.Durability, 100 },
                { AttributeType.Attack, 0 },
                { AttributeType.Defense, 0 },
                { AttributeType.AttackSpeed, 10 },
                { AttributeType.AttackRange, 10 },
            };

            public const float HPPenaltyRatio = 0.3f;

            [Description("This adjusts the Speed stat by multiplying it before calculating Position for rigidBody")]
            public const float MoveSpeedFactor = 3f;
            [Description("It determines the character's running speed by multiplying it with MoveSpeed")]
            public const float RunSpeedFactor = 1.5f;

            public const float DamageEffectDuration = 0.15f;
            
            public const float ProjectileValidTime = 10f;
            public const float ProjectileValidRange = 20f;
            public const float PatrolRange = 3f;
            
            public const float PlayerColliderAttackAngle = 45f;
            public const float PlayerColliderAttackDelay = 0.5f;
            public const float PlayerColliderAttackCoolTime = 0.5f;
            public const float PlayerHitScanAttackRange = 6f;
            public const float PlayerHitScanAttackDelay = 0f;
            public const float PlayerHitScanAttackCoolTime = 0.5f;
        }

        public static class Combat
        {
            public static readonly Dictionary<AttackType, float> FOVIdleRadius = new()
            {
                { AttackType.RangeSingle, 3f },
                { AttackType.Projectile, 6f },
            };
            public static readonly Dictionary<AttackType, float> FOVIdleAngle = new()
            {
                { AttackType.RangeSingle, 120f },
                { AttackType.Projectile, 200f },
            };
            
            public static readonly Dictionary<AttackType, float> FOVDetectRadius = new()
            {
                { AttackType.RangeSingle, 3f },
                { AttackType.Projectile, 6f },
            };
            public static readonly Dictionary<AttackType, float> FOVDetectAngle = new()
            {
                { AttackType.RangeSingle, 120f },
                { AttackType.Projectile, 200f },
            };
        }
        
        public static class MoveDir
        {
            public static readonly Vector2 None = Vector2.zero;
            public static readonly Vector2 Up = Vector2.up;
            public static readonly Vector2 Down = Vector2.down;
            public static readonly Vector2 Left = Vector2.left;
            public static readonly Vector2 Right = Vector2.right;
            public static readonly Vector2 UpLeft = new Vector2(-2, 1).normalized;
            public static readonly Vector2 UpRight = new Vector2(2, 1).normalized;
            public static readonly Vector2 DownLeft = new Vector2(-2, -1).normalized;
            public static readonly Vector2 DownRight = new Vector2(2, -1).normalized;
        }
        
        public static class Effect
        {
            public const int FullReferenceValue = 80;
            public const int ModerateReferenceValue = 50;
            public const int StarvingReferenceValue = 10;
            
            public const int HydratedReferenceValue = 70;
            public const int ThirstyReferenceValue = 10;
        }
        
        public static class Animation
        {
            public const float MoveSpeedFactor = 0.1f;
        }

        public static class Time
        {
            public const int StartHour = 6;
            
            public const int HoursInADay = 24;
            public const int MinutesInAHour = 60;
            public const int MinutesInADay = MinutesInAHour * HoursInADay;

            [Description("How long 1 hour of Game Time takes in Real Time seconds")]
            public const float SecondsPerHour = 75; // default == 75

            public const float SecondsPerMinute = SecondsPerHour / 60;
            public const float MinInputGap = 0.05f;
        }

        public const int FastPathPenaltyHour = 3;
        public const float InteractDelay = 2f;

        public static class Delay
        {
            public const float TooltipDelay = 0.5f;
            public const float AfterChaseDelay = 1f;
            public const float DeadAnimationDelay = 1f;
        }

        public const float Tolerance = 0.05f;
        public const float IsometricPositionTolerance = 0.42f;
        public const float CullingRadius = 40f;
    }
}