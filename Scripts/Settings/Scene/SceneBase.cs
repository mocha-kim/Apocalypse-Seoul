using DataSystem;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Settings.Scene
{
    public abstract class SceneBase : MonoBehaviour
    {
        protected Transform Parent;

        [SerializeField] public string Name = Constants.UndefinedString;
        public abstract SceneType Type { get; protected set; }

        private void Awake()
        {
            Parent = transform.parent;
            
            Init();
        }

        protected virtual void Init()
        {
            var obj = FindObjectOfType(typeof(EventSystem));
            if (obj == null)
            {
                Instantiate(ResourceManager.GetPrefab("EventSystem"), Parent);
            }
            else
            {
                obj.name = "@EventSystem";
            }
        }

        public abstract void Clear();
    }
}