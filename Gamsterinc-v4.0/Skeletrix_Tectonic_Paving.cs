using UnityEngine;
using SovereignMesh.AI;           // The Lilieth & Enki Core
using SovereignMesh.Gamification; // The GamesterInc x CassAI Ledger

// PROJECT: GAMESTERINC v4.0 - OPERATION SKELETRIX
// MODULE: SKELETRIX_TECTONIC_PAVING
// STATUS: 150% TITAN-SPEC | KINETIC HARVESTING ACTIVE

public class Skeletrix_Tectonic_Paving : MonoBehaviour
{
    [Header("Operation Skeletrix Settings")]
    [Tooltip("The piezoceramic graphite layer that absorbs vehicle impact.")]
    public bool isGraphiteMeshAligned = false;
    
    [Header("Kinetic Energy Harvest")]
    public float energyPerVehiclePass = 0.05f; // 0.05Wh per kinetic impact
    private float localGridPowerStored = 0f;

    [Header("RWL Equity & Visuals")]
    public ParticleSystem kineticGlowAura; // The road glows naturally from pressure
    public Light localStreetlight;         // The streetlight powered by the road

    void OnTriggerEnter(Collider other)
    {
        // Audit: Is a VR vehicle driving over the student's completed road?
        if (isGraphiteMeshAligned && other.CompareTag("VR_Vehicle"))
        {
            HarvestKineticEnergy(other.attachedRigidbody.mass);
        }
    }

    // Called when the student successfully lays the 3D Piezo-Glass panels
    public void OnPavingCompleted()
    {
        isGraphiteMeshAligned = true;
        Debug.Log(">> SKELETRIX ACTIVE: Copper-Graphite Induction Mesh locked. Frictionless levitation enabled.");
        
        // Reward the student for building energy-positive infrastructure
        SovereignLedger.MintToGhostID(Player.CurrentGhostID, 75, "SKELETRIX_ROAD_LAID");
        KongManifestation.TriggerApproval("OUSH! THE ROAD IS ALIVE! WE ARE POWERING THE SPINE!");
    }

    void HarvestKineticEnergy(float vehicleWeightKg)
    {
        // Calculate the "Free Energy" based on the vehicle's weight and impact
        float harvestedJoules = energyPerVehiclePass * (vehicleWeightKg / 1000f);
        localGridPowerStored += harvestedJoules;

        // Visual Feedback: The road flexes and glows under pressure, eliminating the need for grid power
        if (kineticGlowAura != null) kineticGlowAura.Play();

        // Route the harvested power directly to the community streetlights
        if (localGridPowerStored >= 1.0f)
        {
            PowerLocalInfrastructure();
        }
    }

    void PowerLocalInfrastructure()
    {
        if (localStreetlight != null && !localStreetlight.enabled)
        {
            localStreetlight.enabled = true;
            Debug.Log(">> LOCAL UTILITY OVERRIDE: Streetlight powered by 100% Kinetic Sovereign Energy.");
            
            // Deduct the power used from the local reservoir
            localGridPowerStored -= 1.0f; 
        }
    }
}
