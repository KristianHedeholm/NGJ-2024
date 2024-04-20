using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Change so this works with Rewind.
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.GetNextScene();
        }
    }
}
