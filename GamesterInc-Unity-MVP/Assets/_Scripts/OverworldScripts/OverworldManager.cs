using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager instance;

    [SerializeField] private Button questAccept;
    public QuestGiverScript questGiver;
    
    private bool dungeonDone, correctionDone = false;
    private void Awake()
    {
        if (instance is not null) Destroy(gameObject);
        instance = this;
        //DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        questAccept.onClick.AddListener(AcceptQuestPress);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (dungeonDone && correctionDone && scene.name == "OverWorld")
        {
            SceneManager.LoadScene("FrameDataScene");
        }
        switch (scene.name)
        {
            case ("DungeonScene"):
                dungeonDone = true;
                break;
            case ("Minigame"):
                correctionDone = true;
                break;
        }
    }

    private void AcceptQuestPress()
    {
        Debug.Log($"Questgiver {questGiver}");
        if (questGiver is not null)
        {
            questGiver.AcceptQuest();
        }
    }
}