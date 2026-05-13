using UnityEngine;

public class AnimusController : MonoBehaviour
{
    [Header("Animus Diagnostics")]
    public float cognitiveLoad = 0f; // 0 (Flow) to 100 (Panic)
    public float heartRate = 60f; // Base resting HR

    private float baseSpeed = 5f;

    void Update()
    {
        HandleMovement();
        RegulateSomaticState();
    }

    void HandleMovement()
    {
        // Standard WASD movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // The higher the cognitive load, the slower/clunkier the movement (Administrative Sloth)
        float currentSpeed = baseSpeed * (1f - (cognitiveLoad / 200f)); 

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * currentSpeed * Time.deltaTime;
    }

    public void SpikeCognitiveLoad(float stressAmount)
    {
        cognitiveLoad += stressAmount;
        heartRate += (stressAmount * 0.5f);
        cognitiveLoad = Mathf.Clamp(cognitiveLoad, 0, 100);
        
        // Pass the damage up to the Macro Overseer
        if (SovereignOverseer.Instance != null)
        {
            SovereignOverseer.Instance.ApplySlothPenalty(0, 0, stressAmount * 0.2f);
        }
        else
        {
            Debug.LogWarning("AnimusController: SovereignOverseer.Instance is null; skipping sloth penalty propagation.");
        }
    }

    void RegulateSomaticState()
    {
        // Slowly recover if standing in a safe, cleared zone
        if (cognitiveLoad > 0)
        {
            cognitiveLoad -= Time.deltaTime * 2f; 
            heartRate = Mathf.Lerp(heartRate, 60f, Time.deltaTime * 0.1f);
        }
    }
}
