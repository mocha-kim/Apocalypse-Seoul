using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CharacterSystem.Stat;
using DataSystem;
using DataSystem.SaveLoad;
using EnvironmentSystem;
using EnvironmentSystem.Time;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class SaveLoadUI : UIBase
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private GameObject _saveInfos;
        [SerializeField] private Text[] _uiText;

        public override UIType GetUIType() => UIType.SaveLoadUI;

        private void Awake()
        {
            _saveButton.OnClickAsObservable().Subscribe(_ => OnClickSave()).AddTo(gameObject);
            _loadButton.OnClickAsObservable().Subscribe(_ => OnClickLoad()).AddTo(gameObject);
            _resetButton.OnClickAsObservable().Subscribe(_ => OnClickReset()).AddTo(gameObject);

            _closeButton.onClick.AddListener(Close);
        }

        public override void Open()
        {
            base.Open();
            UpdateUI();
        }

        public override void Close()
        {
            base.Close();
            //GameManager.Instance.ResumeGame();
        }

        private void UpdateUI()
        {
            try
            {
                // TODO: get data from TimeManager, do not direct load from file
                _saveInfos.SetActive(true);

                _uiText[0].text = SaveSystem.LoadData("TimeManager", TimeManager.GetDayString());

                string[] characterPosition = SaveSystem.LoadData("CharacterPosition", TimeManager.GetDayString()).Split();
                _uiText[1].text = SaveSystem.LoadData("CurrentMap", DataManager.CurrentMap).name
                                  + $"({characterPosition[0]}, {characterPosition[1]})";

                _uiText[2].text = DataManager.Stat.GetStatInfo();
            }
            catch (Exception e)
            {
                _saveInfos.SetActive(false);
            }
        }

        private void OnClickSave()
        {
            UpdateUI();
            DataManager.Save();
        }

        private void OnClickLoad()
        {
            DataManager.Load();
            UpdateUI();
        }
        
        private void OnClickReset()
        {
            DataManager.Reset();
            UpdateUI();
        }
    }
}