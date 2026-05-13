using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string sceneName;

    void Start()
    {
        // Get the button component attached to the same GameObject
        Button button = GetComponent<Button>();

        // Check if there is a button component
        if (button != null)
        {
            // Add an onClick listener to call the SwitchScene method when the button is pressed
            button.onClick.AddListener(SwitchScene);
        }
        else
        {
            Debug.LogError("No Button component found on this GameObject.");
        }
    }

    // Method to switch the scene
    public void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or null. Please specify a valid scene name.");
        }
    }
}
