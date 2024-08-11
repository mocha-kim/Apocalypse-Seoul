using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using InputSystem.UserActionBind;
using Settings;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class SettingUI : UIBase
    {
        [SerializeField] GameObject _waitingInputGo;
        [SerializeField] Transform _verticalLayoutTr;
        [SerializeField] GameObject _bindingPairPrefab;
        [SerializeField] Button _closeButton;
        [SerializeField] Button _resetButton;
        [SerializeField] Button _btnQuit;
        [SerializeField] Button _openSaveLoadUI;
        
        public override UIType GetUIType() => UIType.SettingUI;
        private List<GameObject> _bindingPairGoList = new();
        private Dictionary<UserAction, BindingPairUI> _bindingPairDict = new();

        private bool _isListening;
        private UserAction _curKeyAction;
        
        private void Awake()
        {
            _btnQuit.OnClickAsObservable().Subscribe(_ =>
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif

            }).AddTo(gameObject);
            
            _closeButton.OnClickAsObservable().Subscribe(_ =>
            {
                Close();
            }).AddTo(gameObject);

            _resetButton.OnClickAsObservable().Subscribe(_ =>
            {
                InputBinding.SetDefaultBind();
                RefreshAllBindingUIs();
            }).AddTo(gameObject);
            
            _openSaveLoadUI.OnClickAsObservable().Subscribe(_ =>
            {
                UIManager.Instance.Open(UIType.SaveLoadUI);
            }).AddTo(gameObject);
            
            EventManager.Subscribe(gameObject, Message.OnRefreshAllBindingUIs, _ => RefreshAllBindingUIs());
        }

        private void Update()
        {
            if (_isListening)
            {
                if (ListenInput(out var keyCode))
                {
                    SetKeyBinding(_curKeyAction, keyCode);
                    _isListening = false;
                }
            }

            _waitingInputGo.SetActive(_isListening);
        }

        public override void Open()
        {
            base.Open();
            GameManager.Instance.PauseGame();
            
            _isListening = false;
            _waitingInputGo.SetActive(false);
            LoadInputBindings();
        }

        public override void Close()
        {
            base.Close();
            GameManager.Instance.ResumeGame();
            
            _isListening = false;
            _waitingInputGo.SetActive(false);
        }

        private bool ListenInput(out KeyCode code)
        {
            foreach (var curCode in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
            {
                if (Input.GetKeyDown(curCode))
                {
                    code = curCode;
                    return true;
                }
            }

            code = KeyCode.None;
            return false;
        }

        private void LoadInputBindings()
        {
            int count = 0;

            // 1. Reset
            foreach (var go in _bindingPairGoList)
            {
                Destroy(go);
            }

            _bindingPairDict.Clear();
            _bindingPairGoList.Clear();


            // 2. Load Pairs
            foreach (var pair in InputBinding.Bindings)
            {
                var pairGo = Instantiate(_bindingPairPrefab, _verticalLayoutTr) as GameObject;
                var pairUI = pairGo.GetComponent<BindingPairUI>();

                pairUI.InitLabels($"{pair.Key}", $"{pair.Value}");
                pairUI.AddButtonListener(() =>
                {
                    pairUI.Select();
                    _isListening = true;
                    _curKeyAction = pair.Key;
                });

                _bindingPairDict.Add(pair.Key, pairUI);
                _bindingPairGoList.Add(pairGo);
                count++;
            }

            // Resize Vertical Layout Height
            _verticalLayoutTr.TryGetComponent(out RectTransform rt);
            if (rt)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x,
                    40 +
                    count * 60 +
                    (count - 1) * 10
                );
            }
        }

        private void SetKeyBinding(UserAction action, KeyCode code)
        {
            InputBinding.Bind(action, code);
            RefreshAllBindingUIs();
        }

        private void RefreshAllBindingUIs()
        {
            foreach (var pair in InputBinding.Bindings)
            {
                _bindingPairDict[pair.Key].SetCodeLabel($"{pair.Value}");
                _bindingPairDict[pair.Key].Deselect();
            }
        }
    }
}
