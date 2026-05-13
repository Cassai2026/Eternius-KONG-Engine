using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Adjust if using HDRP

public class MedusaOverlay : MonoBehaviour
{
    public static MedusaOverlay Instance { get; private set; }

    [Header("Post Processing Volume")]
    public Volume globalVolume;
    private Vignette medusaVignette;
    private ColorAdjustments colorGrading;

    [Header("Audio Regulation")]
    public AudioSource backgroundChaosNoise;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Grab the camera filters
        globalVolume.profile.TryGet(out medusaVignette);
        globalVolume.profile.TryGet(out colorGrading);
    }

    public void EngageMedusaLock(string hexColor)
    {
        Debug.LogWarning("[HEKETE OVERRIDE] Cortisol Spike Detected. MEDUSA PROTOCOL ENGAGED.");
        
        // Parse the Sovereign Hex Color (#FF0000)
        Color threatColor;
        ColorUtility.TryParseHtmlString(hexColor, out threatColor);

        // 1. Crush peripheral vision to force focus (Tunnel Vision)
        if (medusaVignette != null)
        {
            medusaVignette.intensity.value = 0.65f; 
            medusaVignette.color.value = threatColor;
        }

        // 2. Desaturate the world to remove visual "Sloth"
        if (colorGrading != null)
        {
            colorGrading.saturation.value = -80f; 
        }

        // 3. Mute construction site noise, isolate breathing/heartbeat audio
        if (backgroundChaosNoise != null)
        {
            backgroundChaosNoise.volume = Mathf.Lerp(backgroundChaosNoise.volume, 0.1f, Time.deltaTime * 5f);
        }
    }

    public void DisengageMedusa()
    {
        // Return to Flow State
        if (medusaVignette != null) medusaVignette.intensity.value = 0.2f;
        if (colorGrading != null) colorGrading.saturation.value = 0f;
        if (backgroundChaosNoise != null) backgroundChaosNoise.volume = 1.0f;
    }
}
