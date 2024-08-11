using System.Collections.Generic;
using DialogSystem;
using EventSystem;
using UI.FixedUI.EventUI.Deal;
using UnityEngine;

namespace UI.FixedUI.EventUI
{
    public class EventControlUI : UIBase
    {
        private int _npcId;

        private int _curScriptId;
        private Dictionary<int, Script> _scripts;
        private EventUIBase _openedEventUI;

        private bool _isReadyToClose;
        
        public override UIType GetUIType() => UIType.EventUI;

        public override void Init()
        {
            base.Init();
            EventManager.Subscribe(gameObject, Message.OnPressEscape, _ => EndEvent());
            EventManager.Subscribe(gameObject, Message.OnNextEventUI, _ => NextEvent());
            EventManager.Subscribe(gameObject, Message.OnEventUIClosed, _ => OnEventUIClosed());
        }

        public void StartEvent(ScriptList scriptList, int npcId)
        {
            _npcId = npcId;
            _isReadyToClose = false;

            _scripts = scriptList.scripts;

            _curScriptId = 0;
            NextEvent();
            UIManager.Instance.CloseMainUI();
        }

        private void NextEvent()
        {
            if (_isReadyToClose)
            {
                return;
            }
            if(_curScriptId == -1)
            {
                EndEvent();
                return;
            }

            if (!_openedEventUI || !_openedEventUI.IsOpen)
            {
                switch (_scripts[_curScriptId].type)
                {
                    case ScriptType.Dialog:
                        _openedEventUI = (EventUIBase)UIManager.Instance.Open(UIType.DialogUI);
                        _curScriptId = ((DialogUI)_openedEventUI).StartDialogue(_scripts, _curScriptId);
                        break;
                    case ScriptType.Merchant:
                        _curScriptId = _scripts[_curScriptId].nextId;
                        _openedEventUI = (EventUIBase)UIManager.Instance.Get(UIType.DealUI);
                        ((DealUI)_openedEventUI).SetEventInfo(_npcId, _curScriptId);
                        _openedEventUI.Open();
                        break;
                    case ScriptType.Technician:
                        _curScriptId = _scripts[_curScriptId].nextId;
                        _openedEventUI = (EventUIBase)UIManager.Instance.Get(UIType.ProducerUpgradeUI);
                        ((ProducerUpgradeUI)_openedEventUI).SetEventInfo(_curScriptId);
                        _openedEventUI.Open();
                        break;
                }
            }
            else
            {
                var tmpId = _openedEventUI.Next(_curScriptId);
                if (_curScriptId == -1 || tmpId == -1)
                {
                    _curScriptId = -1;
                }
                else if (tmpId > _curScriptId)
                {
                    _curScriptId = tmpId;
                }
            }
        }

        private void EndEvent()
        {
            _isReadyToClose = true;
            if (_openedEventUI && _openedEventUI.IsOpen)
            {
                _openedEventUI.Close();
            }
            UIManager.Instance.OpenMainUI();
            
            base.Close();
        }

        private void OnEventUIClosed()
        {
            _openedEventUI = null;
            
            EventManager.OnNext(Message.OnNextEventUI);
        }
    }
}