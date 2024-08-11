using System;
using System.Collections;
using DataSystem;
using EventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.FloatingUI.Tooltip.Target
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class TooltipTarget<T> : MonoBehaviour
    {
        private WaitForSeconds _wait = new(Constants.Delay.TooltipDelay);
        
        private void Awake()
        {
            EventManager.AddEventTrigger(gameObject, EventTriggerType.PointerEnter, _ => OnMouseEnter());
            EventManager.AddEventTrigger(gameObject, EventTriggerType.PointerExit, _ => OnMouseExit());
        }

        private void OnMouseEnter()
        {
            if (GetTarget() == null)
            {
                return;
            }
            StartCoroutine(WaitAndOpen());
        }

        private IEnumerator WaitAndOpen()
        {
            yield return _wait;
            
            EventManager.OnNext(Message.OnTryTooltipOpen, GetTarget());
        }

        private void OnMouseExit()
        {
            StopAllCoroutines();
            EventManager.OnNext(Message.OnTryTooltipClose);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            EventManager.OnNext(Message.OnTryTooltipClose);
        }

        protected abstract T GetTarget();
    }
}