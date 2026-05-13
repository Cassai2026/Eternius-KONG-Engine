using UnityEngine;
public class SovereignOverseer : MonoBehaviour {
    public static SovereignOverseer Instance;
    public float biologicalROI = 100f, sovereignCapital = 5000000f, shiftTime = 14400f;
    void Awake() { Instance = this; }
    void Update() { shiftTime -= Time.deltaTime; }
}
