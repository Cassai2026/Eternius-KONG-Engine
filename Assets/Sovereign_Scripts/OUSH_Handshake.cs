using UnityEngine;

public class OUSH_Handshake : MonoBehaviour {
    public void OnGestureDetected(string gestureName) {
        if (gestureName == "OUSH") {
            Debug.Log("[ANIMUS] OUSH HANDSHAKE DETECTED. FINALIZING BYTECODE.");
            SovereignOverseer.Instance.ApplySovereignAction(100000f);
        }
    }
}
