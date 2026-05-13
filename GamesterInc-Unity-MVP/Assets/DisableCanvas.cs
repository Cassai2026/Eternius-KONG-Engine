using UnityEngine;
using UnityEngine.UI;

public class DisableCanvas : MonoBehaviour
{
    public Canvas canvasToDisable;

    // This method is called when the button is clicked
    public void OnButtonClick()
    {
        // Check if the canvas is not null
        if (canvasToDisable != null)
        {
            // Deactivate the canvas
            canvasToDisable.gameObject.SetActive(false);
        }
    }
}
