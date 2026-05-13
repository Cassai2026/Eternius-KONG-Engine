using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievmentButton : MonoBehaviour
{
    public string achievmentName;
    public void OnButtonClick()
    {
        // Assuming AchievementManager is a singleton and has a method to earn achievements.
        AchievementManager.Instance.EarnAchievment(achievmentName);
    }
}
