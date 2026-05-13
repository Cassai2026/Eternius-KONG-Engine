using UnityEngine;
using UnityEngine.SceneManagement;

public class SurveillenceUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public string sceneToLoad = "Overworld";

    private void Start()
    {
        // Initially hide the game over panel
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reload the current scene
         // Replace with your actual scene name
        SceneManager.LoadScene(sceneToLoad);
    }
}
