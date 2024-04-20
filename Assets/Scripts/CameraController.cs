using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float springFreq;

    [SerializeField]
    private float springDamp;

    [SerializeField]
    private float playerPositionBias;

    private PlayerController player;

    private Vector3 startPos;

    private Vector3 position;
    private Vector3 velocity;

    private Vector3 targetPosition;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();

        startPos = transform.position;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            targetPosition = Vector2.Lerp(startPos, player.transform.position, playerPositionBias);
            targetPosition.z = startPos.z;
        }

        SpringUtils.DoHarmonicSpringMotion(
            ref position,
            ref velocity,
            targetPosition,
            Time.deltaTime,
            springFreq,
            springDamp
        );

        transform.position = position;
    }
}
