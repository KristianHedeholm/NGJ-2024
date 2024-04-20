using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rewired player
    private Player _player;

    private Rigidbody2D _rb2d;

    private enum EPlayerState
    {
        Normal,
        Zip, // Teleporting through light
    }

    private EPlayerState _prevState;
    private EPlayerState _state;

    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField]
    private float rideHeight;

    [SerializeField]
    private float rayDistance;

    [SerializeField]
    private float rideSpringStrength;

    [SerializeField]
    private float rideSpringDamp;

    [SerializeField]
    private float _maxHSpeed;

    [SerializeField]
    private float _acceleration;

    private void Start()
    {
        _player = ReInput.players.GetPlayer(0);
        _rb2d = GetComponent<Rigidbody2D>();

        _prevState = _state = EPlayerState.Normal;
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case EPlayerState.Normal:
                _state = NormalUpdate();
                break;
            case EPlayerState.Zip:
                _state = ZipUpdate();
                break;
        }

        if (_state != _prevState)
        {
            // End-of-state calls
            switch (_prevState)
            {
                case EPlayerState.Normal:
                    NormalEnd();
                    break;
                case EPlayerState.Zip:
                    ZipEnd();
                    break;
            }

            // Start-of-state calls
            switch (_state)
            {
                case EPlayerState.Normal:
                    NormalStart();
                    break;
                case EPlayerState.Zip:
                    ZipStart();
                    break;
            }
        }

        _prevState = _state;
    }

    #region Player states

    private void NormalStart() { }

    private EPlayerState NormalUpdate()
    {
        float xMove = _player.GetAxis("Horizontal");

        var speed = _rb2d.velocity;
        speed.x = Mathf.MoveTowards(speed.x, xMove * _maxHSpeed, _acceleration * Time.deltaTime);
        _rb2d.velocity = speed;

        if (_player.GetButtonDown("Horizontal") || _player.GetNegativeButtonDown("Horizontal"))
        {
            _rb2d.AddForce(Vector2.down * 3f, ForceMode2D.Impulse);
        }

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector3.down,
            rayDistance,
            collisionMask
        );

        if (hit.collider != null)
        {
            Vector2 vel = _rb2d.velocity;

            float rayDirVel = Vector2.Dot(Vector2.down, vel);

            float x = hit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamp);

            _rb2d.AddForce(Vector2.down * springForce);
        }

        return EPlayerState.Normal;
    }

    private void NormalEnd() { }

    private void ZipStart() { }

    private EPlayerState ZipUpdate()
    {
        return EPlayerState.Zip;
    }

    private void ZipEnd() { }

    #endregion
}
