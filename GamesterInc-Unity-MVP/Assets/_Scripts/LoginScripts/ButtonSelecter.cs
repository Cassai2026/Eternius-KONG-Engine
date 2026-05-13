using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    public Button button1;
    public Button button2;

    private Button currentlySelectedButton;

    void Start()
    {
        // Set up initial selection
        button1.onClick.AddListener(() => SelectButton(button1));
        button2.onClick.AddListener(() => SelectButton(button2));
    }

    void SelectButton(Button button)
    {
        // If this button is already selected, do nothing
        if (button == currentlySelectedButton)
            return;

        // Change color of previously selected button back to default and lower its sibling index
        if (currentlySelectedButton != null)
        {
            //currentlySelectedButton.image.color = Color.white; // Default color (white)
            currentlySelectedButton.transform.SetAsFirstSibling(); // Move it below the other button
        }

        // Set new button as selected, change its color to blue, and raise its sibling index
        currentlySelectedButton = button;
        //currentlySelectedButton.image.color = new Color32(0x79, 0xB1, 0xF3, 0xFF); // Blue color in hexadecimal
        currentlySelectedButton.transform.SetAsLastSibling(); // Move it above the other button
    }
}