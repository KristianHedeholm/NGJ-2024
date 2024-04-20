using UnityEngine.SceneManagement;

public class SceneManager : Singleton<SceneManager>
{
    public void GetNextScene()
    {
        var sceneIndex = GetCurrentScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex + 1);
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
