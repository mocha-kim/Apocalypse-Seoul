using System;
using System.Collections;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGameUI
{
    public class ConditionAlertUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        private bool _isInitialized = false;
        private static readonly WaitForSeconds Wait = new(1f);

        public void SetData(string iconPath, string text)
        {
            Debug.Log(_icon + ", " + _text + ", " + iconPath);
            _icon.sprite = ResourceManager.GetSprite(iconPath);
            _text.text = text;

            _isInitialized = true;
        }
        
        public void Show()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            gameObject.SetActive(true);
            StartCoroutine(WaitAndHide());
        }
        
        private IEnumerator WaitAndHide()
        {
            yield return Wait;
            gameObject.SetActive(false);
        }
    }
}