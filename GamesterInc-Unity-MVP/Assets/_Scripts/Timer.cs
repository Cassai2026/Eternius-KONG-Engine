using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 60; // Initial time in seconds
    public Text countdownText;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Decrease time remaining by the time passed since last frame
            DisplayTime(timeRemaining);
        }
        else
        {
            timeRemaining = 0; // Ensure the timer doesn't go negative
            Debug.Log("Time's up!"); // You can perform any action here when the timer reaches 0
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Convert timeToDisplay to minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format the time text
    }
} 