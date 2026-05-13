using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    public GameObject achievmentPrefab;
    public Sprite[] sprites;
    private AchievementButton activeButton;
    public ScrollRect scrollRect;
    public GameObject achievmentMenu;
    public GameObject visualAchievment;
    public Dictionary<string, Achievment> achievments = new Dictionary<string, Achievment>();
    public Sprite unlockedSprite;
    public TextMeshProUGUI textPoints;
    private static AchievementManager instance;
    public int fadeTime = 2;

    // Reference to CoinCounter
    public CoinCounter coinCounter;

    public static AchievementManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AchievementManager>();
            }
            return AchievementManager.instance;
        }
    }

    void Start()
    {
        // To test REMEMBER TO REMOVE
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.DeleteKey("Points");

        achievmentMenu.transform.parent.gameObject.SetActive(true);
        
        activeButton = GameObject.Find("GeneralButton").GetComponent<AchievementButton>();
        CreateAchievment("GeneralCategory", "Learning Movement", "Press on the screen to move your character around", 5, 0, 0);
        CreateAchievment("GeneralCategory", "Open Settings Menu", "Press on the setting icon to access the configuration menu", 5, 4, 0);
        CreateAchievment("GeneralCategory", "Open Progress Menu", "Press on the progress icon to access the achievement menu", 5, 4, 0);
        CreateAchievment("GeneralCategory", "Open Character Menu", "Visit 3 times the shop to customize your character", 5, 6, 3);
        CreateAchievment("GeneralCategory", "Collect Coins", "Collect 25 coins", 5, 2, 25);
        CreateAchievment("GeneralCategory", "Introduction", "Complete all missions in this Unit", 10, 1, 0, new string[] { "Learning Movement", "Open Settings Menu", "Open Progress Menu", "Open Character Menu" });

        CreateAchievment("Unit1Category", "Dungeon", "Find the path and talk with the Dungeon NPC", 5, 3, 0);
        CreateAchievment("Unit1Category", "Correction", "Find the path and talk with the Correction NPC", 5, 3, 0);
        CreateAchievment("Unit1Category", "Inferno", "Find the path and talk with the Inferno NPC", 5, 3, 0);
        CreateAchievment("Unit1Category", "Dungeon Minigame", "Complete the Dungeon Minigame", 15, 9, 0);
        CreateAchievment("Unit1Category", "Correction Minigame", "Complete the Correction Minigame", 15, 7, 0);
        CreateAchievment("Unit1Category", "Inferno Minigame", "Complete the Inferno Minigame", 15, 7, 0);
        CreateAchievment("Unit1Category", "Master Dungeon", "Complete the Dungeon Minigame 3 times", 15, 10, 3);
        CreateAchievment("Unit1Category", "Master Correction", "Complete the Correction Minigame 3 times", 15, 8, 3);
        CreateAchievment("Unit1Category", "Master Inferno", "Complete the inferno Minigame 3 times", 15, 8, 3);

        foreach (GameObject achievmentList in GameObject.FindGameObjectsWithTag("AchievmentList"))
        {
            achievmentList.SetActive(false);
        }

        activeButton.Click();
        achievmentMenu.SetActive(false);

        // Find the CoinCounter instance
        coinCounter = CoinCounter.instance;
        
        // achievmentMenu.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
            achievmentMenu.SetActive(!achievmentMenu.activeSelf);
        }

        if (Input.GetMouseButtonDown(0))
        {
            EarnAchievment("Learning Movement");
        }
    }

    public void EarnAchievment(string title)
    {

        if (achievments.ContainsKey(title) && achievments[title].EarnAchievment())
        {
            GameObject achievment = (GameObject)Instantiate(visualAchievment);
            SetAchievmentInfo("EarnCanvas", achievment, title, achievments[title].CurrentProgression, achievments[title].MaxProgression);

            // Add points to the coin count
            int points = achievments[title].Points;
            if (coinCounter is not null)
                coinCounter.IncreaseCoins(points);
            else
                Debug.LogWarning("NO COINCOUNTER FOUND, NO COINS AWARDED");

            textPoints.text = "Points: " + PlayerPrefs.GetInt("Points");
            StartCoroutine(FadeAchievment(achievment));
        }
    }

    public IEnumerator HideAchievment(GameObject achievment)
    {
        yield return new WaitForSeconds(3);
        Destroy(achievment);
    }

    public void CreateAchievment(string parent, string title, string description, int points, int spriteIndex, int progress, string[] dependencies = null)
    {
        GameObject achievment = (GameObject)Instantiate(achievmentPrefab);

        Achievment newAchievment = new Achievment(title, description, points, spriteIndex, achievment, progress);

        achievments.Add(title, newAchievment);

        SetAchievmentInfo(parent, achievment, title, newAchievment.CurrentProgression, progress);

        if (dependencies != null)
        {
            foreach (string achievmentTitle in dependencies)
            {
                Achievment dependency = achievments[achievmentTitle];
                dependency.Child = title;
                newAchievment.AddDependency(dependency);
            }
        }
    }

    public void SetAchievmentInfo(string parent, GameObject achievment, string title, int currentProgression = 0, int maxProgression = 0)
    {
        achievment.transform.SetParent(GameObject.Find(parent).transform);
        achievment.transform.localScale = Vector3.one;

        string progressText = string.Empty;
        if (maxProgression > 0)
        {
            progressText = $" {currentProgression}/{maxProgression}";
        }

        achievment.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title + progressText;
        achievment.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = achievments[title].Description;
        achievment.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = achievments[title].Points.ToString();
        achievment.transform.GetChild(3).GetComponent<Image>().sprite = sprites[achievments[title].SpriteIndex];
    }

    public void ChangeCategory(GameObject button)
    {
        AchievementButton achievementButton = button.GetComponent<AchievementButton>();

        scrollRect.content = achievementButton.achievmentList.GetComponent<RectTransform>();

        achievementButton.Click();
        activeButton.Click();
        activeButton = achievementButton;
    }

    private IEnumerator FadeAchievment(GameObject achievment)
    {
        CanvasGroup canvasGroup = achievment.GetComponent<CanvasGroup>();

        float rate = 1.0f / fadeTime;

        int startAlpha = 0;
        int endAlpha = 1;

        for (int i = 0; i < 2; i++)
        {
            float progress = 0.0f;
            while (progress < 1.0)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
                progress += rate * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            startAlpha = 1;
            endAlpha = 0;
        }

        Destroy(achievment);
    }

    public void OpenAchievment()
    {
        achievmentMenu.SetActive(!achievmentMenu.activeSelf);

        if (achievmentMenu.activeSelf)
        {
            PauseMenuScript pauseMenu = FindObjectOfType<PauseMenuScript>();
            if (pauseMenu != null)
            {
                pauseMenu.DisablePauseMenu();
            }
        }
        Time.timeScale = achievmentMenu.activeSelf ? 0.0f : 1.0f;
    }
}
