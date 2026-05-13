using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 0; // Default scene index to load, you can change this in the Unity inspector

    public void LoadAnotherScene()
    {
        Debug.Log("Loading scene with index: " + sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        // Show debug message
        Debug.Log("Quitting game...");

        // Quit the application
        Application.Quit();
    }
}