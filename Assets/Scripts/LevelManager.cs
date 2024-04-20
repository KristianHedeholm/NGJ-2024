using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public void GetNextScene()
    {
        if (!CanGetNextScene())
        {
            return;
        }

        var sceneIndex = GetCurrentScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex + 1);
    }

    public bool CanGetNextScene()
    {
        var sceneIndex = GetCurrentScene().buildIndex;
        var numberOfScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;
        return sceneIndex < numberOfScenes;
    }

    public void ResetScene()
    {
        var sceneIndex = GetCurrentScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void GoBackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private Scene GetCurrentScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }
}
