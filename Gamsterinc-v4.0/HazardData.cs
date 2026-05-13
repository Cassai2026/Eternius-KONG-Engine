using UnityEngine;

[CreateAssetMenu(fileName = "NewHazard", menuName = "Sovereign/Hazard Data")]
public class HazardData : ScriptableObject
{
    public string hazardName;
    [TextArea(3, 5)]
    public string description;

    [Header("The Rinse (Penalties)")]
    public float broiDamage = 15f; // How much it hurts the player's health
    public float slothTimeDelay = 3600f; // 1 hour lost if not fixed
    public float capitalBleed = 15000f; // Fines/damage cost

    [Header("The Fix (RAMS Requirement)")]
    public string requiredTool; // e.g., "Insulated Gloves", "Trench Shore"
    public string correctMethodStep1;
    public string correctMethodStep2;
}
