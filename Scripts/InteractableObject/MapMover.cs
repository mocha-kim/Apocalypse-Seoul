using Event;
using Manager;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    public class MapMover : InteractableObject
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            EventManager.OnNext(Message.OnReadyMapMove, true);
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            EventManager.OnNext(Message.OnReadyMapMove, false);
        }

        public override void Interact()
        {
            // This object can be interacted by UI button
        }
    }
}