using System;
using Core.Interface;
using DataSystem;
using JetBrains.Annotations;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [CanBeNull] public InteractableAlertUI AlertUI { get; protected set; }
        [CanBeNull] private ConditionAlertUI _conditionUI;
        
        protected virtual void Awake()
        {
            AlertUI = GetComponentInChildren<InteractableAlertUI>();
            
            _conditionUI = GetComponentInChildren<ConditionAlertUI>();
            if (_conditionUI != null)
            {
                _conditionUI.gameObject.SetActive(false);
            }
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public abstract void Interact();
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != Constants.Layer.Player)
            {
                return;
            }
            
            if (AlertUI != null)
            {
                AlertUI.SetInteractable(true);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != Constants.Layer.Player)
            {
                return;
            }
            
            if (AlertUI != null)
            {
                AlertUI.SetInteractable(false);
            }
        }

        protected virtual void SetConditionData(out string iconPath, out string text)
        {
            iconPath = "";
            text = "";
        }

        protected void NotifyInteractCondition()
        {
            if (_conditionUI == null)
            {
                return;
            }
            
            StopAllCoroutines();
            
            SetConditionData(out var iconPath, out var text);
            _conditionUI.SetData(iconPath, text);
            _conditionUI.Show();
        }

    }
}