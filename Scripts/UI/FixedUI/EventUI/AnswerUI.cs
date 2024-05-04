using Event;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UserActionBind;

namespace UI.FixedUI.EventUI
{
    public class AnswerUI : UIBase
    {
        private bool _isKeyboardEnabled;
        private int _focusedIndex;

        [SerializeField] private GameObject _focusCursor;
        
        private TextMeshProUGUI[] _answerTexts;
        private Button[] _answerButtons;
        
        private string[] _answers;
        private UIHeightController _heightController;
        
        public override UIType GetUIType() => UIType.ComponentUI;

        public override void Init()
        {
            if (IsInit)
            {
                return;
            }

            _answerTexts = GetComponentsInChildren<TextMeshProUGUI>();
            _answerButtons = GetComponentsInChildren<Button>();
            for (var i = 0; i < 4; i++)
            {
                var id = i;
                _answerButtons[i].onClick.AddListener(() => OnSelectAnswer(id, true));
            }

            _heightController = GetComponent<UIHeightController>();

            _isKeyboardEnabled = false;
            _focusedIndex = -1;
            this.UpdateAsObservable()
                .Where(_ => Input.anyKey)
                .Subscribe(_ =>
                {
                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.MoveUp]))
                    {
                        OnFocusChanged(true);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.MoveDown]))
                    {
                        OnFocusChanged(false);
                    }
                });
            _focusCursor.SetActive(false);

            EventManager.Subscribe(gameObject, Message.OnNextEventUI,
                _ => OnSelectAnswer(_focusedIndex, _isKeyboardEnabled));
            base.Init();
            Close();
        }

        public override void Open()
        {
            if (_answers == null)
            {
                Close();
                return;
            }
            
            _isKeyboardEnabled = false;
            _focusedIndex = -1;
            
            gameObject.SetActive(true);
            _heightController.SetActiveChildCount(_answers.Length);
        }

        public override void Close()
        {
            _answers = null;
            _heightController.SetActiveChildCount(0);
            gameObject.SetActive(false);
        }

        public void SetAnswers(string[] answers)
        {
            _answers = answers;
            for (var i = 0; i < _answers.Length; i++)
            {
                _answerTexts[i].text = _answers[i];
            }

            _heightController.SetActiveChildCount(_answers.Length);
        }
        
        // up: TRUE, down: FALSE
        private void OnFocusChanged(bool direction)
        {
            if (!_isKeyboardEnabled)
            {
                _isKeyboardEnabled = true;
                _focusedIndex = direction ? -1 : _answers.Length;
                _focusCursor.SetActive(true);
            }

            _focusedIndex += direction ? 1 : - 1;
            _focusedIndex = Mathf.Clamp(_focusedIndex, 0, _answers.Length - 1);
            _focusCursor.transform.position = _answerButtons[_focusedIndex].transform.position;
        }

        private void OnSelectAnswer(int id, bool isValidSignal)
        {
            if (!isValidSignal || _answers == null)
            {
                return;
            }
            EventManager.OnNext(Message.OnAnswerSelected, _answers[id]);
        }
    }
}