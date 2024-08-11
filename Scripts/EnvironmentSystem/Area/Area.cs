using System.Collections;
using DataSystem;
using EventSystem;
using UnityEngine;

namespace EnvironmentSystem.Area
{
    public class Area : MonoBehaviour
    {
        [SerializeField] private string _name = "";

        [SerializeField] private GameObject[] _objectsToHide;

        protected virtual void Awake()
        {
            if (name == "")
            {
                name = gameObject.name;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != Constants.Layer.PlayerCollider)
            {
                return;
            }
            
            EventManager.OnNext(Message.OnAreaEnter);
            foreach (var objects in _objectsToHide)
            {
                objects.SetActive(false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != Constants.Layer.PlayerCollider)
            {
                return;
            }
            
            EventManager.OnNext(Message.OnAreaExit);
            foreach (var objects in _objectsToHide)
            {
                objects.SetActive(true);
            }
        }
    }
}