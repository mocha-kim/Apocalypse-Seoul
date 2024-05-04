using System.Collections;
using DataSystem.Database;
using Event;
using Manager;
using UnityEngine;
using UnityEngine.UI;

// 참고코드 : https://wergia.tistory.com/194
// 위 코드 기반으로 추가수정하였음.

namespace UI
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image imgFill;
        [SerializeField] Text txtProgress;


        #region Singleton.

        private static SceneLoader _instance;

        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<SceneLoader>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                    else
                    {
                        var prefab = ResourceManager.GetPrefab("SceneLoader");
                        _instance = Instantiate(prefab).GetComponent<SceneLoader>();
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
        }

        #endregion

        public void LoadScene(int id)
        {
            SetUIProgress(0);

            gameObject.SetActive(true);
            StartCoroutine(LoadSceneAsync(id));
        }

        private IEnumerator LoadSceneAsync(int id)
        {
            var sceneName = Database.GetMapData(id).scenePath;
            var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone)
            {
                var progress = Mathf.Clamp01(op.progress / 0.9f);
                SetUIProgress(progress);
                yield return null;
            }
            
            gameObject.SetActive(false);
            EventManager.OnNext(Message.OnSceneLoadComplete, id);
        }
        
        private void SetUIProgress(float progress)
        {
            imgFill.fillAmount = progress;
            txtProgress.text = $"{progress * 100:F2}%";
        }
    }
}
