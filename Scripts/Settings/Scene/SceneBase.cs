using AudioSystem;
using DataSystem;
using UnityEngine;

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
            var obj = FindObjectOfType(typeof(UnityEngine.EventSystems.EventSystem));
            if (obj == null)
            {
                Instantiate(ResourceManager.GetPrefab("EventSystem"), Parent);
            }
            else
            {
                obj.name = "@EventSystem";
            }

            obj = FindObjectOfType(typeof(AudioManager));
            if (obj == null)
            {
                Instantiate(ResourceManager.GetPrefab("AudioManager"));
            }
        }

        public abstract void Clear();
    }
}