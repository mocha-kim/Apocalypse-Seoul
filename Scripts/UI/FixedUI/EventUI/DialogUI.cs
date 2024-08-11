using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem.Database;
using DialogSystem;
using EventSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI.EventUI
{
    public class DialogUI : EventUIBase
    {
        private bool _isLeftSpeakerExist = false;
        private TextMeshProUGUI _leftNameText;
        [SerializeField] private GameObject _leftName;
        [SerializeField] private Image _leftImage;

        private bool _isRightSpeakerExist = false;
        private TextMeshProUGUI _rightNameText;
        [SerializeField] private GameObject _rightName;
        [SerializeField] private Image _rightImage;
        
        private int _curScriptId;
        private int _nextScriptId;
        private Dictionary<int, Script> _scripts;
        private CanvasGroup _canvasGroup;

        private bool _isTyping;
        [SerializeField] private TextMeshProUGUI _dialogText;
        private readonly WaitForSeconds _waitTypingDelay = new(0.05f);
        private Coroutine _enabledFadeCoroutine;
        
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _nextImage;

        private bool _isWaitingAnswer;
        private bool _hasSelectedAnswer;
        [SerializeField] private AnswerUI _answerUI;


        public override UIType GetUIType() => UIType.DialogUI;

        public override void Init()
        {
            base.Init();
            _nextButton.OnClickAsObservable().Subscribe(_ =>
            {
                EventManager.OnNext(Message.OnNextEventUI);
            }).AddTo(gameObject);

            _leftNameText = _leftName.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _rightNameText = _rightName.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            EventManager.Subscribe(gameObject, Message.OnAnswerSelected, OnAnswerSelected);
            _answerUI.Init();
        }

        public override void Open()
        {
            base.Open();
            _answerUI.gameObject.SetActive(false);
            
            _isTyping = false;
            _isWaitingAnswer = false;
            _hasSelectedAnswer = false;
            _dialogText.text = string.Empty;
            
            _isLeftSpeakerExist = false;
            _isRightSpeakerExist = false;
            
            _canvasGroup.alpha = 0f; 
            _leftImage.color = new Color(1, 1, 1, 0); 
            _rightImage.color = new Color(1, 1, 1, 0);
            SetComponentsActive(true, false, false);
        }
        
        public override void Close()
        {
            _isLeftSpeakerExist = false;
            _isRightSpeakerExist = false;
            
            base.Close();
        }

        public override int Next(int curId)
        {
            if (_isWaitingAnswer)
            {
                return curId;
            }
            
            if (curId == -1)
            {
                Close();
                return -1;
            }

            _curScriptId = _hasSelectedAnswer ? _nextScriptId : curId;
            _hasSelectedAnswer = false;
            UpdateDialogue();
            return _nextScriptId;
        }

        public int StartDialogue(Dictionary<int, Script> scripts, int curId)
        {
            _scripts = scripts;

            return Next(curId);
        }

        private void UpdateDialogue()
        {
            if (_isTyping)
            {
                OnTextCompleted();
                return;
            }
            
            StopAllCoroutines();
            _nextImage.SetActive(false);
            _dialogText.text = string.Empty;
            _nextScriptId = _scripts[_curScriptId].nextId;

            var script = _scripts[_curScriptId];
            if (script.type != ScriptType.Dialog)
            {
                Close();
                return;
            }

            var npc = Database.GetNPC(script.speakerId);
            if (script.isRightSpeaker)
            {
                _isRightSpeakerExist = true;
                _rightImage.sprite = npc.portraitImage;
                _rightNameText.text = npc.name;
            }
            else
            {
                _isLeftSpeakerExist = true;
                _leftImage.sprite = npc.portraitImage;
                _leftNameText.text = npc.name;
            }
            
            SetComponentsActive(true, !script.isRightSpeaker, script.isRightSpeaker);
            StartCoroutine(TypeText());
        }

        private void SetComponentsActive(bool isCanvasGroupEnabled, bool isLeftImageEnabled, bool isRightImageEnabled)
        {
            if (_enabledFadeCoroutine != null)
            {
                StopCoroutine(_enabledFadeCoroutine);
            }
            _enabledFadeCoroutine = StartCoroutine(FadeImageAlpha(isCanvasGroupEnabled, isLeftImageEnabled, isRightImageEnabled));
        }
        
        private IEnumerator FadeImageAlpha(bool isCanvasGroupEnabled, bool isLeftImageEnabled, bool isRightImageEnabled)
        {
            _leftName.SetActive(isLeftImageEnabled);
            _rightName.SetActive(isRightImageEnabled);

            var canvasGroupAlpha = isCanvasGroupEnabled ? 1f : 0f;
            var leftImageAlpha = (isLeftImageEnabled ? 1f : 0.6f) * Convert.ToInt32(_isLeftSpeakerExist);
            var rightImageAlpha = (isRightImageEnabled ? 1f : 0.6f) * Convert.ToInt32(_isRightSpeakerExist);
            const float tolerance = 0.001f;
            while (true)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, canvasGroupAlpha, 0.1f);
                _leftImage.color = Color.Lerp(_leftImage.color, new Color(1, 1, 1, leftImageAlpha), 0.1f);
                _rightImage.color = Color.Lerp(_rightImage.color, new Color(1, 1, 1, rightImageAlpha), 0.1f);
                if (Math.Abs(_canvasGroup.alpha - canvasGroupAlpha) < tolerance
                    && Math.Abs(_leftImage.color.a - leftImageAlpha) < tolerance
                    && Math.Abs(_rightImage.color.a - rightImageAlpha) < tolerance)
                {
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator TypeText()
        {
            _isTyping = true;
            _dialogText.text = string.Empty;
            var charsToType = _scripts[_curScriptId].script.ToCharArray();
            
            for (var i = 0; i < charsToType.Length; i++)
            {
                switch (charsToType[i])
                {
                    case '[':
                        var delay = "";
                        i++;
                        while (charsToType[i] != ']')
                        {
                            delay += charsToType[i];
                            i++;
                        }
                        yield return new WaitForSeconds(float.Parse(delay));
                        break;
                    default:
                        _dialogText.text += charsToType[i];
                        break;
                }

                yield return _waitTypingDelay;
                if (_isTyping)
                {
                    continue;
                }
                
                _dialogText.text = "";
                for (i = 0; i < charsToType.Length; i++)
                {
                    if (charsToType[i] == '[')
                    {
                        while (charsToType[i] != ']')
                        {
                            i++;
                        }
                    }
                    else
                    {
                        _dialogText.text += charsToType[i];
                    }
                }
                break;
            }
            OnTextCompleted();
        }

        private void OnTextCompleted()
        {
            if (_isWaitingAnswer)
            {
                return;
            }
            
            _nextImage.SetActive(true);
            _isTyping = false;
            
            if (_scripts[_curScriptId].answers == null || _scripts[_curScriptId].answers.Count < 1)
            {
                return;
            }
            _isWaitingAnswer = true;
            _hasSelectedAnswer = false;
            _answerUI.SetAnswers(_scripts[_curScriptId].answers.Keys.ToArray());
            _answerUI.Open();
        }
        
        private void OnAnswerSelected(EventManager.Event e)
        {
            if (e.Args.Length < 1)
            {
                return;
            }

            _nextScriptId = _scripts[_curScriptId].answers[(string)e.Args[0]];
            _isWaitingAnswer = false;
            _hasSelectedAnswer = true;
            _answerUI.Close();
            EventManager.OnNext(Message.OnNextEventUI);
        }
    }
}
