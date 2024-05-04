using System;
using System.Collections;
using UnityEngine;

namespace Manager
{
    public class AudioManager : MonoBehaviour
    {        
        #region singleton.

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
        }

        #endregion
    }
}