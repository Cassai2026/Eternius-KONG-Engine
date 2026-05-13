using UnityEngine;
using SovereignMesh.Gamification; // The GamesterInc x CassAI Ledger

// PROJECT: GAMESTERINC CSCS GAMIFIED TEST LEVEL
// MODULE: CSCS_TAKIWATANGA_AUDIT
// STATUS: 150% TITAN-SPEC | TAKIWĀTANGA PROTOCOL ACTIVE

public class CSCS_Takiwatanga_Audit : MonoBehaviour
{
    [Header("CSCS Site Safety Settings")]
    [Tooltip("List of interactive hazards (e.g., exposed wires, missing edge protection, no hard hat).")]
    public GameObject[] siteHazards;
    
    [Header("Takiwātanga Protocol (No Ticking Clocks)")]
    [Tooltip("True = The student is evaluated in their own time and space, eliminating test anxiety.")]
    public bool disableTestTimers = true;

    [Header("CSCS Certification Economy")]
    public int totalHazardsIdentified = 0;
    public int requiredToPassCSCS = 10; // Number of safe actions required to pass the game level
    private bool cscsPassed = false;

    void Start()
    {
        if (disableTestTimers)
        {
            Debug.Log(">> CSCS EXAM: Timers Disabled. The learner operates in their own time and space.");
        }
    }

    // This function triggers when the student physically clicks/interacts with a hazard in the game to 'fix' it
    public void OnHazardFixed(GameObject resolvedHazard)
    {
        if (!cscsPassed)
        {
            totalHazardsIdentified++;
            Debug.Log($">> CSCS LOG: Hazard secured. Score: {totalHazardsIdentified}/{requiredToPassCSCS}");
            
            // Visual feedback - replacing the hazard with a safe structure
            resolvedHazard.GetComponent<Renderer>().material.color = Color.green;

            EvaluateCSCSCompetence();
        }
    }

    void EvaluateCSCSCompetence()
    {
        // The Dimensional Model: We assess based on continuous safe actions, not a written exam
        if (totalHazardsIdentified >= requiredToPassCSCS)
        {
            IssueCSCSClearance();
        }
    }

    void IssueCSCSClearance()
    {
        cscsPassed = true;
        
        Debug.Log(">> CSCS PASSED: Practical safety competence demonstrated dynamically.");
        
        // Trigger the visual "Level Complete" sequence for GamesterInc
        KongManifestation.TriggerApproval("OUSH! SITE SECURED! CSCS PASSED!");

        // Mint the achievement to the student's local profile
        SovereignLedger.MintToGhostID(Player.CurrentGhostID, 100, "GAMESTERINC_CSCS_LEVEL_1_CLEARED");
    }
}
