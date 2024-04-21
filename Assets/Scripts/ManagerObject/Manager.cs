using UnityEngine;

public class Manager : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsOfType<Manager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }
}
