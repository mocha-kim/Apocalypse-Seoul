using System;
using EventSystem;
using UnityEngine;

namespace UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public abstract UIType GetUIType();
        public bool IsInit { get; private set; } = false;
        public bool IsOpen { get; private set; } = false;

        public virtual void Init()
        {
            if (IsInit)
            {
                return;
            }
            
            IsInit = true;
        }

        public virtual void Open()
        {
            IsOpen = true;
            gameObject.SetActive(true);
            EventManager.OnNext(Message.OnUIOpened, GetUIType());
        }

        public virtual void Close()
        {
            IsOpen = false;
            gameObject.SetActive(false);
            EventManager.OnNext(Message.OnUIClosed, GetUIType());
        }

        private void OnMouseEnter()
        {
            throw new NotImplementedException();
        }
    }
}