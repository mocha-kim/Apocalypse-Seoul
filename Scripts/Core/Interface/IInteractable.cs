using UI.InGameUI;

namespace Core.Interface
{
    public interface IInteractable
    {
        public InteractableAlertUI AlertUI { get; }
        public void Interact();
    }
}