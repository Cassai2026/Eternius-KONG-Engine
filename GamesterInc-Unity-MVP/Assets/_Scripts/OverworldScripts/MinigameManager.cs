using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    //CALL THIS WHEN MINIGAME ENDS
    //MinigameManager.instance.dungeonSceneDone = true;
    //MinigameManager.instance.correctionSceneDone = true;

    public static MinigameManager instance;

    public int dungeonSceneDoneInt = 0;
    private int correctionSceneDoneInt = 0;

    public int dungeonSceneGaveMoney = 0;
    public int correctionSceneGaveMoney = 0;

    public bool dungeonSceneDone;
    private bool correctionSceneDone;

    private bool dungeonSceneActionExecuted = false;
    private bool correctionSceneActionExecuted = false;

    public int pointsToAddDungeon = 10;
    public int pointsToAddCorrection = 10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load the saved values from PlayerPrefs when the game starts
        LoadDungeonSceneDone();
        LoadCorrectionSceneDone();

        // Execute actions if the scenes are already marked as done
        //if (dungeonSceneDone && !dungeonSceneActionExecuted)
        //{
        //    ExecuteDungeonSceneAction();
        //}

        //if (correctionSceneDone && !correctionSceneActionExecuted)
        //{
        //    ExecuteCorrectionSceneAction();
        //}
    }

    private void Update()
    {
        // Check if the scenes are completed and the associated actions are not executed yet
        if (dungeonSceneDone && !dungeonSceneActionExecuted)
        {
            ExecuteDungeonSceneAction();
            dungeonSceneActionExecuted = true;
        }

        if (correctionSceneDone && !correctionSceneActionExecuted)
        {
            ExecuteCorrectionSceneAction();
            correctionSceneActionExecuted = true;
        }
    }

    private void ExecuteDungeonSceneAction()
    {
        // Implement the action that should happen when dungeonSceneDone becomes true
        Debug.Log("Dungeon scene completed! :DDDD");
        //CoinCounter.instance.IncreaseCoins(pointsToAddDungeon);

        dungeonSceneGaveMoney = 1;
        PlayerPrefs.SetInt("DungeonGaveMoney", dungeonSceneGaveMoney);
        PlayerPrefs.Save();
        // Example action: Load a new scene, trigger an event, etc.
    }

    private void ExecuteCorrectionSceneAction()
    {
        // Implement the action that should happen when correctionSceneDone becomes true
        Debug.Log("Correction scene completed!");
        CoinCounter.instance.IncreaseCoins(pointsToAddCorrection);
        // Example action: Load a new scene, trigger an event, etc.
    }

    private void SaveDungeonSceneDone()
    {
        PlayerPrefs.SetInt("DungeonSceneDone", dungeonSceneDoneInt);
        PlayerPrefs.Save();
    }


    private void SaveCorrectionSceneDone()
    {
        PlayerPrefs.SetInt("CorrectionSceneDone", correctionSceneDoneInt);
        PlayerPrefs.Save();
    }

    private void LoadDungeonSceneDone()
    {
        dungeonSceneDoneInt = PlayerPrefs.GetInt("DungeonSceneDone", 0);
        dungeonSceneDone = dungeonSceneDoneInt == 1;
        dungeonSceneGaveMoney = PlayerPrefs.GetInt("DungeonGaveMoney", 0);
        dungeonSceneActionExecuted = dungeonSceneGaveMoney == 1;
    }

    private void LoadCorrectionSceneDone()
    {
        correctionSceneDoneInt = PlayerPrefs.GetInt("CorrectionSceneDone", 0);
        correctionSceneDone = correctionSceneDoneInt == 1;
    }

    public void SetDungeonSceneDone(bool value)
    {
        dungeonSceneDone = value;
        dungeonSceneDoneInt = value ? 1 : 0; // Convert boolean to integer
        SaveDungeonSceneDone();
    }

    public void SetCorrectionSceneDone(bool value)
    {
        correctionSceneDone = value;
        correctionSceneDoneInt = value ? 1 : 0; // Convert boolean to integer
        SaveCorrectionSceneDone();
    }
}
