using System.Collections;
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
        Possess,
        Zip, // Teleporting through light
        Crystal,
    }

    private EPlayerState _prevState;

    [SerializeField, ReadOnly]
    private EPlayerState _state;

    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField]
    private GameObject _sprite;

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
    private LayerMask _bodyLayerMask;

    [SerializeField]
    private LayerMask _shootingLayerMask;

    [SerializeField]
    private float zipDuration;

    private Rigidbody2D _currentRigidbody;
    private bool _moveHorizontal;
    private bool _moveHorizontalNegative;
    private float _xMove;

    private Crystal _targetCrystal = null;
    private Body _targetBody = null;
    public Body CurrentBody => _targetBody;

    private Body _prevBody = null;

    private Transform _zipTarget = null;

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
        UpdateState();
    }

    private void FixedUpdate()
    {
        if (_state == EPlayerState.Normal || _state == EPlayerState.Possess)
        {
            var speed = _currentRigidbody.velocity;
            speed.x = Mathf.MoveTowards(
                speed.x,
                _xMove * _maxHSpeed,
                _acceleration * Time.deltaTime
            );
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

                var strength = rideSpringStrength;
                var damp = rideSpringDamp;

                if (_targetBody != null)
                {
                    strength = _targetBody.springStrength;
                    damp = _targetBody.springDamp;
                }

                float springForce = (x * strength) - (rayDirVel * damp);

                _currentRigidbody.AddForce(Vector2.down * springForce);
            }
        }
    }

    private void EnterBody(Body body)
    {
        _currentRigidbody = body.Rigidbody2D;
        _sprite.SetActive(false);
        _rb2d.simulated = false;
    }

    private void EnterCrystal(Crystal crystal)
    {
        ResetAimLine();
        _targetCrystal = crystal;
        _rb2d.simulated = false;
        _currentRigidbody = _rb2d;
        _sprite.SetActive(false);
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case EPlayerState.Normal:
                _state = NormalUpdate();
                break;
            case EPlayerState.Possess:
                _state = PossessUpdate();
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
                case EPlayerState.Possess:
                    PossessEnd();
                    break;
                case EPlayerState.Zip:
                    ZipEnd();
                    break;
                case EPlayerState.Crystal:
                    CrystalEnd();
                    break;
            }

            // Start-of-state calls
            switch (_state)
            {
                case EPlayerState.Normal:
                    NormalStart();
                    break;
                case EPlayerState.Possess:
                    PossessStart();
                    break;
                case EPlayerState.Zip:
                    ZipStart();
                    break;
                case EPlayerState.Crystal:
                    CrystalStart();
                    break;
            }
        }

        _prevState = _state;
    }

    private EPlayerState HandleAiming()
    {
        _moveHorizontal = false;
        _moveHorizontalNegative = false;
        _xMove = 0.0f;

        if (_targetBody != null)
        {
            _targetBody.SetIsAiming(true);
        }

        if (_player.GetAxis2D("Horizontal", "Vertical").magnitude < 0.5f)
        {
            ResetAimLine();
            return _state;
        }

        var aimAngle = _player.GetAxis2D("Horizontal", "Vertical").ToAngleDeg();
        aimAngle = Mathf.Round(aimAngle / 45.0f) * 45.0f;
        var aimNormilized = aimAngle.DegAngleToDir(1);
        var playerPosition = new Vector2(
            _currentRigidbody.position.x,
            _currentRigidbody.position.y
        );

        var endOfLinePoint = playerPosition + aimNormilized;

        _aimLine.Start = _currentRigidbody.position;
        _aimLine.End = endOfLinePoint;

        if (_player.GetButtonDown("MainAction"))
        {
            var direction = endOfLinePoint - playerPosition;
            var hit = Physics2D.Raycast(endOfLinePoint, direction, 5000.0f, _shootingLayerMask);

            if (
                hit.collider != null
                && hit.collider.gameObject.layer == LayerMask.NameToLayer("Body")
            )
            {
                if (hit.collider.TryGetComponent<Body>(out var body) && body != _targetBody)
                {
                    _prevBody = _targetBody;
                    _targetCrystal = null;
                    _targetBody = body;
                    _zipTarget = _targetBody.transform;
                    return EPlayerState.Zip;
                }
            }

            if (
                hit.collider != null
                && hit.collider.gameObject.layer == LayerMask.NameToLayer("Crystal")
            )
            {
                if (
                    hit.collider.TryGetComponent<Crystal>(out var crystal)
                    && crystal != _targetCrystal
                )
                {
                    _prevBody = _targetBody;
                    _targetBody = null;
                    _targetCrystal = crystal;
                    _zipTarget = crystal.transform;
                    return EPlayerState.Zip;
                }
            }

            var hitOverlap = Physics2D.OverlapCircle(playerPosition, 1.0f, _bodyLayerMask);
            if (
                hitOverlap != null
                && hitOverlap.TryGetComponent<Body>(out var bodyNew)
                && bodyNew != _targetBody
            )
            {
                _targetCrystal = null;
                _prevBody = _targetBody;
                _targetBody = bodyNew;
                if(_targetBody.IsDead)
                {
                    _targetBody.TriggerAwake();
                }
                _zipTarget = _targetBody.transform;
                return EPlayerState.Zip;
            }
        }

        return _state;
    }

    #region Player states

    private void NormalStart() { }

    private EPlayerState NormalUpdate()
    {
        return HandleAiming();
    }

    private void NormalEnd() { }

    private void ZipStart()
    {
        StartCoroutine(ZipRoutine());
    }

    private IEnumerator ZipRoutine()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = transform.position;

        Vector2 centerPosition = (startPos + endPos) / 2 + Vector2.up * 0.75f;

        if (_zipTarget != null)
        {
            endPos = _zipTarget.position;
            centerPosition = (startPos + endPos) / 2;
        }

        float dist = Vector2.Distance(startPos, endPos);

        ResetAimLine();
        _rb2d.simulated = false;
        _sprite.SetActive(true);

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / (dist * zipDuration);

            transform.position = Vector2.Lerp(startPos, endPos, Easing.Smooth2.In(alpha));
            yield return null;
        }

        if (_targetBody != null)
        {
            EnterBody(_targetBody);
            _state = EPlayerState.Possess;
            yield break;
        }

        if (_targetCrystal != null)
        {
            EnterCrystal(_targetCrystal);
            _state = EPlayerState.Crystal;
            yield break;
        }

        _rb2d.simulated = true;

        _currentRigidbody = _rb2d;
        _sprite.SetActive(true);
        _state = EPlayerState.Normal;
    }

    private EPlayerState ZipUpdate()
    {
        return EPlayerState.Zip;
    }

    private void ZipEnd() { }

    private void CrystalStart() { }

    private EPlayerState CrystalUpdate()
    {
        var newState = HandleAiming();
        if (newState != _state)
        {
            return newState;
        }

        float angle = _player.GetAxis2D("Horizontal", "Vertical").ToAngleDeg();
        _targetCrystal.SetTargetAngle(Mathf.Round(angle / 45.0f) * 45.0f);

        if (_player.GetButtonDown("MainAction"))
        {
            _targetCrystal = null;
            _targetBody = null;
            if (_prevBody != null)
            {
                _targetBody = _prevBody;
                _zipTarget = _targetBody.transform;
            }
            return EPlayerState.Zip;
        }

        return EPlayerState.Crystal;
    }

    private void CrystalEnd() { }

    private void PossessStart() { }

    private EPlayerState PossessUpdate()
    {
        if (_player.GetButton("AimMode"))
        {
            return HandleAiming();
        }

        ResetAimLine();

        if (_targetBody != null)
        {
            _targetBody.SetIsAiming(false);
        }

        if (_player.GetButtonDown("MainAction"))
        {
            _targetBody.TriggerDead();
            _prevBody = _targetBody;
            _targetCrystal = null;
            _targetBody = null;
            _zipTarget = null;
            return EPlayerState.Zip;
        }

        transform.position = _currentRigidbody.position;

        _moveHorizontal = _player.GetButtonDown("Horizontal");
        _moveHorizontalNegative = _player.GetNegativeButtonDown("Horizontal");
        _xMove = _player.GetAxis("Horizontal");

        return EPlayerState.Possess;
    }

    private void PossessEnd() { }

    #endregion

    private void ResetAimLine()
    {
        _aimLine.Start = Vector3.zero;
        _aimLine.End = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * rayDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * rideHeight);
    }
}
