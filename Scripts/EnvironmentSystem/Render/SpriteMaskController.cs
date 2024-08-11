using System;
using System.Collections.Generic;
using DataSystem;
using EventSystem;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EnvironmentSystem.Render
{
    [RequireComponent(typeof(Collider2D))]
    public class SpriteMaskController : MonoBehaviour
    {
        [SerializeField] private string targetLayerString = Constants.Layer.WallString;
        private int _targetLayerDecimal;
        private int _targetLayerBinary;

        [SerializeField] private float detectRadius = 1f;
        
        private readonly TimeSpan _time = TimeSpan.FromMilliseconds(100f);

        private bool _isSwaped = false;
        private readonly List<RaycastHit2D> _hitsUnique = new();
        private readonly List<RaycastHit2D> _hitsFront = new();

        private void Awake()
        {
            _targetLayerDecimal = LayerMask.NameToLayer(targetLayerString);
            _targetLayerBinary = LayerMask.GetMask(targetLayerString);
            if (_targetLayerDecimal == -1)
            {
                Debug.LogWarning("[SpriteMaskController] Awake(): Invalid mask target layer string");
                _targetLayerDecimal = 0;
            }

            GetComponent<Collider2D>().isTrigger = true;

            EventManager.Subscribe(gameObject, Message.OnAreaEnter, _ => SwapDetectedWallsMask());
            EventManager.Subscribe(gameObject, Message.OnAreaExit, _ => SwapDetectedWallsMask());
        }

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .ThrottleFirst(_time)
                .Subscribe(OnTriggerEnter2DHandler)
                .AddTo(gameObject);
            
            this.OnTriggerExit2DAsObservable()
                .ThrottleFirst(_time)
                .Subscribe(OnTriggerExit2DHandler)
                .AddTo(gameObject);
        }

        private void OnTriggerEnter2DHandler(Collider2D other)
        {
            if (other.gameObject.layer != _targetLayerDecimal)
            {
                return;
            }

            UpdateFrontWallsMask();
        }

        private void OnTriggerExit2DHandler(Collider2D other)
        {
            if (other.gameObject.layer != _targetLayerDecimal)
            {
                return;
            }

            ResetRendererMask(other);
        }

        private void UpdateFrontWallsMask()
        {
            _isSwaped = false;
            
            var position = (Vector2)transform.position;
            var uniqueColliders = new HashSet<Collider2D>();

            var horizontalPosition = position + Vector2.up * detectRadius;
            var hits = Physics2D.RaycastAll(horizontalPosition, Vector2.down, 2 * detectRadius, _targetLayerBinary);
            foreach (var hit in hits)
            {
                _hitsUnique.Add(hit);
            }
            
            var verticalPosition = position + Vector2.right * detectRadius;
            hits = Physics2D.RaycastAll(verticalPosition, Vector2.left, 2 * detectRadius, _targetLayerBinary);
            foreach (var hit in hits)
            {
                if (uniqueColliders.Add(hit.collider))
                {
                    _hitsUnique.Add(hit);
                }
            }

            _hitsFront.Clear();
            foreach (var hit in _hitsUnique)
            {
                if (hit.point.y <= position.y)
                {
                    hit.collider.GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    _hitsFront.Add(hit);
                }
            }
            
            _hitsUnique.Clear();
        }

        private void SwapDetectedWallsMask()
        {
            foreach (var hit in _hitsFront)
            {
                hit.collider.GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }

            UpdateFrontWallsMask();
        }

        private void ResetRendererMask(Collider2D other)
        {
            if (other == null)
            {
                return;
            }

            other.GetComponent<TilemapRenderer>().maskInteraction = SpriteMaskInteraction.None;
        }
    }
}
