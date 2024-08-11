using System;
using DataSystem;
using EventSystem;
using ItemSystem.Produce;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI.EventUI
{
    public class ProducerNodeUI : UIBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _defaultSlot;
        [SerializeField] private Sprite _selectedSlot;
        
        [SerializeField] private Image _iconImage;
        
        [SerializeField] [CanBeNull] private Button _button;

        private Producer _data;
        
        public override UIType GetUIType() => UIType.ComponentUI;

        private void Awake()
        {
            if (_button == null)
            {
                return;
            }

            _button.OnClickAsObservable()
                .Subscribe(_ => OnClickProducer())
                .AddTo(gameObject);
            EventManager.Subscribe(gameObject, Message.OnClickProducerNode, OnClickProducerNode);
        }

        private void OnEnable()
        {
            gameObject.SetActive(_data != null);
        }

        public void SetData(Producer data)
        {
            _data = data;
            
            InitUI();
        }

        private void InitUI()
        {
            if (_data == null)
            {
                return;
            }

            _background.sprite = _defaultSlot;
            _iconImage.sprite = ResourceManager.GetSprite(_data.spritePath) ??
                                ResourceManager.GetSprite(Constants.Path.DefaultIconPath);
        }

        private void UpdateUI(bool isSelected)
        {
            if (_background == null)
            {
                _background = GetComponent<Image>();
            }
            _background.sprite = isSelected ? _selectedSlot : _defaultSlot;
        }

        private void OnClickProducer()
        {
            EventManager.OnNext(Message.OnClickProducerNode, _data.id);
        }
        
        private void OnClickProducerNode(EventManager.Event e)
        {
            var producerId = -1;
            try
            {
                producerId = (int)e.Args[0];
            }
            catch
            {
                Debug.LogError("[ProducerNodeUI] OnClickProducerNode(): Invalid event argument");
            }

            UpdateUI(producerId == _data.id);
        }
    }
}