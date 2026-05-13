using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest              // References related to the quest system
{
    public bool isActive;

    public string title;
    public string description;
    public int experienceReward;
    public int goldReward;
    public string minigameSceneName;

    public QuestGoal goal;

    public void Complete()          // When a quest is completed
    {
        isActive = false;
        Debug.Log(title + " was completed!");
    }
}