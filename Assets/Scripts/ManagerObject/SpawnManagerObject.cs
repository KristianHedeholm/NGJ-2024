using UnityEngine;

public class SpawnManagerObject : MonoBehaviour
{
    [SerializeField]
    private GameObject _SpawnObject;

    private void Awake()
    {
        DontDestroyOnLoad(_SpawnObject);
    }
}
