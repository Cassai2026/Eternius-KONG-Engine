using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public static bool paused = false;
    public GameObject PauseMenuCanvas;       // PauseMenuCanvas object in the game
    private Animator[] animators;            // Array to hold all animators in the scene

    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1f;
        animators = FindObjectsOfType<Animator>();   // Find all Animator components in the scene
    }

    // Update is called per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))    // If escape is pressed
        {
            if (InfernoCorrectionHolder.IsActive)
            {
                Debug.Log("Other menu is open");
                return;
            }
            
            if (paused)                     // If paused, continue the game
            {
                Continue();
            }
            else                                // Else, pause the game
            {
                Stop();
            }
        }
    }

    void Stop()                                 // Pause the game
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        SetAnimatorsEnabled(false);            // Disable all animators
    }

    public void Continue()                               // Let the game continue
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        SetAnimatorsEnabled(true);             // Enable all animators
    }

    public void MainMenuButton()          // Button for mainmenu
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);       // Goes to mainmenu scene when button is clicked
    }

    // New function to open pause menu when button is clicked
    public void OpenPauseMenu()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        SetAnimatorsEnabled(false);            // Disable all animators
    }
    public void DisablePauseMenu()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;  // Optionally resume the game time if needed
        paused = false;
        SetAnimatorsEnabled(true);  // Enable all animators
    }

    private void SetAnimatorsEnabled(bool enabled)
    {
        foreach (Animator animator in animators)
        {
            animator.enabled = enabled;
        }
    }
}