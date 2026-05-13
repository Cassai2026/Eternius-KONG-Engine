using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DesignatedArea : MonoBehaviour
{
    // List to track NPCs in the smoking area
    public List<GameObject> npcsInSmokingArea = new List<GameObject>();

    // Reference to the Tilemap component
    public Tilemap tilemap;

    // Bounds for the overlap detection
    public Vector2 areaSize = new Vector2(5f, 5f); // Adjust this size to fit your tilemap

    void Start()
    {
        // Get the Tilemap component from the GameObject this script is attached to
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        // Update the smoking status of each NPC in the list
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, areaSize, 0f);

        // Track which NPCs are currently detected
        List<GameObject> currentlyDetectedNPCs = new List<GameObject>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                currentlyDetectedNPCs.Add(collider.gameObject);

                // If this NPC is not in the list, add it
                if (!npcsInSmokingArea.Contains(collider.gameObject))
                {
                    npcsInSmokingArea.Add(collider.gameObject);
                }
            }
        }

        // Check if any NPCs have left the area
        for (int i = npcsInSmokingArea.Count - 1; i >= 0; i--)
        {
            GameObject npc = npcsInSmokingArea[i];
            if (!currentlyDetectedNPCs.Contains(npc))
            {
                npcsInSmokingArea.Remove(npc);
            }
        }
    }

    // Check if the NPC is in the safe zone
    public bool IsNpcInSafeZone(GameObject npc)
    {
        return npcsInSmokingArea.Contains(npc);
    }

    // Visualization of the overlap area in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
