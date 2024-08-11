using DataSystem;
using DataSystem.Database;
using DG.Tweening;
using EventSystem;
using Settings.Scene;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class MapNodeUI : MonoBehaviour
    {
        [SerializeField] private int _sceneId;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _nameText;
        
        private MapData _data;
        private Tweener _tween;

        private bool _isInitialized = false;
        
        private void OnDestroy()
        {
            _tween?.Kill();
        }

        public void Init()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;
            
            if (_sceneId == -1)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _data = Database.GetMapData(_sceneId);
            if (_data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _nameText.text = _data.name;
            _nameText.color = Color.gray;

            if (_data.id == DataManager.CurrentMap.id)
            {
                _nameText.color = Color.gray;
                return;
            }
            EventManager.Subscribe(gameObject, Message.OnMapSelect, OnSelect);
            _selectButton.OnClickAsObservable().Subscribe(_ => OnClickButton()).AddTo(gameObject);
        }

        private void OnSelect(EventManager.Event e)
        {
            var id = (int)e.Args[0];
            if (id != _data.id)
            {
                _tween?.Kill();
                _tween = _nameText.DOColor(Color.black, 0.3f);
            }
            else
            {
                _tween?.Kill();
                _tween = _nameText.DOColor(Color.white, 0.3f);
            }
        }

        private void OnClickButton()
        {
            EventManager.OnNext(Message.OnMapSelect, _data.id);
        }
    }
}