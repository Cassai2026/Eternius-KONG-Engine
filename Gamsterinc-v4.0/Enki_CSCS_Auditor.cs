using UnityEngine;
using SovereignMesh.AI;           // The Enki Logic Node
using SovereignMesh.Gamification; // The GamesterInc x CassAI Ledger

// PROJECT: GAMESTERINC CSCS GAMIFIED TEST LEVEL
// MODULE: ENKI_SOVEREIGN_AUDITOR
// STATUS: 150% TITAN-SPEC | HSE LEGACY OVERRIDE ACTIVE

public class Enki_CSCS_Auditor : MonoBehaviour
{
    [Header("Enki Anti-Cheat Metrics")]
    [Tooltip("The student must physically focus on the hazard in 3D space to process it.")]
    public float requiredObservationTime = 2.5f; 
    private float currentObservationTime = 0f;
    private bool isFocusedOnHazard = false;

    [Header("HSE Legacy Filter (No Noise)")]
    [Tooltip("Strips away bureaucratic fluff and delivers raw, life-saving safety physics.")]
    public string sovereignSafetyData = "Trench collapse imminent. Angle of repose compromised. Brace immediately.";

    void Update()
    {
        // Enki constantly audits the player's cognitive focus in the 4D space
        AuditCognitiveFocus();
    }

    void AuditCognitiveFocus()
    {
        // Simulating a Raycast from the player's Augmented Cognitive Device (ACD) HUD
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isFocusedOnHazard = true;
                currentObservationTime += Time.deltaTime;
            }
            else
            {
                isFocusedOnHazard = false;
            }
        }
    }

    // Called when the student attempts to click/interact to "fix" the hazard
    public void AttemptToSecureHazard()
    {
        if (currentObservationTime < requiredObservationTime)
        {
            TriggerEnkiRejection();
        }
        else
        {
            ValidateSovereignSafety();
        }
    }

    void TriggerEnkiRejection()
    {
        Debug.Log(">> ENKI AUDIT FAILED: Speedrunning detected. Safety is not a guessing game.");
        
        // Enki manifests to provide a sharp, tactical correction
        EnkiManifestation.TriggerWarning("Slow down. Observe the environment. You are guessing, not auditing.");
    }

    void ValidateSovereignSafety()
    {
        Debug.Log($">> ENKI AUDIT PASSED: {sovereignSafetyData}");
        
        // Deliver the exact, bullshit-free safety information to the student's HUD
        ACD_HUD.DisplaySovereignTruth(sovereignSafetyData);

        // Mint the achievement to the Sovereign Ledger
        SovereignLedger.MintToGhostID(Player.CurrentGhostID, 25, "HSE_HAZARD_SECURED");
        
        // Disable this specific hazard auditor once secured
        this.enabled = false; 
    }
}
