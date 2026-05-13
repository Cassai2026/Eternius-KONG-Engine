using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueTrigger trigger;
    public QuestGiverScript questGiver;

    public Message[] messagesIncomplete; // Dialogue messages when quest is incomplete
    public Actor[] actorsIncomplete;     // Actors when quest is incomplete

    public Message[] messagesComplete;   // Dialogue messages when quest is complete
    public Actor[] actorsComplete;       // Actors when quest is complete
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reset dialogues
            trigger.messages = messagesIncomplete;
            trigger.actors = actorsIncomplete;

            // Determine which set of dialogues to use based on quest completion status
            if (!questGiver.quest.isActive && questGiver.quest.goal.IsReached())
            {
                trigger.messages = messagesComplete;
                trigger.actors = actorsComplete;
            }

            trigger.StartDialogue();
            OverworldManager.instance.questGiver = this.questGiver;
        }
    }

    public void DialogueEnded()
    {
        questGiver.quest.Complete(); // Mark the quest as completed
        questGiver.StartQuest(); // Call the method to start the quest in QuestGiverScript

        // Notify TargetIndicator that the quest status has changed
        TargetIndicator targetIndicator = FindObjectOfType<TargetIndicator>();
        if (targetIndicator != null)
            targetIndicator.quest = questGiver.quest;
    }

    public void QuestAccepted()
    {
        // Notify TargetIndicator that the quest status has changed
        TargetIndicator targetIndicator = FindObjectOfType<TargetIndicator>();
        if (targetIndicator != null)
            targetIndicator.quest = questGiver.quest;
    }
}