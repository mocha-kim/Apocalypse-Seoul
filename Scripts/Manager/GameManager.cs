using System;
using DataSystem;
using DataSystem.Database;
using Event;
using Settings.Scene;
using UI;
using UI.FixedUI;
using UnityEngine;
using UserActionBind;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        #region singleton
        
        private static GameManager _instance;
        
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GameManager>();
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

        #region MonoBehaviour
        
        private void OnAwake()
        {
            ResourceManager.Init();
            InputBinding.Init();
            
            DataManager.Init();
            Database.Init();
            
            TimeManager.Init();
            SubscribeEvents();

            var scene = FindObjectOfType<GameScene>();
            if (scene != null)
            {
                DataManager.CurrentMap = Database.GetMapData(scene.Name);
            }
        }
        
        private void Start()
        {
            // debug code
            DataManager.Storage.AddItem(100001, 10);
            DataManager.Storage.AddItem(100002, 10);
            DataManager.Storage.AddItem(100003, 10);
            DataManager.Storage.AddItem(100004, 10);
            DataManager.Storage.AddItem(100005, 10);
            DataManager.Storage.AddItem(100006, 10);
            DataManager.Storage.AddItem(200001, 2);
            DataManager.PlayerInventory.AddItem(100003, 4);
            DataManager.PlayerInventory.AddItem(100024, 1);
            DataManager.PlayerInventory.AddItem(200001, 3);
            DataManager.PlayerInventory.AddItem(200008, 2);
            
            EventManager.OnNext(Message.OnReadyGameManager);
        }

        private void Update()
        {
            TimeManager.OnUpdate();
        }

        #endregion

        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }
      
        private void SubscribeEvents()
        {
            EventManager.Subscribe(gameObject, Message.OnTrySceneLoad, OnTrySceneLoad);
            EventManager.Subscribe(gameObject, Message.OnSceneLoadComplete, OnSceneLoadComplete);
        }

        private void OnTrySceneLoad(EventManager.Event e)
        {
            try
            {
                var targetSceneId = (int)e.Args[0];
                SceneLoader.Instance.LoadScene(targetSceneId);
            }
            catch
            {
                Debug.Log("[GameManager] OnTrySceneLoad(): Invalid event argument");
            }
        }

        private void OnSceneLoadComplete(EventManager.Event e)
        {
            try
            {
                var id = (int)e.Args[0];
                DataManager.CurrentMap = Database.GetMapData(id);
                if (DataManager.CurrentMap == null)
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("[GameScene] OnSceneLoadComplete(): " + exception.Message);
                return;
            }
            
            FindObjectOfType<GameScene>().SetMapData();
        }
    }
}