using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private RadarChart uiStatRadarChart;
    [SerializeField] List<Stats.StatSetup> setupList;
    [SerializeField] private int[] statsList;
    Stats stats = new Stats(10, 2, 4, 18, 15);

    private int index = 0;
    
    void Start()
    {
        if (setupList.Count >= 3)
        {
            stats = new Stats(setupList);
        }
        else if (statsList.Length >= 3)
        {
            stats = new Stats(statsList);
        }

        uiStatRadarChart.SetStats(stats);
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            stats.ChangeStatAmount(index, 1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            stats.ChangeStatAmount(index, -1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            index++;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            index--;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            stats.RandomizeStats();
        }

        if (index < 0)
        {
            index = stats.GetStatsLength()-1;
        } else if (index >= stats.GetStatsLength())
        {
            index = 0;
        }

        uiStatRadarChart.SetStats(stats);
    }
}
