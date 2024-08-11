using CharacterSystem.Stat;
using DataSystem;
using EnvironmentSystem;
using EnvironmentSystem.Time;
using EventSystem;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class MainUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI dayText;
        
        [SerializeField] private Button moveSceneButton;

        private static PlayerStat Stat => DataManager.Stat;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider spSlider;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;

        [SerializeField] private TextMeshProUGUI debugText;
        
        public override UIType GetUIType() => UIType.MainUI;

        public override void Init()
        {
            base.Init();
            
            this.UpdateAsObservable()
                .Subscribe(_ => UpdateTimer())
                .AddTo(gameObject);
            
            moveSceneButton.onClick.AddListener(MoveScene);
            EventManager.Subscribe(gameObject, Message.OnPlayerAttributeChanged, _ => UpdateSliders());
            EventManager.Subscribe(gameObject, Message.OnReadyMapMove, OnReadyMapMove);
            
            debugText.gameObject.SetActive(false);
            moveSceneButton.gameObject.SetActive(false);
            
            UpdateTimer();
            UpdateSliders();
            
            base.Open();
        }

        private void OnReadyMapMove(EventManager.Event e)
        {
            try
            {
                var value = (bool)e.Args[0];
                moveSceneButton.gameObject.SetActive(value);
            }
            catch
            {
                Debug.LogError("[MainUI] OnReadyMapMove(): Invalid event argument");
            }
        }

        public override void Open()
        {
            // This UI cannot be opened, Do nothing
        }

        public override void Close()
        {
            // This UI cannot be closed, Do nothing
        }

        private void UpdateTimer()
        {
            timeText.text = TimeManager.GetTimeString();
            dayText.text = TimeManager.GetDayString();

            debugText.text = Stat.GetStatInfo();
        }
        
        private void UpdateSliders()
        {
            hpSlider.value = (float)Stat.Attributes[AttributeType.Hp].ModifiedValue / Stat.Attributes[AttributeType.Hp].BaseValue;
            spSlider.value = (float)Stat.Attributes[AttributeType.Sp].ModifiedValue / Stat.Attributes[AttributeType.Sp].BaseValue;
            hungerSlider.value = (float)Stat.Attributes[AttributeType.Hunger].ModifiedValue / Stat.Attributes[AttributeType.Hunger].BaseValue;
            thirstSlider.value = (float)Stat.Attributes[AttributeType.Thirst].ModifiedValue / Stat.Attributes[AttributeType.Thirst].BaseValue;
        }
    
        private void MoveScene()
        {
            if (DataManager.CurrentMap.id == Constants.Scene.Home)
            {
                UIManager.Instance.Open(UIType.MapUI);
            }
            else
            {
                TimeManager.OnMoveHome();
                EventManager.OnNext(Message.OnTrySceneLoad, Constants.Scene.Home);
            }
        }
    }
}
