using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeOnTrigger : MonoBehaviour
{
    // Index of the scene to load
    public int sceneIndex;

    private void Start()
    {
        // Debug to check if the script is running
        Debug.Log("SceneChangeOnTrigger script is running.");
    }

    // This function is called when another collider enters the trigger collider attached to the object where this script is also attached
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D called with: " + other.name); // Log the name of the object entering the trigger

        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Log a message to the console
            Debug.Log("Player has entered the trigger.");

            // Load the specified scene by index
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.Log("Entered object is not the player.");
        }
    }
}