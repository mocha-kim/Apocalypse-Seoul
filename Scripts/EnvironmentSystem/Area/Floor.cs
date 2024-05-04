using System;
using UnityEngine;

namespace EnvironmentSystem.Area
{
    public class Floor : Area
    {
        [SerializeField] private Floor _upstairs;
        [SerializeField] private Floor _downstairs;

        [SerializeField] private Stairs[] _stairsToUp;
        [SerializeField] private Stairs[] _stairsToDown;

        private void OnEnable()
        {   
            foreach (var stairs in _stairsToUp)
            {
                stairs.OnFadeNextFloor += GoUpstairs;
            }
            foreach (var stairs in _stairsToDown)
            {
                stairs.OnFadeNextFloor += GoDownstairs;
            }
        }

        private void OnDisable()
        {
            foreach (var stairs in _stairsToUp)
            {
                stairs.OnFadeNextFloor -= GoUpstairs;
            }
            foreach (var stairs in _stairsToDown)
            {
                stairs.OnFadeNextFloor -= GoDownstairs;
            }
        }

        private void GoUpstairs()
        {
            if (_upstairs == null)
            {
                return;
            }
            gameObject.SetActive(false);
            _upstairs.gameObject.SetActive(true);
        }
        
        private void GoDownstairs()
        {
            if (_downstairs == null)
            {
                return;
            }
            gameObject.SetActive(false);
            _downstairs.gameObject.SetActive(true);
        }
    }
}