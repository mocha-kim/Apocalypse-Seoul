using System;
using CharacterSystem.Character.Combat;
using CharacterSystem.Stat;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;

namespace CharacterSystem.Character.Enemy
{
    [Serializable]
    public class Enemy
    {
        public int id = -1;
        public string name = "";
        public AttackType attacktype;

        public string spritePath;
        public EnemyStat stat;

        public Enemy(int id, string name, AttackType type, string spritePath
            , int hp, int speed, int attack, int defense, int attackSpeed, int attackRange)
        {
            this.id = id;
            this.attacktype = type;
            this.name = name;

            this.spritePath = spritePath;
            stat = new EnemyStat(hp, speed, attack, defense, attackSpeed, attackRange);
        }
    }
}