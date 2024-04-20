using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMover : MonoBehaviour
{
    [SerializeField]
    private LayerMask _collisionMask;

    private const float _skinWidth = .0015f;

    [SerializeField]
    private int _horizontalRayCount = 4;

    [SerializeField]
    private int _verticalRayCount = 4;

    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;

    private BoxCollider2D _collider;
    private RaycastOrigins _raycastOrigins;

    public CollisionInfo Collisions;

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        Collisions.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + _skinWidth;

        for (int i = 0; i < _horizontalRayCount; i++)
        {
            Vector2 rayOrigin =
                (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.right * directionX,
                rayLength,
                _collisionMask
            );

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - _skinWidth) * directionX;
                rayLength = hit.distance;

                Collisions.left = directionX == -1;
                Collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + _skinWidth;

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Vector2 rayOrigin =
                (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.up * directionY,
                rayLength,
                _collisionMask
            );

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - _skinWidth) * directionY;
                rayLength = hit.distance;

                Collisions.Below = directionY == -1;
                Collisions.Above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
        _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (_horizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft,
            topRight;
        public Vector2 bottomLeft,
            bottomRight;
    }

    public struct CollisionInfo
    {
        public bool Above,
            Below;
        public bool left,
            right;

        public void Reset()
        {
            Above = Below = false;
            left = right = false;
        }
    }
}
