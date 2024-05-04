using System;
using System.Collections.Generic;
using System.Linq;
using DataSystem.Database;
using Event;
using ItemSystem.Inventory;
using ItemSystem.Produce;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FloatingUI.Produce
{
    public abstract class ProducerUI : MouseTriggerUI
    {
        private Vector2 _productNodeSize;
        private Vector2 _contentAreaSize;
        private int _productNodeCount = 0;
        private GameObject _productNodePrefab;
        [SerializeField] private Vector2 _space;
        [SerializeField] private int _colCount;
        [SerializeField] private GameObject _productListParent;

        private List<ItemRecipe> _recipeDataList;

        [SerializeField] private GameObject _selectedArea;
        [SerializeField] private Button _produceButton;

        private ProductNodeUI _targetProduct;
        private ItemRecipe _targetItemRecipe;

        [SerializeField] private GameObject _materialParent;
        private List<MaterialNodeUI> _materialNodes;

        protected abstract ProducerType GetProducerType();
        public abstract override UIType GetUIType();
        protected override bool HasEnterExitTrigger => false;

        public override void Init()
        {
            base.Init();

            _productNodePrefab = ResourceManager.GetPrefab("ProductNodeUI");
            _productNodeSize = _productNodePrefab.GetComponent<RectTransform>().sizeDelta;
            _contentAreaSize = _productListParent.GetComponent<RectTransform>().sizeDelta;
            
            _targetProduct = _selectedArea.GetComponentInChildren<ProductNodeUI>();
            _materialNodes = _materialParent.GetComponentsInChildren<MaterialNodeUI>().ToList();
            
            InitData();
            
            _produceButton.onClick.AddListener(OnClickProduce);
            EventManager.Subscribe(gameObject, Message.OnProducerUpgraded, OnProducerUpgraded);
            EventManager.Subscribe(gameObject, Message.OnClickProductNode, OnClickProductNode);
            EventManager.Subscribe(gameObject, Message.OnUpdateInventory, OnUpdateInventory);
        }

        public override void Open()
        {
            base.Open();
            _selectedArea.SetActive(false);
        }

        private void InitData()
        {
            _recipeDataList = Database.GetItemRecipeList(GetProducerType());
            foreach (var recipe in _recipeDataList)
            {
                if (recipe.producerLevel > DataManager.Stat.ProducerLevel[GetProducerType()])
                {
                    continue;
                }
                
                var newNode = Instantiate(_productNodePrefab, _productListParent.transform);
                newNode.SetActive(true);
                newNode.GetComponent<RectTransform>().anchoredPosition = GetNextNodePosition();
                
                var nodeUI = newNode.GetComponent<ProductNodeUI>();
                nodeUI.SetData(recipe);
                nodeUI.SetContext(this);
                
                _productNodeCount++;
            }

            _productListParent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(_contentAreaSize.x, _productNodeCount / _colCount * (_productNodeSize.y + _space.y));
        }

        private void UpdateLevelData()
        {
            var level = DataManager.Stat.ProducerLevel[GetProducerType()];
            foreach (var recipe in _recipeDataList)
            {
                if (recipe.producerLevel != level)
                {
                    continue;
                }
                
                var newNode = Instantiate(_productNodePrefab, _productListParent.transform);
                newNode.SetActive(true);
                newNode.GetComponent<ProductNodeUI>().SetData(recipe);
                newNode.GetComponent<RectTransform>().anchoredPosition = GetNextNodePosition();
                _productNodeCount++;
            }
            
            _productListParent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(_contentAreaSize.x, _productNodeCount / _colCount * (_productNodeSize.y + _space.y));
        }

        private Vector2 GetNextNodePosition()
        {
            var x = (_space.x + _productNodeSize.x) * (_productNodeCount % _colCount);
            var y = -(_space.y + _productNodeSize.y) * (_productNodeCount / _colCount);
            return new Vector2(x, y);
        }
        
        private void SetRecipe(int recipeId)
        {
            _targetItemRecipe = Database.GetItemRecipe(ProducerType.CraftingTable, recipeId);
            _targetProduct.SetData(_targetItemRecipe);
            UpdateProductUI();
        }

        private void SetMaterials()
        {
            if (_targetItemRecipe == null)
            {
                return;
            }
            
            for (var i = 0; i < _targetItemRecipe.materials.Count; i++)
            {
                _materialNodes[i].SetData(_targetItemRecipe.materials[i]);
            }
            
            UpdateRecipeUI();
        }

        private void UpdateProductUI()
        {
            if (_targetProduct == null)
            {
                _selectedArea.SetActive(false);
                return;
            }
            
            _selectedArea.SetActive(true);
        }

        private void UpdateRecipeUI()
        {
            if (_targetItemRecipe == null)
            {
                _selectedArea.SetActive(false);
                return;
            }
            _produceButton.interactable = _targetItemRecipe.IsProducible();

            foreach (var nodeUI in _materialNodes)
            {
                nodeUI.gameObject.SetActive(false);
            }
            for (var i = 0; i < _targetItemRecipe.materials.Count; i++)
            {
                _materialNodes[i].gameObject.SetActive(true);
                _materialNodes[i].UpdateUI();
            }
        }

        private void OnProducerUpgraded(EventManager.Event e)
        {
            try
            {
                if (GetProducerType() != (ProducerType)e.Args[0])
                {
                    return;
                }
            }
            catch
            {
                Debug.LogError("[ProducerUI] OnProducerUpgraded(): Invalid event argument");
                return;
            }
            UpdateLevelData();
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
                Debug.LogError("[ProducerUI] OnUpdateInventory(): Invalid event argument");
                return;
            }

            UpdateRecipeUI();
        }

        private void OnClickProductNode(EventManager.Event e)
        {
            var recipeId = -1;
            try
            {
                recipeId = (int)e.Args[0];
            }
            catch
            {
                Debug.LogError("[ProducerUI] OnClickProductNode(): Invalid event argument");
            }

            SetRecipe(recipeId);
            SetMaterials();
        }

        private void OnClickProduce()
        {
            EventManager.OnNext(Message.OnClickProduce, _targetItemRecipe.id);
            UpdateRecipeUI();
        }
    }
}