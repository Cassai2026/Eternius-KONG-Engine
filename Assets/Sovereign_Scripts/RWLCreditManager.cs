using UnityEngine;

public class RWLCreditManager : MonoBehaviour {
    public float totalCreditsMinted;
    public float creationEquity;

    public void MintCredits(float hoursWorked) {
        float newCredits = hoursWorked * 10f; // 10x Multiplier
        totalCreditsMinted += newCredits;
        creationEquity += (newCredits * 0.13f); // 13/33 Split Logic
    }
}
