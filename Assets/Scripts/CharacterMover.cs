using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterMover : MonoBehaviour
{
    const float _skinWidth = .015f;
    public int _horizontalRayCount = 4;
    public int _verticalRayCount = 4;

    float _horizontalRaySpacing;
    float _verticalRaySpacing;

    BoxCollider2D _collider;
    RaycastOrigins _raycastOrigins;

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();

        for (int i = 0; i < _verticalRayCount; i++)
        {
            Debug.DrawRay(
                _raycastOrigins.bottomLeft + Vector2.right * _verticalRaySpacing * i,
                Vector2.up * -2,
                Color.red
            );
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
}
