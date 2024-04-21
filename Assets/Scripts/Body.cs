using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public bool IsDead { get; private set; }

    [SerializeField]
    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public float springStrength;
    public float springDamp;
    private bool _isFliped;

    private void Update()
    {
        _animator.SetBool("isMoving", _rigidbody2D.velocity.x != 0.0f);
        UpdateFlip();

    }

    public void SetIsAiming(bool isAiming)
    {
        _animator.SetBool("isAiming", isAiming);
    }

    public void TriggerDead()
    {
        _animator.SetTrigger("Die");
        IsDead = true;
    }

    public void TriggerAwake()
    {
        _animator.SetTrigger("Awake");
    }

    private void UpdateFlip()
    {
        if(_isFliped)
        {
            if(_rigidbody2D.velocity.x > 0.0f)
            {
                _isFliped = false;
            }
        }
        else
        {
            if (_rigidbody2D.velocity.x < 0.0f)
            {
                _isFliped = true;
            }
        }
        _spriteRenderer.flipX = _isFliped;
    }
}
