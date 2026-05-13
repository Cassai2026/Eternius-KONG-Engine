using UnityEngine;
using TMPro; // For UI
using System;

public class SovereignOverseer : MonoBehaviour
{
    public static SovereignOverseer Instance { get; private set; }

    [Header("Sovereign Metrics")]
    public float sovereignCapital = 5000000f; // £5M Budget
    public float biologicalROI = 100f; // Starts at 100% capacity
    public float shiftTimeRemaining = 14400f; // 4 Hours in seconds (The Night Fix)

    [Header("UI Binding")]
    public TextMeshProUGUI capitalText;
    public TextMeshProUGUI broiText;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // 4D Time is always draining. 
        shiftTimeRemaining -= Time.deltaTime;
        UpdateUI();

        if (shiftTimeRemaining <= 0 || biologicalROI <= 0)
        {
            TriggerSystemicCollapse();
        }
    }

    public void ApplySlothPenalty(float timeLoss, float capitalLoss, float broiDamage)
    {
        // When a hazard is mismanaged, the system bleeds.
        shiftTimeRemaining -= timeLoss;
        sovereignCapital -= capitalLoss;
        biologicalROI -= broiDamage;
        Debug.Log($"[SOVEREIGN OVERRIDE] Sloth Penalty Applied. B-ROI: {biologicalROI}%");
    }

    public void ApplySovereignAction(float capitalGain)
    {
        // Correct RAMS execution restores the budget.
        sovereignCapital += capitalGain;
        Debug.Log("[SOVEREIGN OVERRIDE] Asset Reclaimed.");
    }

    private void UpdateUI()
    {
        if(capitalText) capitalText.text = $"Capital: £{sovereignCapital:N0}";
        if(broiText) broiText.text = $"B-ROI: {biologicalROI:F1}%";
        if(timeText) timeText.text = $"Time: {TimeSpan.FromSeconds(shiftTimeRemaining):hh\\:mm\\:ss}";
    }

    private void TriggerSystemicCollapse()
    {
        Debug.LogError("SYSTEMIC COLLAPSE. Shift Failed. Biological ROI Depleted.");
        // Trigger game over / reset state here
    }
}
