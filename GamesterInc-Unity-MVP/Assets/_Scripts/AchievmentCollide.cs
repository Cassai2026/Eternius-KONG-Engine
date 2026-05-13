using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievmentCollide : MonoBehaviour
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!EventSystem.current.IsPointerOverGameObject(-1))
        {
            AchievementManager.Instance.EarnAchievment(achievmentName);
        }
    }
}
