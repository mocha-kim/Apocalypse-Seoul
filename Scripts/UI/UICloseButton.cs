using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UICloseButton : MonoBehaviour
    {
        [SerializeField] private UIBase _parentUI;

        private void Awake()
        {
            if (_parentUI == null)
            {
                _parentUI = transform.parent.GetComponent<UIBase>();
            }
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _parentUI.Close();
        }
    }
}