using System;
using DataSystem;
using UnityEngine;

namespace EnvironmentSystem.Area
{
    [RequireComponent(typeof(Collider2D))]
    public class Stairs : MonoBehaviour
    {
        public Action OnFadeNextFloor;

        private void Start()
        {
            if (OnFadeNextFloor == null)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != Constants.Layer.PlayerCollider)
            {
                return;
            }
            
            OnFadeNextFloor.Invoke();
        }
    }
}