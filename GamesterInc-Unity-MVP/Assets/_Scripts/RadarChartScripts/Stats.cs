using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Stats
{
   private static int STAT_MIN = 1;
   private static int STAT_MAX = 20;

   private SingeStat stat1, stat2, stat3, stat4, stat5;

   private SingeStat[] stats;

   public Stats(List<StatSetup> list)
   {
      stats = new SingeStat[list.Count];
      for (int i = 0; i < list.Count; i++)
      {
         stats[i] = new SingeStat(list[i].value, list[i].name);
      }
   }

   public Stats(int statAmount1, int statAmount2, int statAmount3, int statAmount4, int statAmount5)
   {
      stats = new SingeStat [5];
      stats[0] = new SingeStat(statAmount1);
      stats[1] = new SingeStat(statAmount2);
      stats[2] = new SingeStat(statAmount3);
      stats[3] = new SingeStat(statAmount4);
      stats[4] = new SingeStat(statAmount5);
   }

   public Stats(int[] statsAmount)
   {
      stats = new SingeStat [statsAmount.Length];
      for (int i = 0; i < statsAmount.Length; i++)
      {
         stats[i] = new SingeStat(statsAmount[i]);
      }
   }
   
   private SingeStat GetSingleStat(int statNumber)
   {
      int number = Mathf.Clamp(statNumber, 0, stats.Length-1);
      return stats[statNumber];
   }

   public void RandomizeStats()
   {
      System.Random r = new System.Random();
      foreach (SingeStat stat in stats)
      {
         stat.SetStatAmount(r.Next(STAT_MIN, STAT_MAX+1));
      }
   }

   public void SetStatAmount(int statNumber, int statAmount)
   {
      GetSingleStat(statNumber).SetStatAmount(statAmount);
   }

   public void ChangeStatAmount(int statNumber, int statChangeAmount)
   {
      SetStatAmount(statNumber, GetStatAmount(statNumber) + statChangeAmount);
   }

   public int GetStatsLength()
   {
      return stats.Length;
   }

   public string GetStatName(int statNumber)
   {
      return GetSingleStat(statNumber).GetStatName();
   }

   public int GetStatAmount(int statNumber)
   {
      return GetSingleStat(statNumber).GetStatAmount();
   }
   public float GetStatAmountNormalized(int statNumber)
   {
      return GetSingleStat(statNumber).GetStatAmountNormalized();
   }

   /// <summary>
   /// Default class for any stat on the radar chart
   /// </summary>
   private class SingeStat
   {
      private string name;
      private int stat;

      public SingeStat(int statAmount, string statName = "")
      {
         SetStatAmount(statAmount);
         name = statName;
      }

      public void SetStatAmount(int attackStatAmount)
      {
         stat = Mathf.Clamp(attackStatAmount, STAT_MIN, STAT_MAX);
      }

      public string GetStatName()
      {
         return name;
      }
      public int GetStatAmount()
      {
         return stat;
      }
      public float GetStatAmountNormalized()
      {
         return (float)stat / STAT_MAX;
      }
   }

   [Serializable]
   public class StatSetup
   {
      public string name;
      public int value;
   }
}
