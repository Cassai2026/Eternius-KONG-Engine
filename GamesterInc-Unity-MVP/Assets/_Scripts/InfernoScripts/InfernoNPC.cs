using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class InfernoNPC : MonoBehaviour
{
    //Enums:
    public enum State
    {
        Null,
        Walking,
        Working,
        FixingClothing,
        Panicking,
        Extinguishing,
        Dead
    }
    private State state = State.Null;
    private bool breakingRule;
    [NonSerialized] public bool TalkedTo = false;
    [NonSerialized] public bool beingHelped = false;
    
    [SerializeField] private PpeItems currentPpeItems;
    private PpeItems _desiredPpeItems = new ();

    //Remaining variables:
    private InfernoManager manager;
    private System.Random rand = new();
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float workSpeed = 1;
    [SerializeField] private float helpSpeed = 2;
    [SerializeField] private InfernoRegions[] tasks;
    private int taskProgress = 0;
    private Vector2 taskPos = Vector2.zero;
    
    [SerializeField] private Slider WorkSlider;
    [SerializeField] private Image WorkSliderFill;
    private float elpasedToWork = 0;

    [Header("FIRE SETUP")] 
    private InfernoManager.ExtinguisherTypes extinguisherHeld = InfernoManager.ExtinguisherTypes.Null;
    private InfernoManager.ExtinguisherTypes askedExtinguisher = InfernoManager.ExtinguisherTypes.Null;
    
    [Header("SPRITES")]
    [SerializeField]private SpriteRenderer npcRenderer;
    [Space(2)]
    [SerializeField] private Sprite DeathSprite;
    [Space(2)] 
    [SerializeField] private GameObject helmetObject;
    [SerializeField] private GameObject maskObject;
    [SerializeField] private GameObject gogglesObject;
    [SerializeField] private GameObject glovesObject;
    [SerializeField] private GameObject earProtObject;

    [Header("EXTINGUISHER SPRITES")] 
    [SerializeField] private SpriteRenderer extinguisherRenderer;
    [SerializeField] private Sprite waterExtinguisher;
    [SerializeField] private Sprite foamExtinguisher;
    [SerializeField] private Sprite dryExtinguisher;
    [SerializeField] private Sprite co2Extinguisher;
    [SerializeField] private Sprite wetExtinguisher;

    public static System.Action OnDone;
    public static System.Action OnDeath;
    public State NPCState => state;

    /// <summary>
    /// Called when the NPC is instantiated.
    /// </summary>
    /// <param name="pManager"></param>
    /// <param name="pTasks"></param>
    public void SetUp(InfernoManager pManager, InfernoRegions[] pTasks)
    {
        List<InfernoRegions> regions = pTasks.ToList();
        regions = regions.OrderBy(x => rand.Next()).ToList();
        if (rand.Next(0, 100) < 30)
            regions.RemoveAt(regions.Count-1);
        if (rand.Next(0, 100) < 10)
            regions.RemoveAt(regions.Count-1);

        tasks = regions.ToArray();
        
        
        manager = pManager;
        taskProgress = -1;
        SetExtinguisher(InfernoManager.ExtinguisherTypes.Null);
        SetEquipment(currentPpeItems);
        NextTask();
    }
    
    private void OnEnable()
    {
        InfernoFire.OnExtinguished += ExtinguishDone;
    }    
    private void OnDisable()
    {
        InfernoFire.OnExtinguished -= ExtinguishDone;
    }

    private void Update()
    {
        if (TalkedTo) return;
        // Logic for each state, how they handle in the update
        switch (state)
        {
            case(State.Walking): //Move towards the most recent goal
                if (Vector2.Distance(transform.position, taskPos) > 0.2f)
                {
                    Vector2 position = transform.position;
                    Vector2 moveDir = new Vector2(taskPos.x - position.x, taskPos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                }
                else
                {
                    if (taskProgress < 0)
                    {
                        //OnDone?.Invoke();
                        Destroy(gameObject);
                        return;
                    }

                    elpasedToWork = tasks[taskProgress].WorkDuration;
                    ChangeState(State.Working);
                }
                break;
            case(State.Working): //Track progress bar of npc working. Is interactable
                if (tasks[taskProgress].burning)
                {
                    ChangeState(State.Panicking);
                    return;
                }

                if (elpasedToWork <= 0) {
                    Help(false);
                    bool[] status = currentPpeItems.CompareList(tasks[taskProgress].RequiredItems);
                    if (status[0] || status[1])
                    {
                        Die();
                        return;
                    }
                    manager.RemoveNPC(this);
                    OnDone?.Invoke();
                    NextTask();
                    
                } else {
                    elpasedToWork -= Time.deltaTime * (beingHelped ? helpSpeed : workSpeed);
                    WorkSlider.value = (tasks[taskProgress].WorkDuration - elpasedToWork) / tasks[taskProgress].WorkDuration;
                }
                break;
            case(State.FixingClothing): 
                //Move to cabinet
                Vector2 cabinetPos = manager.cabinet.transform.position;
                if (Vector2.Distance(transform.position, cabinetPos) > 0.2f)
                {
                    Vector2 position = transform.position;
                    Vector2 moveDir = new Vector2(cabinetPos.x - position.x, cabinetPos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                    break;
                }
                //Change visuals and ppe state
                SetEquipment(_desiredPpeItems);
                ChangeState(State.Walking);
                break;
            case (State.Panicking): //When interacting with fire, will leave screen if not interrupted
                if (Vector2.Distance(transform.position, manager.transform.position) > 0.2f)
                {
                    Vector2 position = transform.position;
                    var managerPos = manager.transform.position;
                    Vector2 moveDir = new Vector2(managerPos.x - position.x, managerPos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                } else {
                    manager.RemoveNPC(this);
                    Destroy(gameObject);
                    return;
                }
                break;
            case State.Extinguishing:
                if (!manager.FireObject.enabled) //Failsafe if there is no fire
                {
                    ChangeState(State.Walking);
                    return;
                }
                //Move to cabinet
                if (askedExtinguisher != InfernoManager.ExtinguisherTypes.Null)
                {
                    Vector2 extinguisherCabinetPos = manager.cabinet.transform.position;
                    if (Vector2.Distance(transform.position, extinguisherCabinetPos) > 0.2f)
                    {
                        Vector2 position = transform.position;
                        Vector2 moveDir = new Vector2(extinguisherCabinetPos.x - position.x, extinguisherCabinetPos.y - position.y).normalized;
                        transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                        break;
                    }
                    // Equip extinguisher
                    extinguisherHeld = askedExtinguisher;
                    SetExtinguisher(askedExtinguisher);
                    askedExtinguisher = InfernoManager.ExtinguisherTypes.Null;
                    break;
                }
                // Move to fire
                Vector2 firePos = manager.FireObject.transform.position;
                if (Vector2.Distance(transform.position, firePos) > 0.2f)
                {
                    Vector2 position = transform.position;
                    Vector2 moveDir = new Vector2(firePos.x - position.x, firePos.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                } else {
                    //Extinguish fire or die
                    if (!manager.FireObject.effectiveExtinguishers.Contains(extinguisherHeld))
                    {
                        Die();
                        return;
                    }
                    manager.FireObject.ExtinguishFire();
                }
                break;
        }
    }

    /// <summary>
    /// Prepares NPC's variables for next task (move to and work or leave)
    /// </summary>
    private void NextTask()
    {
        _desiredPpeItems = new ();
        ChangeState(State.Walking);
        if (taskProgress >= tasks.Length - 1)
        {
            taskProgress = -2;
            taskPos = manager.transform.position;
            return;
        }

        taskProgress++;
        taskPos = tasks[taskProgress].GetRandomWorkPos();

        if (rand.Next(0, 100) < 50)
        {
            AskEquipmentFix(tasks[taskProgress].RequiredItems);
        }
    }

    public InfernoRegions GetCurrentTask(){
        return tasks[taskProgress];
    }

    //Changes state of npc while working
    public void Help(bool helping = true)
    {
        beingHelped = helping;
        WorkSliderFill.color = helping ? Color.green : Color.white;
    }

    //Called when player wants ppe changes
    public void AskEquipmentFix(PpeItems pPpeToFix)
    {
        if (pPpeToFix.isEmpty()) return;
        extinguisherHeld = askedExtinguisher = InfernoManager.ExtinguisherTypes.Null;
        _desiredPpeItems = pPpeToFix;
        ChangeState(State.FixingClothing);
    }
    
    public void AskExtinguisher(InfernoManager.ExtinguisherTypes type)
    {
        if (type == InfernoManager.ExtinguisherTypes.Null) return;
        askedExtinguisher = type;
        ChangeState(State.Extinguishing);
    }

    /// <summary>
    /// Set sprites of npc to desired sprites
    /// </summary>
    private void SetEquipment(PpeItems toFix)
    {
        maskObject.SetActive(toFix.mask);
        gogglesObject.SetActive(toFix.Goggles);
        glovesObject.SetActive(toFix.Gloves);
        earProtObject.SetActive(toFix.EarProtector);

        currentPpeItems = toFix;
        _desiredPpeItems = new PpeItems();
    }

    /// <summary>
    /// Only set the sprite of the npc's held extinguisher
    /// </summary>
    /// <param name="type">The type of extinguisher the npc should hold</param>
    private void SetExtinguisher(InfernoManager.ExtinguisherTypes type)
    {
        Sprite newSprite = null;
        switch (type)
        {
            case (InfernoManager.ExtinguisherTypes.Water):
                newSprite = waterExtinguisher;
                break;
            case (InfernoManager.ExtinguisherTypes.Foam):
                newSprite = foamExtinguisher;
                break;
            case (InfernoManager.ExtinguisherTypes.DryPowder):
                newSprite = dryExtinguisher;
                break;
            case (InfernoManager.ExtinguisherTypes.CO2):
                newSprite = co2Extinguisher;
                break;
            case (InfernoManager.ExtinguisherTypes.WetChemicals):
                newSprite = wetExtinguisher;
                break;
        }

        extinguisherRenderer.color = (newSprite is null) ? Color.clear : Color.white;
        extinguisherRenderer.sprite = newSprite;
    }

    private void ChangeState(State changeTo)
    {
        WorkSlider.gameObject.SetActive(changeTo == State.Working);
        state = changeTo;
    }

    private void ExtinguishDone()
    {
        if (extinguisherHeld != InfernoManager.ExtinguisherTypes.Null || askedExtinguisher != InfernoManager.ExtinguisherTypes.Null)
        {
            extinguisherHeld = InfernoManager.ExtinguisherTypes.Null;
            SetExtinguisher(InfernoManager.ExtinguisherTypes.Null);
            ChangeState(State.Walking);
        }
    }

    /// <summary>
    /// Remove npc from game and punish player
    /// </summary>
    private void Die()
    {
        ChangeState(State.Null);
        SetEquipment(new PpeItems());
        npcRenderer.sprite = DeathSprite;
        helmetObject.SetActive(false);
        manager.RemoveNPC(this);
        OnDeath?.Invoke();
        GetComponent<BoxCollider2D>().enabled = false;
        enabled = false;
    }

    [Serializable]
    public class PpeItems
    {
        public bool mask = false;
        public bool EarProtector = false;
        public bool Goggles = false;
        public bool Gloves = false;

        public PpeItems(bool pMask = false, bool pEarProtector = false, bool pGoggles = false, bool pGloves = false)
        {
            mask = pMask;
            EarProtector = pEarProtector;
            Goggles = pGoggles;
            Gloves = pGloves;
        }
        
        public PpeItems(bool[] list)
        {
            mask = list[0];
            EarProtector = list[1];
            Goggles = list[2];
            Gloves = list[3];
        }

        /// <summary>
        /// Returns if the entire list is compeltely false or not
        /// </summary>
        public bool isEmpty()
        {
            return !mask && !EarProtector && !Goggles && !Gloves;
        }

        /// <summary>
        /// Converts all booleans into one array. (Ordering is important when checking)
        /// </summary>
        public bool[] GetItemList()
        {
            bool[] list = { mask, EarProtector, Goggles, Gloves};
            return list;
        }

        /// <summary></summary>
        /// <param name="compareTo">list of PPE to compare and see the differences</param>
        /// <returns>2 booleans. [0] implies missing items. [1] implies too many items</returns>
        public bool[] CompareList(PpeItems compareTo)
        {
            bool[] selfList = GetItemList();
            bool[] compareList = compareTo.GetItemList();
            bool isMissing = false;
            bool isTooMuch = false;
            for (int i = 0; i < selfList.Length; i++)
            {
                if (selfList[i] != compareList[i])
                {
                    if (selfList[i])
                        isTooMuch = true;
                    else
                        isMissing = true;
                }
            }
            bool[] list = { isMissing, isTooMuch };
            return list;
        }
    }
}
