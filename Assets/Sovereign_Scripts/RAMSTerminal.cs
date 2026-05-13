using UnityEngine;
using System.Collections.Generic;

public class RAMSTerminal : MonoBehaviour {
    public List<string> methodStatementSteps = new List<string>();
    public bool isPillarAligned = false;

    public void SubmitRAMS(List<string> steps) {
        // 14+1 Pillar Validation Logic
        if (steps.Count >= 3) {
            isPillarAligned = true;
            SovereignOverseer.Instance.ApplySovereignAction(50000f);
            Debug.Log("[RAMS] COMPLIANCE VERIFIED. CAPITAL RECLAIMED.");
        } else {
            SovereignOverseer.Instance.ApplySlothPenalty(3600f, 15000f, 10f);
        }
    }
}
