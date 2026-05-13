using UnityEngine;
using System;

[Serializable]
public struct Pillar {
    public string name;
    public float virtueScore;
    public float sinScore;
}

public class PillarGovernance : MonoBehaviour {
    public Pillar[] pillars = new Pillar[15];
    
    public void UpdateAlignment(int index, float push, float pull) {
        pillars[index].virtueScore += push;
        pillars[index].sinScore += pull;
        if (pillars[index].sinScore > pillars[index].virtueScore) {
            Debug.LogError("[LILIETH] KERNEL PANIC: ALIGNMENT DRIFT DETECTED.");
        }
    }
}
