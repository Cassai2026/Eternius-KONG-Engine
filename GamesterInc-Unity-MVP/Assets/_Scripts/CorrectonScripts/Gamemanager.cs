using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TimerCorrection timerCorrection; // Reference to the TimerCorrection script
    public Image[] strikeImages; // Array to hold the UI Image components for strikes
    public Sprite strikeSprite1; // Sprite for no strike
    public Sprite strikeSprite2; // Sprite for a strike
    public int maxStrikes = 3; // Maximum allowed strikes

    private int strikes; // Current strike count
    private bool gameOverAchieved; // Flag to check if the game over has been achieved

    void Start()
    {
        strikes = 0;
        gameOverAchieved = false;

        // Initialize all strike images to the first sprite (no strike)
        foreach (var img in strikeImages)
        {
            img.sprite = strikeSprite1;
        }
    }

    void Update()
    {
        if (!gameOverAchieved)
        {
            // Check for game over conditions based on time from TimerCorrection or strikes
            if (timerCorrection.GetRemainingTime() <= 0f || strikes >= maxStrikes)
            {
                GameOver();
            }
            else
            {
                Time.timeScale = 1f; // Ensure the game runs at normal speed if not game over
            }
        }
    }

    public void IncrementStrikes()
    {
        if (strikes < maxStrikes)
        {
            // Change the sprite of the next strike image
            strikeImages[strikes].sprite = strikeSprite2;
            strikes++;

            // Check if game over condition is met
            if (strikes >= maxStrikes)
            {
                GameOver();
            }
        }
    }

    public void AddTime()
    {
        // Call AddTime method on TimerCorrection
        timerCorrection.time += 10f;
    }

    private void GameOver()
    {
        if (!gameOverAchieved)
        {
            // Show game over panel
            FindObjectOfType<SurveillenceUI>().ShowGameOverPanel();
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Game Over!");

            // Award achievements
            if (AchievementManager.Instance is not null)
            {
                AchievementManager.Instance.EarnAchievment("Correction Minigame");
                AchievementManager.Instance.EarnAchievment("Master Correction");
            }

            gameOverAchieved = true; // Set the flag to true to prevent further awarding
        }
    }
}
