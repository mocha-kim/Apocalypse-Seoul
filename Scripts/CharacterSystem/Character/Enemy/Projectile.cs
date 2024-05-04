using System;
using System.Collections;
using System.Collections.Generic;
using Core.Interface;
using DataSystem;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<Projectile> _managedPool;

    private Rigidbody2D _rigidbody2D;
    private Vector2 _direction;

    private int _damage;
    private int _speed;
    private Transform _attackTrans;
    
    private LayerMask _targetMask;
    private LayerMask _obstacleMask;

    private bool _isInitialized = false;
    private bool _isReleased = false;
    private float _destroyTime;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _obstacleMask = Constants.Layer.Obstacle;
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            gameObject.SetActive(false);
        }
        _isReleased = false;
    }

    private void Update()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + _direction * (Time.fixedDeltaTime * _speed));
        _destroyTime -= Time.fixedDeltaTime;

        if (_destroyTime <= 0)
        {
            Release();
        }
    }

    public void ShootProjectile(Vector2 direction, int damage, int speed, Transform attackTrans)
    {
        this._direction = direction;
        this._damage = damage;
        this._speed = speed;
        this._attackTrans = attackTrans;

        _destroyTime = Constants.Character.ProjectileValidTime;
    }

    public void Init(IObjectPool<Projectile> pool, string[] targetMaskStrings)
    {
        _managedPool = pool;
        _targetMask = LayerMask.GetMask(targetMaskStrings);

        _isInitialized = true;
    }

    private void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;
        _managedPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == _obstacleMask)
        {
            Release();
            return;
        }

        if (((1 << col.gameObject.layer) & _targetMask) > 0)
        {
            IDamageable damageable = col.transform.parent.GetComponent<IDamageable>();
            
            damageable?.Damage(_damage);
            damageable?.SetTarget(_attackTrans);
            Release();
        }
    }
}