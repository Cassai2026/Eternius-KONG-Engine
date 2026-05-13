using UnityEngine;
using UnityEngine.UI;

public class TimerCorrection : MonoBehaviour
{
    public float time = 120f; // Initial time in seconds
    public Text timerText; // Reference to the timer text UI element
    public Image fill; // Reference to the fill image for a visual timer
    public float maxTime; // Maximum time for the fill amount calculation

    void Start()
    {
        maxTime = time; // Set maxTime to the initial time value
        UpdateTimerUI(); // Initialize the timer UI
    }

    void Update()
    {
        // Decrease time based on the frame's delta time
        time -= Time.deltaTime;

        // Ensure time doesn't go below zero
        if (time < 0)
        {
            time = 0;
        }

        // Update the timer UI elements
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        // Update the timer text and fill amount
        timerText.text = "" + Mathf.Round(time).ToString();
        fill.fillAmount = time / maxTime;
    }

    public float GetRemainingTime()
    {
        return time; // Return the current remaining time
    }
}