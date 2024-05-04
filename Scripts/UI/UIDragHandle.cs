using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(EventTrigger))]
    public class UIDragHandle : MonoBehaviour
    {
        private RectTransform _parentTransform;

        private Vector2 _rectBegin;
        private Vector2 _moveBegin;

        private void Awake()
        {
            EventManager.AddEventTrigger(gameObject, EventTriggerType.BeginDrag, _ => OnBeginDrag());
            EventManager.AddEventTrigger(gameObject, EventTriggerType.Drag, _ => OnDrag());
            
            _parentTransform = transform.parent.GetComponent<RectTransform>();
        }

        private void OnBeginDrag()
        {
            _rectBegin = _parentTransform.position;
            _moveBegin = Input.mousePosition;
        }

        private void OnDrag()
        {
            _parentTransform.position = _rectBegin + (Vector2)Input.mousePosition - _moveBegin;
        }
    }
}