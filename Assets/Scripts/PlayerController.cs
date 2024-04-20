using Rewired;
using Shapes;
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
        Crystal,
    }

    private EPlayerState _prevState;
    private EPlayerState _state;

    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

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

    [SerializeField]
    private LayerMask _bodyLayerMask;

    private Rigidbody2D _currentRigidbody;
    private bool _controlBody;
    private bool _moveHorizontal;
    private bool _moveHorizontalNegative;
    private float _xMove;

    private void Start()
    {
        _player = ReInput.players.GetPlayer(0);
        _rb2d = GetComponent<Rigidbody2D>();
        _currentRigidbody = _rb2d;

        _prevState = _state = EPlayerState.Normal;

        var prefab = LineManager.Instance.GetLine(LineManager.Line.AimLine);
        var gameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        _aimLine = gameObject.GetComponent<Line>();

        ResetAimLine();
    }

    private void Update()
    {
        if (_state != EPlayerState.Normal)
        {
            return;
        }

        if(_player.GetButton("AimMode"))
        {
            _moveHorizontal = false;
            _moveHorizontalNegative = false;
            _xMove = 0.0f;

            float aimX = _player.GetAxis("Horizontal");
            float aimY = _player.GetAxis("Vertical");

            var aimDirection = new Vector2(aimX, aimY);
            var aimNormilized = aimDirection.normalized;
            var playerPosition = new Vector2(
                _currentRigidbody.position.x,
                _currentRigidbody.position.y
            );

            _aimLine.Start = _currentRigidbody.position;
            _aimLine.End = playerPosition + aimNormilized;
        }
        else
        {
            ResetAimLine();

            if (_player.GetButtonDown("MainAction"))
            {
                if (!_controlBody)
                {
                    var collider = Physics2D.OverlapCircle(
                    _currentRigidbody.position,
                    1.05f,
                    _bodyLayerMask.value
                    );

                    if (collider != null && collider.TryGetComponent<Body>(out var body))
                    {
                        _currentRigidbody = body.Rigidbody2D;
                        _controlBody = true;
                        _spriteRenderer.enabled = false;
                        _rb2d.simulated = false;
                    }
                }
                else
                {
                    _controlBody = false;
                    _rb2d.simulated = true;

                    _rb2d.position = _currentRigidbody.position;
                    _currentRigidbody = _rb2d;
                    _spriteRenderer.enabled = true;
                }
            }

            transform.position = _currentRigidbody.position;

            _moveHorizontal = _player.GetButtonDown("Horizontal");
            _moveHorizontalNegative = _player.GetNegativeButtonDown("Horizontal");
            _xMove = _player.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case EPlayerState.Normal:
                _state = NormalFixUpdate();
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

    private EPlayerState NormalFixUpdate()
    {
        var speed = _currentRigidbody.velocity;
        speed.x = Mathf.MoveTowards(speed.x, _xMove * _maxHSpeed, _acceleration * Time.deltaTime);
        _currentRigidbody.velocity = speed;

        if (_moveHorizontal || _moveHorizontalNegative)
        {
            _currentRigidbody.AddForce(Vector2.down * 3f, ForceMode2D.Impulse);
        }

        RaycastHit2D hit = Physics2D.Raycast(
            _currentRigidbody.position,
            Vector3.down,
            rayDistance,
            collisionMask
        );

        if (hit.collider != null)
        {
            Vector2 vel = _currentRigidbody.velocity;

            float rayDirVel = Vector2.Dot(Vector2.down, vel);

            float x = hit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDamp);

            _currentRigidbody.AddForce(Vector2.down * springForce);
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
