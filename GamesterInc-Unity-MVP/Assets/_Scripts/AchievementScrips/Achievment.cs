using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Achievment
{
    private string name;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    private string description;

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    private bool unlocked;

    public bool Unlocked
    {
        get { return unlocked; }
        set { unlocked = value; }
    }

    private int points;

    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    private int spriteIndex;

    public int SpriteIndex
    {
        get { return spriteIndex; }
        set { spriteIndex = value; }
    }

    private GameObject achievmentRef;

    private List<Achievment> dependencies = new List<Achievment>();

    private string child;

    public string Child
    {
        get { return child; }
        set { child = value; }
    }

    private int currentProgression;
    private int maxProgression;

    public int CurrentProgression
    {
        get { return currentProgression; }
    }

    public int MaxProgression
    {
        get { return maxProgression; }
    }

    public Achievment(string name, string description, int points, int spriteIndex, GameObject achievmentRef, int maxProgression)
    {
        this.name = name;
        this.description = description;
        this.unlocked = false;
        this.points = points;
        this.spriteIndex = spriteIndex;
        this.achievmentRef = achievmentRef;
        this.maxProgression = maxProgression;
        LoadAchievment();
    }

    public void AddDependency(Achievment dependency)
    {
        dependencies.Add(dependency);
    }

    public bool EarnAchievment()
    {
        if (!unlocked && !dependencies.Exists(x => x.unlocked == false) && CheckProgress())
        {
            achievmentRef.GetComponent<UnityEngine.UI.Image>().sprite = AchievementManager.Instance.unlockedSprite;
            SaveAchievment(true);

            if (child != null)
            {
                AchievementManager.Instance.EarnAchievment(child);
            }
            return true;
        }
        return false;
    }

    public void SaveAchievment(bool value)
    {
        unlocked = value;

        int tmpPoints = PlayerPrefs.GetInt("Points");

        PlayerPrefs.SetInt("Points", tmpPoints += points);

        PlayerPrefs.SetInt("Progression" + Name, currentProgression);

        PlayerPrefs.SetInt(name, value ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadAchievment()
    {
        unlocked = PlayerPrefs.GetInt(name) == 1 ? true : false;

        currentProgression = PlayerPrefs.GetInt("Progression" + Name);

        if (unlocked)
        {
            AchievementManager.Instance.textPoints.text = "Points: " + PlayerPrefs.GetInt("Points");
            achievmentRef.GetComponent<UnityEngine.UI.Image>().sprite = AchievementManager.Instance.unlockedSprite;
        }

        UpdateProgressText();
    }

    public bool CheckProgress()
    {
        currentProgression++;

        UpdateProgressText();

        SaveAchievment(false);

        if (maxProgression == 0)
        {
            return true;
        }
        if (currentProgression >= maxProgression)
        {
            return true;
        }

        return false;
    }

    private void UpdateProgressText()
    {
        if (maxProgression > 0)
        {
            achievmentRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Name + " " + currentProgression + "/" + maxProgression;
        }
        else
        {
            achievmentRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Name;
        }
    }
}
