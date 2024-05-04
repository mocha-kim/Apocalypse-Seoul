using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EnvironmentSystem.Light
{
    [RequireComponent(typeof(Light2D))]
    public class LightObject : MonoBehaviour
    {
        protected Light2D ThisLight;

        protected virtual void Awake()
        {
            ThisLight = GetComponent<Light2D>();
        }

        protected virtual void On()
        {
            ThisLight.enabled = true;
        }
        
        protected virtual void Off()
        {
            ThisLight.enabled = false;
        }
    }
}