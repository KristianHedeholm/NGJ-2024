using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    [SerializeField]
    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public float springStrength;
    public float springDamp;

    private void Update()
    {
        _animator.SetBool("isMoving", _rigidbody2D.velocity.x != 0.0f);
        _spriteRenderer.flipX = _rigidbody2D.velocity.x < 0.0f;
    }
}
