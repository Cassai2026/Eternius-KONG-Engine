using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonReward : MonoBehaviour
{
    [SerializeField] private DungeonPlayer player;
    [SerializeField] private DungeonCharacter.Skill skill;

    public void SetUp(DungeonPlayer pPlayer)
    {
        player = pPlayer;
    }

    private void Update()
    {
        if (player is null) return;
        if (Vector2.Distance(transform.position, player.transform.position) < 1)
        {
            GiveSkill();
            Destroy(gameObject);
        }
    }

    private void GiveSkill()
    {
        player.skills[0] = skill;
    }
}
