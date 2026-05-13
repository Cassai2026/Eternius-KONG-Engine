using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementTrigger : MonoBehaviour
{
    public string achievmentName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Ensure the pointer is not over a UI element
            if (!EventSystem.current.IsPointerOverGameObject(-1))
            {
                // Trigger the achievement
                AchievementManager.Instance.EarnAchievment(achievmentName);
            }
        }
    }
}
