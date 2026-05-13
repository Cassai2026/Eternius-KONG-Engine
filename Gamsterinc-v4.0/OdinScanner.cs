using UnityEngine;
using TMPro; // For the Crosshair/Interaction Text

public class OdinScanner : MonoBehaviour
{
    [Header("Scanner Specs")]
    public float scanRange = 4.0f; // How close the player needs to be
    public LayerMask interactableLayer; // Set your hazards to an 'Interactable' layer
    
    [Header("UI Hook")]
    public TextMeshProUGUI centerCrosshairText; // UI text in the middle of the screen

    private AnimusController animus;
    private ISovereignInteractable currentTarget;

    private void Start()
    {
        animus = GetComponentInParent<AnimusController>();
    }

    private void Update()
    {
        PerformRaycast();
        HandleInputs();
    }

    private void PerformRaycast()
    {
        // Shoot a laser from the center of the camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, scanRange, interactableLayer))
        {
            // Did we hit something with the Sovereign Interface?
            ISovereignInteractable interactable = hitInfo.collider.GetComponent<ISovereignInteractable>();
            
            if (interactable != null)
            {
                currentTarget = interactable;
                if(centerCrosshairText != null) 
                    centerCrosshairText.text = currentTarget.GetInteractText();
                return;
            }
        }

        // If we hit nothing, clear the target and text
        currentTarget = null;
        if(centerCrosshairText != null) centerCrosshairText.text = "";
    }

    private void HandleInputs()
    {
        if (currentTarget == null) return;

        // Press 'F' to run the O.D.I.N Scan (Reveal the hazard)
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentTarget.OnScanReveal();
        }

        // Press 'E' to Interact (Open the RAMS Terminal)
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.OnInteract(animus);
        }
    }
}
