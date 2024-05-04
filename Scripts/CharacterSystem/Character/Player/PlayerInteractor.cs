using Core.Interface;
using Event;
using Manager;
using UnityEngine;

namespace CharacterSystem.Character.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        private IInteractable _target = null;

        private void Awake()
        {
            EventManager.Subscribe(gameObject, Message.OnTryInteract, OnTryInteract);
        }

        private void OnTryInteract(EventManager.Event obj)
        {
            _target?.Interact();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var temp = other.GetComponent<IInteractable>();
            if (temp == null)
            {
                return;
            }

            _target = temp;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var temp = other.GetComponent<IInteractable>();
            if (temp == null)
            {
                return;
            }

            _target = null;
        }
    }
}