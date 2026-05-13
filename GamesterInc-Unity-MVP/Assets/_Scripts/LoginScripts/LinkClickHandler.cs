using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LinkClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject objectToActivate;

    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text tmpText = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, Input.mousePosition, null);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID() == "example")
            {
                // Activate the game object
                objectToActivate.SetActive(true);
            }
        }
    }
}