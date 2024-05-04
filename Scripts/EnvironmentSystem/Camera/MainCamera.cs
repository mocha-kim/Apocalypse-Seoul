using UnityEngine;

namespace EnvironmentSystem.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MainCamera : MonoBehaviour
    {
        #region singleton
        
        private static MainCamera _instance;
        
        public static MainCamera Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                
                var obj = FindObjectOfType<MainCamera>();
                if (obj != null)
                {
                    _instance = obj;
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (Instance == this)
            {
                _camera = GetComponent<UnityEngine.Camera>();
                return;
            }
            Destroy(gameObject);
        }
        
        #endregion

        private static UnityEngine.Camera _camera;

        private void OnEnable()
        {
            var player = GameObject.FindWithTag("Player");
            GetComponent<FollowCharacter>().InitTarget(player.transform);
        }

        public static Vector3 GetMouseWorldPosition() => _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}