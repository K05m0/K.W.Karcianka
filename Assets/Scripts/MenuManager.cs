using UnityEngine;
using UnityEngine.SceneManagement; // For scene loading
using UnityEngine.UI; // For UI elements

public class MainMenu : MonoBehaviour
{
    // Method to load the MainGame scene
    public void PlayGame()
    {
        // Replace "MainGame" with the actual name of your main game scene
        SceneManager.LoadScene("MainGame");
    }

    // Method to quit the game
    public void QuitGame()
    {
        // This will quit the application. It works in a built game, but not in the editor
        Application.Quit();

        // For testing purposes in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
