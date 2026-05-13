using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestGiverScript : MonoBehaviour
{
    public Quest quest;
    public Player player;
    public GameObject questWindow;
    public Text titleText;
    public Text descriptionText;
    public Text goldText;
    public Text experienceText;

    public static bool IsQuestWindowActive = false; // Static flag

    public void StartQuest()
    {
        if (!quest.isActive && !quest.goal.IsReached()) // Check if the quest is not active and not completed
        {
            Debug.Log(quest.title);
            OpenQuestWindow();
        }
    }

    public void OpenQuestWindow()
    {
        questWindow.SetActive(true);
        IsQuestWindowActive = true; // Set flag to true
        titleText.text = quest.title;
        descriptionText.text = quest.description;
        goldText.text = quest.goldReward.ToString();
        //experienceText.text = quest.experienceReward.ToString(); // Added this line to display experience reward
    }

    // If quest is accepted it hides the quest windows & gives the quest to player + loads them to minigame
    public void AcceptQuest() 
    {
        questWindow.SetActive(false);
        IsQuestWindowActive = false; // Set flag to false
        quest.isActive = true;
        // Give to the player
        player.quest = quest;

        if (!string.IsNullOrEmpty(quest.minigameSceneName))
        {
            SceneManager.LoadScene(quest.minigameSceneName);
        }
    }

    // Checks if quest accepted
    public void QuestAccepted()             
    {
        NPC npc = GetComponent<NPC>();
        if (npc != null)
            npc.QuestAccepted();
    }

    // Checks if goal of quest is reached
    public void MinigameCompleted()         
    {
        if (!quest.isActive && !quest.goal.IsReached())
        {
            quest.goal.MinigameCompleted();
            CheckQuestCompletion(); 
        }
    }

    // Check if the quest is completed
    private void CheckQuestCompletion()
    {
        if (quest.goal.IsReached())
        {
            quest.Complete(); 
        }
    }

    // Method to close the quest window
    public void CancelQuest()
    {
        questWindow.SetActive(false);
        IsQuestWindowActive = false; // Set flag to false
    }
}