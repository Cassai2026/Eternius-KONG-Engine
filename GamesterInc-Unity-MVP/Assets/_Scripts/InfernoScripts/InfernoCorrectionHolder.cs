using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InfernoCorrectionHolder : InfernoButtonHolder
{
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite leaveSprite;
    [Header("BUTTONS")] 
    [SerializeField] private Button maskButton;
    [SerializeField] private Button gogglesButton;
    [SerializeField] private Button glovesButton;
    [SerializeField] private Button earProtButton;
    [SerializeField] private Button helpButton;

    private bool[] chosenItems = { false, false,false,false };

    public static bool IsActive = false;
    
    protected override void Awake()
    {
        base.Awake();
        InfernoButtonColorer buttonColorer;
        buttonColorer = maskButton.AddComponent<InfernoButtonColorer>();
        buttonColorer.SetUp(this, 0);
        buttonColorer =earProtButton.AddComponent<InfernoButtonColorer>();
        buttonColorer.SetUp(this, 1);
        buttonColorer =gogglesButton.AddComponent<InfernoButtonColorer>();
        buttonColorer.SetUp(this, 2);
        buttonColorer =glovesButton.AddComponent<InfernoButtonColorer>();
        buttonColorer.SetUp(this, 3);
        // maskButton.onClick.AddListener(delegate { OnItemButtonPress(0); });
        // earProtButton.onClick.AddListener(delegate { OnItemButtonPress(1); });
        // gogglesButton.onClick.AddListener(delegate { OnItemButtonPress(2); });
        // glovesButton.onClick.AddListener(delegate { OnItemButtonPress(3); });
        helpButton.onClick.AddListener(OnHelpButtonPress);
    }

    protected override void Start()
    {
        base.Start();
        IsActive = false;
    }
    
    private void OnEnable()
    {
        IsActive = true;
        chosenItems = new[] { false, false,false,false };
        SetLeaveButtonSprite();
        
        Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var position = player.transform.position;
        Vector2 newPos = new (position.x, position.y + 1.5f);

        transform.position = Camera.main.WorldToScreenPoint(newPos);
         
        if (player.TalkingToNPC != null)
            helpButton.gameObject.SetActive(player.TalkingToNPC.NPCState == InfernoNPC.State.Working);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        IsActive = false;
    }

    private void OnItemButtonPress(int choiceID)
    {
        chosenItems[choiceID] = !chosenItems[choiceID];
        SetLeaveButtonSprite();
    }
    
    private void OnHelpButtonPress()
    {
        player.state = InfernoPlayer.PlayerState.Helping;
        player.TalkingToNPC.Help(true);
        player.TalkingToNPC.TalkedTo = false;
        gameObject.SetActive(false);
    }
    
    protected override void OnLeaveButtonPress()
    {
        //Make player playable, breaks connection with the NPC and, if required, gives the NPC new tasks
        player.TalkingToNPC.TalkedTo = false;
        InfernoNPC.PpeItems list = new (chosenItems);
        if (!list.isEmpty())
            player.TalkingToNPC.AskEquipmentFix(list);
        player.TalkingToNPC = null;
        base.OnLeaveButtonPress();
    }

    /// <summary>
    /// Set to change the visuals of the leave button, showing the player can tell the npc to change their ppe
    /// </summary>
    private void SetLeaveButtonSprite()
    {
        bool hasTrue = false;
        foreach (bool b in chosenItems){
            if (b)
            {
                hasTrue = true;
                break;
            }

        }
        leaveButton.image.sprite = hasTrue ? correctSprite : leaveSprite;
    }

    public class InfernoButtonColorer : MonoBehaviour
    {
        private InfernoCorrectionHolder holder;
        private Image image;
        private int index;
        private bool pressed = false;

        private void OnEnable()
        {
            pressed = false;
            if (image is not null) 
                image.color = Color.white;
        }

        public void SetUp(InfernoCorrectionHolder pHolder, int pIndex)
        {
            holder = pHolder;
            index = pIndex;
            image = GetComponent<Image>();
            GetComponent<Button>().onClick.AddListener(OnButtonPress);
        }

        private void OnButtonPress()
        {
            holder.OnItemButtonPress(index);
            pressed = !pressed;
            image.color = pressed ? Color.gray : Color.white;
        }
    }
}
