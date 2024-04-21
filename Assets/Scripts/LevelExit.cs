using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent<Body>(out var body);
        if (player.CurrentBody == body)
        {
            LevelManager.Instance.GetNextScene();
        }
    }
}
