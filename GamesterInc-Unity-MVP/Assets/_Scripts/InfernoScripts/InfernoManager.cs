using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class InfernoManager : MonoBehaviour
{
    public enum ExtinguisherTypes
    {
        Null,
        Water,
        Foam,
        DryPowder,
        CO2,
        WetChemicals
    }

    [SerializeField] private int PeopleToHelp;
    private int peopleHelped = 0;
    private int strikes = 0;
    
    [Header("NPC SETUP")]
    [SerializeField] private InfernoRegions[] tasks;
    public GameObject cabinet;
    [SerializeField] private InfernoNPC NPCPrefab;
    [SerializeField] private int maxNPCAmount = 3;
    [SerializeField] private float NPCSpawnRate = 5f;
    private float elapsedSpawnRate = 0;
    private List<InfernoNPC> npcObjs = new ();

    [Space(5)] 
    [Header("FIRE SETUP")] 
    [SerializeField] private InfernoFire fireObject;
    [SerializeField] private float fireSpawnTimer;
    private float fireSpawnElapsed = 0.0f;
    private bool fireActive = false;

    [Space(5)]
    [Header("SETUP")] 
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite strikeSprite;
    [SerializeField] private Image[] strikeRenderer;
    [SerializeField] private Image[] completedRenderer;
    [SerializeField] private TMP_Text endGameText;
    private Button exitButton;
    private GameObject exitPanel;
    
    public InfernoFire FireObject => fireObject;

    private System.Random random = new();
    

    private void Awake()
    {
        fireObject.gameObject.SetActive(false);
        endGameText.transform.parent.gameObject.SetActive(false);
        exitButton = endGameText.GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(OnReturnPress);
        foreach (InfernoRegions region in tasks)
        {
            region.Manager = this;
        }
    }

    private void Start()
    {
        SetStrikes(strikes = 0);
        SetCompleted(peopleHelped = 0);
    }

    private void OnEnable()
    {
        InfernoNPC.OnDone += AddDone;
        InfernoNPC.OnDeath += AddStrike;
        InfernoFire.OnExtinguished += FireDone;
    }

    private void OnDisable()
    {
        InfernoNPC.OnDone -= AddDone;
        InfernoNPC.OnDeath -= AddStrike;
        InfernoFire.OnExtinguished -= FireDone;
    }
    
    private void Update()
    {
        HandleFiresSpawning();
        HandleNPCSPawning();
    }

    private void HandleNPCSPawning()
    {
        //Countdown till next npc spawn (unless there already are enough on screen)
        if (npcObjs.Count >= maxNPCAmount) return;
        if (elapsedSpawnRate <= 0)
        {
            elapsedSpawnRate = NPCSpawnRate;
            
            InfernoNPC newNPC = Instantiate(NPCPrefab, transform.position, Quaternion.identity);
            newNPC.SetUp(this, tasks);
            npcObjs.Add(newNPC);
        }
        else
        {
            elapsedSpawnRate -= Time.deltaTime;
        }
    }

    private void HandleFiresSpawning()
    {
        //Countdown till fire spawns (if not already active)
        if (fireActive) return;
        if (fireSpawnElapsed >= fireSpawnTimer)
        {
            fireActive = true;
            fireSpawnElapsed = 0.0f;
            fireObject.SetBurn(tasks[random.Next(0, tasks.Length - 1)]);
            return;
        }

        fireSpawnElapsed += Time.deltaTime;
    }
    
    private void FireDone()
    {
        fireActive = false;
    }

    /// <summary>
    /// Checks when a task is done, game ends if enough tasks are done.
    /// </summary>
    private void AddDone()
    {
        peopleHelped++;
        SetCompleted(peopleHelped);
        if (peopleHelped < PeopleToHelp) return;
        SetEndScreen(true);
    }

    public InfernoNPC GetActiveNPC()
    {
        return npcObjs[0];
    }

    public void RemoveNPC(InfernoNPC npc)
    {
        npcObjs.Remove(npc);
    }

    private void SetCompleted(int completed)
    {
        for (int i = 0; i < completedRenderer.Length; i++)
        {
            completedRenderer[i].sprite = i >= completed ? emptySprite : completedSprite;
        }
    }

    private void AddStrike()
    {
        strikes++;
        SetStrikes(strikes);
        if (strikes >= strikeRenderer.Length)
        {
            SetEndScreen(false);
        }
    }

    private void SetStrikes(int strikes)
    {
        for (int i = 0; i < strikeRenderer.Length; i++)
        {
            strikeRenderer[i].sprite = i >= strikes ? emptySprite : strikeSprite;
        }
    }

    //Stops game and shows rather player has won or lost.
    public void SetEndScreen(bool gameWon)
    {
        Time.timeScale = 0;
        endGameText.transform.parent.gameObject.SetActive(true);
        endGameText.text = gameWon ? "YOU WON" : "YOU LOST";
    }

    private void OnReturnPress()
    {
        if (AchievementManager.Instance is not null && strikes < 3)
        {
            AchievementManager.Instance.EarnAchievment("Inferno Minigame");
            AchievementManager.Instance.EarnAchievment("Master Inferno");
        }
        SceneManager.LoadScene("OverWorld");
    }
}
