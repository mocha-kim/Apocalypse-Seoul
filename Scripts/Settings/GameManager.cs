using System;
using CharacterSystem.Character.Player;
using DataSystem;
using DataSystem.Database;
using EnvironmentSystem;
using EnvironmentSystem.Camera;
using EnvironmentSystem.Time;
using EventSystem;
using InputSystem.UserActionBind;
using Settings.Scene;
using UI;
using UnityEngine;

namespace Settings
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
        
        public GameObject Player { get; private set; }
        private Vector3 _playerSpawnPosition = Vector3.zero;
        
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
            
            Player = FindObjectOfType<PlayerCharacter>().gameObject;
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
            DataManager.PlayerInventory.AddItem(200009, 2);
            DataManager.PlayerInventory.AddItem(100013, 10);
            
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
        
        public void TeleportPlayer(Vector3 position)
        {
            Player.transform.position = position;
            MainCamera.Instance.transform.position = position;
            
            _playerSpawnPosition = Vector3.zero;
        }
      
        private void SubscribeEvents()
        {
            EventManager.Subscribe(gameObject, Message.OnTrySceneLoad, OnTrySceneLoad);
            EventManager.Subscribe(gameObject, Message.OnSceneLoadComplete, OnSceneLoadComplete);

            EventManager.Subscribe(gameObject, Message.OnPlayerDead, _ => OnPlayerDead());
        }

        private void OnTrySceneLoad(EventManager.Event e)
        {
            int targetSceneId = -1;
            _playerSpawnPosition = Vector3.zero;
            try
            {
                targetSceneId = (int)e.Args[0];
                if (e.Args.Length > 1)
                {
                    _playerSpawnPosition = (Vector3)e.Args[1];
                }
                
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
                Debug.LogError("[GameManager] OnSceneLoadComplete(): " + exception);
                return;
            }
            
            Player = FindObjectOfType<PlayerCharacter>().gameObject;
            TeleportPlayer(_playerSpawnPosition);
            FindObjectOfType<GameScene>().SetMapData();
        }

        private void OnPlayerDead()
        {
            EventManager.OnNext(Message.OnTrySceneLoad, Constants.Scene.Home);
            DataManager.ExecuteDeadPenalty();
        }
    }
}