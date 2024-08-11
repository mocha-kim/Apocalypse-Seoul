using Alpha;
using Core;
using DataSystem;
using EnvironmentSystem.Time;
using EventSystem;
using UnityEngine;

namespace EnvironmentSystem.Light
{
    public class SunLight : LightObject
    {
        [SerializeField] private Gradient _lightGradient;
        
        protected override void Awake()
        {
            base.Awake();
            EventManager.Subscribe(gameObject, Message.OnEveryMinute, _ => OnEveryMinute());
        }

        private void OnEveryMinute()
        {
            ThisLight.color = _lightGradient.Evaluate(CurrentPercentOfTheDay());
        }

        private float CurrentPercentOfTheDay() => (float)TimeManager.TotalMinutes() / Constants.Time.MinutesInADay;
    }
}