using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Quest quest;

    // Call this method whenever an enemy is killed in the game
    public void EnemyKilled()
    {
        if (quest.isActive)
        {
            quest.goal.EnemyKilled();
            if (quest.goal.IsReached())
            {
                CoinCounter.instance.IncreaseCoins(quest.goldReward);
                quest.Complete();
            }
        }
    }
}