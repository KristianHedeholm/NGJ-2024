using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        AudioManager.Instance.StartAmbience(FMODEvents.Instance.MenuMusic);
    }

    // Update is called once per frame
    void Update()
    {
        // Change so this works with Rewind.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.GetNextScene();
            AudioManager.Instance.StopAmbience();
            AudioManager.Instance.StartAmbience(FMODEvents.Instance.MainMusic);
        }
    }
}
