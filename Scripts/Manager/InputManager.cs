using System;
using System.Collections;
using DataSystem;
using Event;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UserActionBind;

namespace Manager
{
    public class InputManager : MonoBehaviour
    {
        #region singleton.

        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<InputManager>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        #endregion

        private const int RightMouse = Constants.Input.RightMouse;
        private const int LeftMouse = Constants.Input.LeftMouse;
        
        public bool IsFunctionDown { get; private set; }
        private Action OnFunctionRelease => () => IsFunctionDown = false;
        
        public Vector3 MousePosition { get; set; }
        private Action OnRightMouseRelease => () => EventManager.OnNext(Message.OnRightMouseUp);

        private void OnAwake()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.anyKey)
                .Subscribe(_ =>
                {
                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.Inventory]))
                    {
                        ToggleUI(UIType.PlayerInventoryUI);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.Interact]))
                    {
                        EventManager.OnNext(Message.OnTryInteract);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.Escape]))
                    {
                        EventManager.OnNext(Message.OnPressEscape);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.QuickSlot1]))
                    {
                        EventManager.OnNext(Message.OnTryItemUse, DataManager.QuickSlots[0].Item.id);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.QuickSlot2]))
                    {
                        EventManager.OnNext(Message.OnTryItemUse, DataManager.QuickSlots[1].Item.id);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.QuickSlot3]))
                    {
                        EventManager.OnNext(Message.OnTryItemUse, DataManager.QuickSlots[2].Item.id);
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.Function]))
                    {
                        IsFunctionDown = true;
                        StartCoroutine(DetectKeyRelease(InputBinding.Bindings[UserAction.Function], OnFunctionRelease));
                    }

                    if (Input.GetKeyDown(InputBinding.Bindings[UserAction.ChangeWeapon]))
                    {
                        EventManager.OnNext(Message.OnPressChangeWeapon);
                    }

                    if (Input.GetMouseButtonDown(LeftMouse))
                    {
                        EventManager.OnNext(Message.OnLeftMouseDown);
                    }
                    if (Input.GetMouseButtonDown(RightMouse))
                    {
                        EventManager.OnNext(Message.OnRightMouseDown);
                        StartCoroutine(DetectMouseRelease(RightMouse, OnRightMouseRelease));
                    }
                });
        }

        public static void StartDetectingRelease(KeyCode key, Action onReleased)
        {
            Instance.StartCoroutine(DetectKeyRelease(key, onReleased));
        }
        
        public static void StartDetectingRelease(int button, Action onReleased)
        {
            Instance.StartCoroutine(DetectMouseRelease(button, onReleased));
        }

        private static void ToggleUI(UIType type)
        {
            if (UIManager.Instance.IsOpened(type))
            {
                UIManager.Instance.Close(type);
            }
            else
            {
                UIManager.Instance.Open(type);
            }
        }

        private static IEnumerator DetectKeyRelease(KeyCode key, Action onReleased)
        {
            while (true)
            {
                if (Input.GetKey(key))
                {
                    yield return null;
                    continue;
                }

                onReleased.Invoke();
                break;
            }
        }
        
        private static IEnumerator DetectMouseRelease(int button, Action onReleased)
        {
            while (true)
            {
                if (Input.GetMouseButton(button))
                {
                    yield return null;
                    continue;
                }

                onReleased.Invoke();
                break;
            }
        }
    }
}