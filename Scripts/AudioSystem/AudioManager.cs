using Core;
using EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public SerializableDictionary<string, SFXType> _sfxType;
        
        public SerializableDictionary<SFXType, AK.Wwise.Event> _themeSong;
        private AK.Wwise.Event themeEvent;
        
        public SerializableDictionary<SFXType, AK.Wwise.Event> _ambience;
        private AK.Wwise.Event ambienceEvent;
        
        public SerializableDictionary<SFXType, AK.Wwise.Event> _sfx;
        
        public SerializableDictionary<SFXParameter, AK.Wwise.RTPC> _wwiseParamter;


        #region singleton

        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<AudioManager>();
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

        private void OnAwake()
        {
            EventManager.Subscribe(gameObject, Message.OnSceneLoadComplete, SceneLoad);
        }

        private void Start()
        {
            Scene scene = SceneManager.GetActiveScene();
            
            if (_sfxType.TryGetValue(scene.name, out SFXType sfxType))
            {
                if (_themeSong.TryGetValue(sfxType, out AK.Wwise.Event theme))
                {
                    themeEvent = theme;
                    themeEvent.Post(gameObject);
                }

                if (_ambience.TryGetValue(sfxType, out AK.Wwise.Event ambience))
                {
                    ambienceEvent = ambience;
                    ambienceEvent.Post(gameObject);
                }
            }
            else if(themeEvent != null)
            {
                themeEvent.ExecuteAction(gameObject, AkActionOnEventType.AkActionOnEventType_Stop, 500, AkCurveInterpolation.AkCurveInterpolation_Log1);
                ambienceEvent.ExecuteAction(gameObject, AkActionOnEventType.AkActionOnEventType_Stop, 500, AkCurveInterpolation.AkCurveInterpolation_Log1);
            }
        }

        private void SceneLoad(EventManager.Event e)
        {
            Start();
        }

        public void PlaySFX(SFXType sfxType, GameObject gObject)
        {
            if (_sfx.TryGetValue(sfxType, out AK.Wwise.Event outValue))
            {
                outValue.Post(gObject);
            }
        }

        public void SetWwiseParameter(SFXParameter sfxParameter, int parameter)
        {
            AkSoundEngine.SetRTPCValue(_wwiseParamter[sfxParameter].ToString(), parameter);
        }
    }
}