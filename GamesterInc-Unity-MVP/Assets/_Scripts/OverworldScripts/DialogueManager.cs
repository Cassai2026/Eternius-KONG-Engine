using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image ActorImage;
    public Text ActorName;
    public Text MessageText;
    public RectTransform BackgroundBox;
    public GameObject choiceButtonPrefab; // Prefab for choice buttons
    public Transform choiceButtonContainer; // Container to hold choice buttons

    private Message[] currentMessages;
    private Actor[] currentActors;
    private int activeMessage = 0;
    public static bool isActive = false;

    public NPC npc; // Reference to the NPC

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;

        Debug.Log("Started Conversation! Loaded messages: " + messages.Length);
        DisplayMessage();
        BackgroundBox.LeanScale(Vector3.one, 0.5f);
    }

    private void DisplayMessage()
    {

        Message messageToDisplay = currentMessages[activeMessage];
        MessageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        ActorName.text = actorToDisplay.name;
        ActorImage.sprite = actorToDisplay.sprite;

        AnimateTextColor();
    }

    private IEnumerator WaitForNextMessageInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        NextMessage();
    }


    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversation ended!");
            BackgroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
            isActive = false;

            npc.DialogueEnded(); // Notify NPC that dialogue ended
        }
    }

    private void AnimateTextColor()
    {
        LeanTween.textAlpha(MessageText.rectTransform, 0, 0);
        LeanTween.textAlpha(MessageText.rectTransform, 1, 0.5f);
    }

    void Start()
    {
        BackgroundBox.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isActive)
        {
            NextMessage();
        }
    }
}