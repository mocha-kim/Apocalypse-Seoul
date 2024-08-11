using System;
using Core.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterSystem.Character.Combat
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] private Character _context = null;
        public IDamageable Damageable { get; set; }
        public IAffectable Affectable { get; set; }

        private void Awake()
        {
            Damageable = _context.GetComponent<IDamageable>();
            Affectable = _context.GetComponent<IAffectable>();
        }
    }
}