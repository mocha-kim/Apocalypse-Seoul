using System.Collections.Generic;
using System.Linq;
using DataSystem;
using EventSystem;
using ItemSystem.Produce;
using UI.FixedUI;
using UI.FloatingUI.Inventory;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        #region singleton
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<UIManager>();
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

        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Canvas _dynamicCanvas;
        [SerializeField] private Canvas _otherCanvas;
        
        public Transform MainCanvas => _mainCanvas.transform;
        public Transform DynamicCanvas => _dynamicCanvas.transform;
        public Transform OtherCanvas => _otherCanvas.transform;

        private List<UIBase> _activeUIs = new();
        private List<UIBase> _cachedUIs = new();
        public static int OpenItemBoxID = -1;
        public static int OpenProducerId = -1;
        public static ProducerType? OpenProducerType = null;

        private List<GameObject> _cachedTooltips = new();

        public List<InventoryUI<ItemSlotUI>> ActiveInventories { get; private set; } = new();
        
        private CanvasGroup _mainUICanvasGroup;

        private void OnAwake()
        {
            // mainUI init
            var go = Instantiate(
                ResourceManager.GetPrefab(UIType.MainUI.ToString())
                , _mainCanvas.transform);
            go.GetComponent<MainUI>().Init();

            _mainUICanvasGroup = go.GetComponent<CanvasGroup>();

            // tooltips init
            _cachedTooltips.Add
            (
                Instantiate(
                    ResourceManager.GetPrefab(UIType.ItemTooltipUI.ToString())
                    , _otherCanvas.transform)
            );

            foreach (var tooltip in _cachedTooltips)
            {
                tooltip.transform.GetChild(0).gameObject.SetActive(false);
            }

            // subscribe events.
            EventManager.Subscribe(gameObject, Message.OnUIOpened, OnUIOpened);
            EventManager.Subscribe(gameObject, Message.OnUIClosed, OnUIClosed);
            EventManager.Subscribe(gameObject, Message.OnPressEscape, OnPressEscape);
            EventManager.Subscribe(gameObject, Message.OnTrySceneLoad, OnTrySceneLoad);
        }

        public UIBase Open(UIType uiType)
        {
            UIBase ui = Get(uiType);
            if (ui == null)
            {
                return null;
            }
            
            if (!ui.IsOpen)
            {
                ui.transform.SetAsLastSibling();
                ui.Open();
            }
            return ui;
        }

        public void Close(UIType uiType)
        {
            UIBase ui = Get(uiType);
            if (ui == null)
            {
                return;
            }
            
            if (ui.IsOpen)
            {
                ui.Close();
            }
        }
        
        public void CloseAll()
        {
            for (int i = 0; i < _cachedUIs.Count; i++)
            {
                if (_cachedUIs[i].IsOpen)
                {
                    UIBase ui = _cachedUIs[i];
                    ui.Close();
                }
            }
        }

        public UIBase Get(UIType uiType)
        {
            if (uiType < 0 || _dynamicCanvas == null)
            {
                return null;
            }
            
            foreach (var cachedUI in _cachedUIs)
            {
                if (cachedUI.GetUIType() == uiType)
                {
                    return cachedUI;
                }
            }

            var prefab = ResourceManager.GetPrefab(uiType.ToString());
            var go = Instantiate(prefab, _dynamicCanvas.transform);
            go.SetActive(false);
            
            var ui = go.GetComponent<UIBase>();
            ui.Init();
            
            _cachedUIs.Add(ui);
            return ui;
        }

        public bool IsOpened(UIType uiType)
        {
            var ui = Get(uiType);
            return ui != null && ui.IsOpen;
        }
        
        private void OnUIOpened(EventManager.Event e)
        {
            UIType type = (UIType)e.Args[0];
            if (type == UIType.ToastUI)
            {
                return;
            }
            
            var ui = Get(type);
            if (!_activeUIs.Contains(ui) && ui != null)
            {
                _activeUIs.Add(ui);
            }

            if (ui is InventoryUI<ItemSlotUI> && !ActiveInventories.Contains(ui))
            {
                ActiveInventories.Add(ui as InventoryUI<ItemSlotUI>);
            }
            if (_activeUIs.Count > 0)
            {
                EventManager.OnNext(Message.OnCharacterBusy);
            }
        }

        private void OnUIClosed(EventManager.Event e)
        {
            UIType type = (UIType)e.Args[0];
            if (type == UIType.ToastUI)
            {
                return;
            }
            
            var ui = Get(type);
            _activeUIs.Remove(ui);
            
            if (ui is InventoryUI<ItemSlotUI>)
            {
                ActiveInventories.Remove(ui as InventoryUI<ItemSlotUI>);
            }

            if (_activeUIs.Count == 0)
            {
                EventManager.OnNext(Message.OnCharacterFree);
            }
        }

        private void OnPressEscape(EventManager.Event e)
        {
            if (_activeUIs.Count > 0)
            {
                _activeUIs.Last().Close();
            }
            else
            {
                Open(UIType.SettingUI);
            }
        }

        private void OnTrySceneLoad(EventManager.Event e)
        {
            CloseAll();
        }
        
        public void CloseMainUI()
        {
            _mainUICanvasGroup.alpha = 0f;
        }
        
        public void OpenMainUI()
        {
            _mainUICanvasGroup.alpha = 1f;
        }
    }
}