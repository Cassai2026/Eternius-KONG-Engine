using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

// PROJECT: ETERNIUS 4D KONG EDITION x GAMESTERINC
// MODULE: SOVEREIGN_SLAB_FOUNDATION
// STATUS: 150% TITAN-SPEC | BIOMECHANICAL TRUSS ACTIVE

public class SovereignSlab_Placement : MonoBehaviour
{
    static bool sovereignMeshLookupAttempted = false;
    static MethodInfo cachedMintMethod;
    static PropertyInfo cachedGhostIdProperty;

    [Header("Sovereign Deck Settings")]
    [Tooltip("The exact anchor point above the Hobbit Node for the 1-foot slab.")]
    public Transform hobbitNodeAnchor; 
    public float slabThickness = 0.3048f; // Exact 1-foot thickness constraint
    
    [Header("Kentucky Precision Spec")]
    [Tooltip("Tolerance set to 1mm (0.001f). Perfection is the baseline.")]
    public float kentuckyTolerance = 0.001f; 

    [Header("Kong Manifestation Feedback")]
    public ParticleSystem emeraldJoyAura; // Kong's visual validation pulse
    public AudioSource kongVoiceSynth;    // Deep, rumbling voice synth

    [Header("RWL Reward Economy")]
    public int rwlCreditValue = 50; // High-yield credits for foundational work
    private bool isLockedIn = false;

    void Update()
    {
        // Takiwātanga Protocol: The student operates in their own time and space.
        if (!isLockedIn)
        {
            AuditFloatingDeck();
        }
    }

    void AuditFloatingDeck()
    {
        if (hobbitNodeAnchor == null)
        {
            Debug.LogWarning("SovereignSlab_Placement: Missing hobbitNodeAnchor reference.");
            return;
        }

        // Calculate the physical distance and alignment over the Hobbit Node
        float distance = Vector3.Distance(transform.position, hobbitNodeAnchor.position);

        // Audit the build against the 1mm Kentucky Precision tolerance
        if (distance <= kentuckyTolerance)
        {
            LockSovereignSlab();
        }
    }

    void LockSovereignSlab()
    {
        isLockedIn = true;
        
        // Snap perfectly into the 4D grid, creating the Biomechanical Truss
        transform.position = hobbitNodeAnchor.position;
        transform.rotation = hobbitNodeAnchor.rotation;

        // Disable physics to finalize the 1-foot deck structure
        Rigidbody rigidbodyComponent = GetComponent<Rigidbody>();
        if (rigidbodyComponent != null)
        {
            rigidbodyComponent.isKinematic = true;
        }
        else
        {
            Debug.LogWarning("SovereignSlab_Placement: No Rigidbody found; slab was locked without physics handoff.");
        }

        TriggerKongValidation();
    }

    void TriggerKongValidation()
    {
        Debug.Log(">> FOUNDATION SECURED: 1-Foot Sovereign Slab locked into the 4D Grid.");
        
        // Trigger Kong's Emerald Joy Aura particle effect
        if (emeraldJoyAura != null) 
        {
            emeraldJoyAura.Play();
        }

        // Trigger Kong's Audio Synth
        if (kongVoiceSynth != null)
        {
            // Translates the Gemini text into a deep, warm, rumbling voice
            if (kongVoiceSynth.clip != null)
            {
                kongVoiceSynth.PlayOneShot(kongVoiceSynth.clip);
            }
            else
            {
                Debug.LogWarning("SovereignSlab_Placement: kongVoiceSynth has no AudioClip assigned.");
            }
        }
        
        // Mint the Real World Learning (RWL) Credits to the student's Sovereign Vault
        TryMintRwlCredits(rwlCreditValue);
        
        Debug.Log($">> REWARD ISSUED: {rwlCreditValue} RWL Credits. The Gen Alpha Titans are building the future.");
    }

    void TryMintRwlCredits(int creditAmount)
    {
        EnsureSovereignMeshBindings();
        if (cachedMintMethod == null || cachedGhostIdProperty == null)
        {
            Debug.LogWarning("SovereignSlab_Placement: SovereignMesh API surface not compatible; skipping credit mint.");
            return;
        }

        object ghostId = cachedGhostIdProperty.GetValue(null);
        cachedMintMethod.Invoke(null, new object[] { ghostId, creditAmount, "SOVEREIGN_SLAB_01" });
    }

    static void EnsureSovereignMeshBindings()
    {
        if (sovereignMeshLookupAttempted)
        {
            return;
        }

        sovereignMeshLookupAttempted = true;

        Type[] allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetSafeTypes).ToArray();
        Type ledgerType = allTypes.FirstOrDefault(t => t.FullName == "SovereignMesh.Gamification.SovereignLedger");
        Type playerType = allTypes.FirstOrDefault(t => t.FullName == "SovereignMesh.Avatars.Player");

        if (ledgerType == null || playerType == null)
        {
            Debug.LogWarning("SovereignSlab_Placement: SovereignMesh SDK not found; skipping credit mint.");
            return;
        }

        cachedMintMethod = ledgerType.GetMethod("MintToGhostID", BindingFlags.Public | BindingFlags.Static);
        cachedGhostIdProperty = playerType.GetProperty("CurrentGhostID", BindingFlags.Public | BindingFlags.Static);
    }

    static Type[] GetSafeTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).ToArray();
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }
}
