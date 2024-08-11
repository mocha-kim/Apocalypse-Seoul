using System.Collections.Generic;
using CharacterSystem.Character;
using DataSystem;
using DataSystem.Database;
using DialogSystem;
using EventSystem;
using UI;
using UI.FixedUI.EventUI;
using UI.InGameUI;
using UnityEngine;

namespace InteractableObject
{
    public class NPCObject : InteractableObject
    {
        public int ID;
        private NPC _data;
        
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private ObjectDir _direction;

        private bool _isUIOpened = false;
        private EventControlUI _eventControlUI;
        private Animator _animator;
        private int _hashDirection;

        private void Start()
        {
            if (ID == -1)
            {
                gameObject.SetActive(false);
                return;
            }

            _data = Database.GetNPC(ID);
            
            _renderer.sprite = ResourceManager.GetSprite(_data.spritePath);
            _data.Init();

            _animator = _renderer.GetComponent<Animator>();
            if (_animator != null)
            {
                _hashDirection = Animator.StringToHash("Direction");
                _animator.SetInteger(_hashDirection, (int)_direction);
            }
            
            EventManager.Subscribe(gameObject, Message.OnUIClosed, OnUIClosed);
        }
        
        public override void Interact()
        {
            _eventControlUI = (EventControlUI)UIManager.Instance.Get(UIType.EventUI);
            if (!_isUIOpened && (!_eventControlUI || !_eventControlUI.IsOpen))
            {
                _isUIOpened = true;
                UIManager.Instance.Open(UIType.EventUI);
                _eventControlUI = (EventControlUI)UIManager.Instance.Get(UIType.EventUI);
                _eventControlUI.StartEvent(Database.GetScriptList(_data.id, 0), _data.id);
            }
            else
            {
                EventManager.OnNext(Message.OnNextEventUI);
            }
        }

        private void OnUIClosed(EventManager.Event e)
        {
            UIType type = (UIType)e.Args[0];
            if (type == UIType.DialogUI)
            {
                _isUIOpened = false;
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            base.OnTriggerExit2D(other);
            
            UIManager.Instance.Close(UIType.DialogUI);
            _isUIOpened = false;
        }
    }

}