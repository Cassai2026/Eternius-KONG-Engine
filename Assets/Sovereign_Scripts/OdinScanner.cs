using UnityEngine;
using TMPro;

public class OdinScanner : MonoBehaviour {
    public float scanRange = 10f;
    public LayerMask hazardLayer;
    public TextMeshProUGUI hudDisplay;

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) ExecuteOdinScan();
    }

    void ExecuteOdinScan() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, scanRange, hazardLayer)) {
            HazardNode node = hit.collider.GetComponent<HazardNode>();
            if (node != null) {
                node.OnScanReveal();
                hudDisplay.text = "[O.D.I.N] HAZARD DETECTED: " + node.hazardProfile.hazardName;
            }
        }
    }
}
