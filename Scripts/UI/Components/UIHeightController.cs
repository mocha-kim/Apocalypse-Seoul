using System.Collections.Generic;
using UnityEngine;

namespace UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIHeightController : MonoBehaviour
    {
        private float _width;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeight;
        
        private float _childHeight;
        
        private List<GameObject> _children = new();
        private int _activatedChildCount = 0;

        private RectTransform _rectTransform;

        private void Awake()
        {
            if (_minHeight >= _maxHeight)
            {
                gameObject.SetActive(false);
                return;
            }

            _rectTransform = GetComponent<RectTransform>();
            
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                _children.Add(transform.GetChild(i).gameObject);
            }

            _width = _rectTransform.sizeDelta.x;
            _childHeight = (_maxHeight - _minHeight) / childCount;
            _activatedChildCount = childCount;
        }

        private void OnEnable()
        {
            UpdateChildrenActive();
        }

        public void SetActiveChildCount(int count)
        {
            _activatedChildCount = count;
            if (gameObject.activeSelf)
            {
                UpdateChildrenActive();
            }
        }

        private void UpdateChildrenActive()
        {
            for (var i = 0; i < _activatedChildCount; i++)
            {
                _children[i].SetActive(true);
            }
            for (var i = _activatedChildCount; i < _children.Count; i++)
            {
                _children[i].SetActive(false);
            }

            _rectTransform.sizeDelta = new Vector2(_width, _minHeight + _childHeight * _activatedChildCount);
        }
    }
}