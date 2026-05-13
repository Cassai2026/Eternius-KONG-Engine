using UnityEngine;
using UnityEngine.Rendering;

public class SomaticShield : MonoBehaviour {
    public float heartRateBaseline = 60f;
    public float cognitiveLoad;

    public void Regulate(float currentHR) {
        if (currentHR > 100f) {
            // Trigger MEDUSA PROTOCOL
            Debug.LogWarning("[HEKETE] CORTISOL SPIKE. ENGAGING MEDUSA SHIELD.");
        }
    }
}
