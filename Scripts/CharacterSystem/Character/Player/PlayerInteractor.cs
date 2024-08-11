using Core.Interface;
using EventSystem;
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
            var target = other.GetComponent<IInteractable>();
            if (target == null)
            {
                return;
            }

            _target = target;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var target = other.GetComponent<IInteractable>();
            if (target == null || target != _target)
            {
                return;
            }

            _target = null;
        }
    }
}