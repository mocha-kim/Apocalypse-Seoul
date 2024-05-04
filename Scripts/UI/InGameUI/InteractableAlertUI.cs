 using UnityEngine;

namespace UI.InGameUI
{
    public class InteractableAlertUI : MonoBehaviour
    {
        private GameObject _render;
        private Animator _animator;
        private int _hashIsInteractable;

        private void Awake()
        {
            _render = transform.GetChild(0).gameObject;
            _animator = _render.GetComponent<Animator>();
            _hashIsInteractable = Animator.StringToHash("IsInteractable");
            
            _render.SetActive(false);
        }

        public void SetInteractable(bool value)
        {
            _render.SetActive(value);
            _animator.SetBool(_hashIsInteractable, value);
        }
    }
}