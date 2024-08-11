using System;
using DataSystem;
using DataSystem.Database;
using EventSystem;
using Settings.Scene;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class MapUI : UIBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _moveButton;
        [SerializeField] private GameObject mapBoard;
        [SerializeField] private TextMeshProUGUI mapName;
        [SerializeField] private TextMeshProUGUI mapDesc;

        private MapNodeUI[] _mapNodes;
        
        private int _selectedSceneId = 0;
        private MapData _selectedMap = null;

        public override UIType GetUIType() => UIType.MapUI;

        private void Awake()
        {
            _mapNodes = GetComponentsInChildren<MapNodeUI>();
            
            EventManager.Subscribe(gameObject, Message.OnMapSelect, OnSelect);
            _closeButton.OnClickAsObservable().Subscribe(_ => Close()).AddTo(gameObject);
            _moveButton.OnClickAsObservable().Subscribe(_ => OnClickMove()).AddTo(gameObject);
        }

        public override void Open()
        {
            base.Open();
            foreach (var node in _mapNodes)
            {
                node.Init();
            }

            EventManager.OnNext(Message.OnMapSelect, DataManager.CurrentMap.id);
        }

        private void OnSelect(EventManager.Event e)
        {
            try
            {
                var id = (int)e.Args[0];
                _selectedSceneId = id;
            }
            catch
            {
                Debug.LogError("[MapUI] OnSelect(): Invalid event argument");
                return;
            }

            if (DataManager.CurrentMap != null && _selectedSceneId == DataManager.CurrentMap.id)
            {
                mapBoard.SetActive(false);
                _moveButton.interactable = false;
                _selectedMap = null;
            }
            else
            {
                mapBoard.SetActive(true);
                _moveButton.interactable = true;
                _selectedMap = Database.GetMapData(_selectedSceneId);
                mapName.text = _selectedMap?.name ?? Constants.UndefinedString;
                mapDesc.text = _selectedMap?.description ?? Constants.UndefinedString;
            }
        }

        private void OnClickMove()
        {
            if (_selectedMap == null)
            {
                return;
            }
            
            EventManager.OnNext(Message.OnTrySceneLoad, _selectedMap.id);
            Close();
        }
    }
}