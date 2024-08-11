using EventSystem;
using InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class MouseTriggerUI : UIBase
    {
        public abstract override UIType GetUIType();
        protected abstract bool HasEnterExitTrigger { get; }

        public override void Init()
        {
            base.Init();
            
            if (HasEnterExitTrigger)
            {
                EventManager.AddEventTrigger(gameObject
                    , EventTriggerType.PointerEnter
                    , _ => OnMouseEnter());
                EventManager.AddEventTrigger(gameObject
                    , EventTriggerType.PointerExit
                    , _ => OnMouseExit());
            }
            EventManager.AddEventTrigger(gameObject
                , EventTriggerType.PointerClick
                , _ => OnMouseClick());
        }

        protected virtual void OnMouseEnter()
        {
        }

        protected virtual void OnMouseExit()
        {
        }

        protected virtual void OnMouseClick()
        {
            Debug.Log("Click " + gameObject.name);
            MouseData.FocusedUI = this;
        }
    }
}