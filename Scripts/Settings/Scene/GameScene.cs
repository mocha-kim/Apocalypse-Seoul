using EnvironmentSystem.Camera;
using Manager;
using UI;
using UI.FixedUI;
using UnityEngine;
using EventManager = Manager.EventManager;

namespace Settings.Scene
{
    public abstract class GameScene : SceneBase
    {
        private MapData _mapData;
        
        protected override void Init()
        {
            base.Init();
            
            var obj = FindObjectOfType(typeof(GameManager));
            if (obj == null)
            {
                var newObject = Instantiate(ResourceManager.GetPrefab("EmptyObject"));
                newObject.AddComponent<GameManager>();
                newObject.name = "GameManager";
            }

            obj = FindObjectOfType(typeof(InputManager));
            if (obj == null)
            {
                var newObject = Instantiate(ResourceManager.GetPrefab("EmptyObject"));
                newObject.AddComponent<InputManager>();
                newObject.name = "InputManager";
            }

            var mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera.GetComponent<MainCamera>() != null)
            {
                return;
            }
            Destroy(mainCamera);
            mainCamera = Instantiate(ResourceManager.GetPrefab("MainCamera"));
            mainCamera.transform.SetAsFirstSibling();
        }
        
        public void SetMapData()
        {
            _mapData = DataManager.CurrentMap;
            
            var ui = UIManager.Instance.Open(UIType.ToastUI) as ToastUI;
            ui.Init(_mapData.name);
            
            TimeManager.Play();
        }
    }
}