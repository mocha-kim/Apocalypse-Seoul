using System;
using System.Collections.Generic;
using System.Linq;
using DataSystem.Database;
using Event;
using ItemSystem.Inventory;
using ItemSystem.Produce;
using Manager;
using TMPro;
using UI.FloatingUI.Produce;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI.EventUI
{
    public class ProducerUpgradeUI : EventUIBase
    {
        private ProducerType _targetType;
        [SerializeField] private Button _upgradeButton;
        
        private int _nextScriptId;
        
        [SerializeField] private GameObject _producerListParent;
        
        [SerializeField] private GameObject _selectedArea;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        
        private Producer _targetProducer = null;
        private Recipe _targetRecipe;
        
        [SerializeField] private GameObject _materialParent;
        private List<MaterialNodeUI> _materialNodes;
        
        public override UIType GetUIType() => UIType.ProducerUpgradeUI;

        public override void Init()
        {
            base.Init();
            
            _materialNodes = _materialParent.GetComponentsInChildren<MaterialNodeUI>().ToList();
            
            InitData();
            
            _upgradeButton.onClick.AddListener(OnClickUpgrade);
            EventManager.Subscribe(gameObject, Message.OnClickProducerNode, OnClickProductNode);
            EventManager.Subscribe(gameObject, Message.OnProducerUpgraded, OnProducerUpgraded);
            EventManager.Subscribe(gameObject, Message.OnUpdateInventory, OnUpdateInventory);
        }

        public override int Next(int curId)
        {
            Close();
            return _nextScriptId;
        }

        private void InitData()
        {
            var i = 0;
            var childCount = _producerListParent.transform.childCount;
            foreach (ProducerType type in Enum.GetValues(typeof(ProducerType)))
            {
                if (i >= childCount)
                {
                    break;
                }
                var node = _producerListParent.transform.GetChild(i).GetComponent<ProducerNodeUI>();
                node.SetData(DataManager.GetCurrentProducer(type));
                node.gameObject.SetActive(true);
                i++;
            }
        }
        
        private void SetProducer(int producerId)
        {
            _targetRecipe = Database.GetUpgradeRecipe(producerId);
            _targetProducer = Database.GetProducer(producerId);
            UpdateProducerUI();
        }
        
        private void SetMaterials()
        {
            if (_targetRecipe == null)
            {
                return;
            }
            
            for (var i = 0; i < _targetRecipe.materials.Count; i++)
            {
                _materialNodes[i].SetData(_targetRecipe.materials[i]);
            }
            
            UpdateRecipeUI();
        }
        
        public void SetEventInfo(int nextScriptId)
        {
            _nextScriptId = nextScriptId;
        }
        
        private void UpdateProducerUI()
        {
            if (_targetProducer== null)
            {
                _selectedArea.SetActive(false);
                return;
            }

            _nameText.text = _targetProducer.name + " Lv " + _targetProducer.level;
            _descriptionText.text = _targetProducer.description;
            _selectedArea.SetActive(true);
        }
        
        private void UpdateRecipeUI()
        {
            if (_targetRecipe == null)
            {
                _selectedArea.SetActive(false);
                return;
            }
            _upgradeButton.interactable = _targetRecipe.IsProducible();

            foreach (var nodeUI in _materialNodes)
            {
                nodeUI.gameObject.SetActive(false);
            }
            for (var i = 0; i < _targetRecipe.materials.Count; i++)
            {
                _materialNodes[i].gameObject.SetActive(true);
                _materialNodes[i].UpdateUI();
            }
        }
        
        private void OnUpdateInventory(EventManager.Event e)
        {
            try
            {
                if ((InventoryType)e.Args[0] != InventoryType.Storage)
                {
                    return;
                }
            }
            catch
            {
                Debug.LogError("[ProducerUpgradeUI] OnUpdateInventory(): Invalid event argument");
                return;
            }

            UpdateRecipeUI();
        }

        private void OnClickProductNode(EventManager.Event e)
        {
            var producerId = -1;
            try
            {
                producerId = (int)e.Args[0];
            }
            catch
            {
                Debug.LogError("[ProducerUpgradeUI] OnClickProductNode(): Invalid event argument");
            }

            SetProducer(producerId);
            SetMaterials();
        }

        private void OnClickUpgrade()
        {
            EventManager.OnNext(Message.OnTryProducerUpgrade, _targetType);
        }
        
        private void OnProducerUpgraded(EventManager.Event obj)
        {
            UpdateRecipeUI();
            UpdateProducerUI();
        }
    }
}