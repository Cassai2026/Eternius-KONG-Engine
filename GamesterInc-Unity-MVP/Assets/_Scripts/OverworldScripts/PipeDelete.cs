using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PipeDelete : MonoBehaviour
{

    public int value;
    // Deletes pipe when collided with and increases coinCounter
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            AchievementManager.Instance.EarnAchievment("Collect Coins");
            Destroy(gameObject);
            CoinCounter.instance.IncreaseCoins(value);
        }
        
    }
}
