using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceDTrigger : MonoBehaviour
{
    [Header("Visual Cues")]
    [SerializeField] private List<GameObject> visualCues = new List<GameObject>();
    [Header("Ink Json")]
    [SerializeField] private TextAsset inkJSON;
    private bool npcInRange;
    private bool dialogueCooldown;
    private NPCController currentNPC;

    // Time to wait before allowing a new dialogue (in seconds)
    private float dialogueCooldownTime = 5f;
    private float dialogueCooldownTimer;

    private void Awake()
    {
        npcInRange = false;
        dialogueCooldown = false;
        SetVisualCuesActive(false);

        // Subscribe to the dialogue end event
        ChoiceDManager.OnDialogueEnd += HandleDialogueEnd;
    }
    
    

    private void Update()
    {
        if (npcInRange && !ChoiceDManager.GetInstance().dialogueIsPlaying && !dialogueCooldown)
        {
            SetVisualCuesActive(true);
            if (Input.GetKeyDown(KeyCode.E)) // Key to talk to NPC
            {
                Debug.Log("Interacting with NPC.");
                ChoiceDManager.GetInstance().EnterDialogueMode(inkJSON);        // Gets data from other file
            }
        }
        else
        {
            SetVisualCuesActive(false); // Hide visual cues
        }

        // Update the dialogue cooldown timer
        if (dialogueCooldown)
        {
            dialogueCooldownTimer -= Time.deltaTime;
            if (dialogueCooldownTimer <= 0)
            {
                dialogueCooldown = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("NPC")) // Tag the NPC with "NPC" tag
        {
            npcInRange = true;
            currentNPC = collider.gameObject.GetComponent<NPCController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("NPC"))
        {
            npcInRange = false;
        }
    }

    private void HandleDialogueEnd()
    {
        // Handle what should happen when the dialogue ends
        // For example, you could deactivate the VisualCues here
        SetVisualCuesActive(false);

        // Start the dialogue cooldown
        StartDialogueCooldown();
    }

    private void SetVisualCuesActive(bool active)
    {
        foreach (GameObject visualCue in visualCues)
        {
            visualCue.SetActive(active);
        }
    }

    private void StartDialogueCooldown()
    {
        dialogueCooldown = true;
        dialogueCooldownTimer = dialogueCooldownTime;
    }
    
    public void HandlePlayerAnswer(bool playerAnsweredCorrectly)
    {
        if (playerAnsweredCorrectly)
        {
            // Inform the NPC controller about the correct answer
         //   currentNPC.HandleRuleBreakingInteraction(playerAnsweredCorrectly);
        }
        else
        {
            // Handle incorrect answer if needed
        }
    }
}