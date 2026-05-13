using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HazardNode : MonoBehaviour, ISovereignInteractable
{
    [Header("Sovereign Data")]
    public HazardData hazardProfile; // Drag your ScriptableObject here
    
    public bool isResolved = false;
    private bool isScanned = false;

    // Optional: A visual effect (like a red glowing outline) that turns on when scanned
    public GameObject scanHighlightVFX; 

    private void Start()
    {
        if (scanHighlightVFX != null) scanHighlightVFX.SetActive(false);
    }

    // What the UI shows when the player looks at it
    public string GetInteractText()
    {
        if (isResolved) return "[RESOLVED] " + hazardProfile.hazardName;
        if (!isScanned) return "Unidentified Anomaly. [PRESS F] to Scan.";
        
        return $"[HAZARD DETECTED] {hazardProfile.hazardName} - [PRESS E] to open RAMS Terminal.";
    }

    // Called when the O.D.I.N. Scanner hits it
    public void OnScanReveal()
    {
        if (!isScanned && !isResolved)
        {
            isScanned = true;
            if (scanHighlightVFX != null) scanHighlightVFX.SetActive(true);
            Debug.Log($"[O.D.I.N] Hazard Revealed: {hazardProfile.hazardName}");
            
            // Spike the player's cognitive load because they just realized they are in danger
            FindObjectOfType<AnimusController>().SpikeCognitiveLoad(10f);
        }
    }

    // Called when the player presses the Interact button (E)
    public void OnInteract(AnimusController player)
    {
        if (isResolved) return;

        if (!isScanned)
        {
            Debug.Log("[SYSTEM] Cannot interact. Hazard has not been scanned.");
            return;
        }

        // This is where we will trigger the RAMS UI Terminal to open
        Debug.Log($"[RAMS PROTOCOL INITIATED] Assessing: {hazardProfile.hazardName}");
        // RAMS_UIManager.Instance.OpenTerminal(this); // (We will build this next)
    }
}
