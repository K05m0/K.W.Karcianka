using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    private Scene actualScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actualScene = SceneManager.GetActiveScene();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(actualScene.name);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
