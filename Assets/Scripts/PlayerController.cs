using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using Shapes;

public class PlayerController : MonoBehaviour
{
    // Rewired player
    private Player _player;

    private Rigidbody2D _rb2d;

    private enum EPlayerState
    {
        Normal,
        Zip, // Teleporting through light
        Crystal,
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

    private Line _aimLine;

    [SerializeField]
    private Body _body = null;

    private void Start()
    {
        _player = ReInput.players.GetPlayer(0);
        _rb2d = GetComponent<Rigidbody2D>();

        _prevState = _state = EPlayerState.Normal;

        var prefab = LineManager.Instance.GetLine(LineManager.Line.AimLine);
        var gameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        _aimLine = gameObject.GetComponent<Line>();

        ResetAimLine();
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

            case EPlayerState.Crystal:
                _state = CrystalUpdate();
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
       
        var rb2d = _rb2d;
        if(_body != null)
        {
            rb2d = _body.Rigidbody2D;
        }
        
        ResetAimLine();

        float xMove = _player.GetAxis("Horizontal");

        var speed = rb2d.velocity;
        speed.x = Mathf.MoveTowards(speed.x, xMove * _maxHSpeed, _acceleration * Time.deltaTime);
        rb2d.velocity = speed;

        if (_player.GetButtonDown("Horizontal") || _player.GetNegativeButtonDown("Horizontal"))
        {
            rb2d.AddForce(Vector2.down * 3f, ForceMode2D.Impulse);
        }

        RaycastHit2D hit = Physics2D.Raycast(
            rb2d.position,
            Vector3.down,
            rayDistance,
            collisionMask
        );

        if (hit.collider != null)
        {
            Vector2 vel = rb2d.velocity;

            float rayDirVel = Vector2.Dot(Vector2.down, vel);

            float x = hit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamp);

            rb2d.AddForce(Vector2.down * springForce);
        }

        if(!Mathf.Approximately(rb2d.velocity.x, 0))
        {
            return EPlayerState.Normal;
        }

        float aimX = _player.GetAxis("AimHorizontal");
        float aimY = _player.GetAxis("AimVertical");

        var aimDirection = new Vector2(aimX, aimY);
        var aimNormilized = aimDirection.normalized;
        var playerPosition = new Vector2(rb2d.position.x, rb2d.position.y);

        _aimLine.Start = rb2d.position;
        _aimLine.End = playerPosition + aimNormilized;


        return EPlayerState.Normal;
    }

    private void NormalEnd() { }

    private void ZipStart() { }

    private EPlayerState ZipUpdate()
    {
        return EPlayerState.Zip;
    }

    private void ZipEnd() { }

    private void CrystalStart() { }

    private EPlayerState CrystalUpdate()
    {
        
        
        return EPlayerState.Crystal;
    }

    private void CrystalEnd() { }

    private void ResetAimLine()
    {
        _aimLine.Start = Vector3.zero;
        _aimLine.End = Vector3.zero;
    }

    #endregion
}
