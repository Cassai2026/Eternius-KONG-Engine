using UnityEngine;

// Any object in the game that can be interacted with MUST inherit this interface.
public interface ISovereignInteractable
{
    string GetInteractText();
    void OnInteract(AnimusController player);
    void OnScanReveal();
}
