using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerCorrection : MonoBehaviour
{
    public TMP_Text textComponent;
    public float textSpeed;
    public float lineDelay = 2f;
    public GameObject Dialoguebox;
    public Button[] choiceButtons; // Array to hold references to the choice buttons

    private string[] lines;
    private int index;
    private System.Action<int> onChoiceSelected; // Action to handle choice selection

    void Start()
    {
        textComponent.text = string.Empty;
        
        HideChoices();
    }

    public void SetLines(string[] newLines)
    {
        lines = newLines;
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        textComponent.text = string.Empty;
        Dialoguebox.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(lineDelay);
        if (index == lines.Length - 1)
        {
            ShowChoices();
        }
        else
        {
            NextLine();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Dialoguebox.SetActive(false);
        }
    }
 
    void ShowChoices()
    {
        // Example choices for demonstration
        string[] possibleChoices = { "Stop smoking", "Wear your hard hat", "nothing's wrong" };

        // Randomize the choice order
        System.Random rnd = new System.Random();
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int randIndex = rnd.Next(possibleChoices.Length);
            choiceButtons[i].GetComponentInChildren<Text>().text = possibleChoices[randIndex];
            possibleChoices = RemoveAt(possibleChoices, randIndex);
            choiceButtons[i].gameObject.SetActive(true);
            int index = i; // Capture the current index for the callback
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
        }
    }

    // Utility function to remove an element from an array
    string[] RemoveAt(string[] array, int index)
    {
        if (array.Length == 0 || index < 0 || index >= array.Length) return array;
        string[] newArray = new string[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i == index) continue;
            newArray[j++] = array[i];
        }
        return newArray;
    }

    void HideChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

    void OnChoiceSelected(int choiceIndex)
    {
        HideChoices();
        Dialoguebox.SetActive(false); // Hide the dialogue box
        textComponent.text = string.Empty; // Clear the text

        // Callback for NPCController to handle the player's choice
        onChoiceSelected?.Invoke(choiceIndex);
    }

    public void SetChoiceCallback(System.Action<int> choiceCallback)
    {
        onChoiceSelected = choiceCallback;
    }

    public string GetChoiceText(int choiceIndex)
    {
        return choiceButtons[choiceIndex].GetComponentInChildren<Text>().text;
    }
}
