using UnityEngine;

public class StretfordAlpha_LevelCore : MonoBehaviour {
    [Header("Site Zones")]
    public GameObject theSpine; // Elevated 3D printed deck
    public GameObject theDitch;  // A56 Liability zone
    
    void Start() {
        Debug.Log("[SYSTEM] STRETFORD ALPHA INITIALIZED. LOADING 4D MESH...");
        // Apply 47 MSI Structural Graphite properties to the Spine
    }

    public void TriggerNightFix() {
        RenderSettings.ambientIntensity = 0.5f;
        Debug.Log("[SOVEREIGN] NIGHT-FIX PROTOCOL ACTIVE. ILLUMINATING THE SPINE.");
    }
}
