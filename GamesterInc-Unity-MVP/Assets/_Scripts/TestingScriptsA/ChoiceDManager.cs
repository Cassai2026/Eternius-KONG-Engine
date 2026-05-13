using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class ChoiceDManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText; 

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    private static ChoiceDManager instance;
    private Story currentStory; 
    public bool dialogueIsPlaying { get; private set; }
    public delegate void DialogueStartAction();
    public static event DialogueStartAction OnDialogueStart;
    public delegate void DialogueEndAction();
    public static event DialogueEndAction OnDialogueEnd;

    private NPCController currentNPC;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
    }

    public static ChoiceDManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // Get all choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        // Add EventTrigger components and set up events for each choice
        foreach (GameObject choice in choices)
        {
            EventTrigger trigger = choice.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = choice.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnChoiceHover(choice); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerClick;
            entryClick.callback.AddListener((eventData) => { OnChoiceClick(choice); });
            trigger.triggers.Add(entryClick);
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))  // Key for talking to NPC
        {
            ContinueStory();
        }
    }

    // New method to enter dialogue mode with an NPC
    public void EnterDialogueModeWithNPC(NPCController npc)
    {
        currentNPC = npc;
        // Load the appropriate Ink JSON file for the NPC
        // For simplicity, assuming a single dialogue for demonstration
        // You might want to use different inkJSON based on the NPC state
        TextAsset inkJSON = Resources.Load<TextAsset>("NPCDialogue");
        EnterDialogueMode(inkJSON);
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        if (OnDialogueStart != null)
        {
            OnDialogueStart();
        }

        ContinueStory();
    }

    private void ExitDialogueMode(int choiceIndex = -1)
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        if (OnDialogueEnd != null)
        {
            OnDialogueEnd();
        }

        if (currentNPC != null)
        {
            // Check player's choice correctness and notify the NPC if a choice index is provided
            if (choiceIndex != -1)
            {
                bool playerAnsweredCorrectly = EvaluatePlayerChoice(choiceIndex);
               // currentNPC.HandleRuleBreakingInteraction(playerAnsweredCorrectly);
            }
            currentNPC = null;
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else
        {
            Debug.Log("Story ended.");
            ExitDialogueMode(); // No choice index needed when the story ends
        }
    }

    private void DisplayChoices()
    {
        List<Ink.Runtime.Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Ink.Runtime.Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        // Clear selected object to prevent accidental selection issues
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnChoiceHover(GameObject choice)
    {
        EventSystem.current.SetSelectedGameObject(choice);
    }

    private void OnChoiceClick(GameObject choice)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            if (choice == choices[i])
            {
                Debug.Log("Choice selected: " + i);
                ChooseChoice(i);
                return;
            }
        }
    }

    private void ChooseChoice(int choiceIndex)
    {
        if (currentStory.currentChoices.Count > choiceIndex)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            Debug.Log("Choice made: " + choiceIndex);
            ContinueStory();
            FindObjectOfType<ChoiceDTrigger>().HandlePlayerAnswer(EvaluatePlayerChoice(choiceIndex));
        }
        else
        {
            Debug.LogError("Invalid choice index: " + choiceIndex);
        }
    }

    // Dummy method to evaluate if the player's choice was correct
    // You can customize this based on your game's logic
    private bool EvaluatePlayerChoice(int choiceIndex)
    {
        // Check if the choice index is within the bounds of the current choices
        return choiceIndex >= 0 && choiceIndex < currentStory.currentChoices.Count && currentStory.currentChoices[choiceIndex].index == 0;
    }
}
