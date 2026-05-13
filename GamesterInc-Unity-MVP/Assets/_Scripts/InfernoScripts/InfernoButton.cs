using System;
using UnityEngine;
using UnityEngine.UI;


//THIS CLASS IS OLD AND ONLY USABLE FOR SINGULAR SELECTION OF PPE
public class InfernoButton : MonoBehaviour
{
    [SerializeField] private InfernoNPC.PpeItems ppe;
    [SerializeField] private InfernoManager.ExtinguisherTypes extinguisherType;
    public bool helpButton = false;
    public bool extinguisherButton = false;
    private InfernoPlayer player;
    private GameObject holder;
    private InfernoManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<InfernoManager>();
        player = FindObjectOfType<InfernoPlayer>();
        holder = transform.parent.gameObject;
        GetComponent<Button>().onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        if (helpButton)
        {
            player.state = InfernoPlayer.PlayerState.Helping;
            player.TalkingToNPC.Help(true);
            player.TalkingToNPC.TalkedTo = false;
            holder.gameObject.SetActive(false);
            return;
        }

        if (extinguisherButton)
        {
            Debug.Log("e");
            player.handleFire = false;
            player.ActivatePlayer(true);
            InfernoNPC npc = manager.GetActiveNPC();
            npc.AskExtinguisher(extinguisherType);
            holder.gameObject.SetActive(false);
            return;
        }

        player.ActivatePlayer(true);
        player.TalkingToNPC.TalkedTo = false;
        player.TalkingToNPC.AskEquipmentFix(ppe);
        player.TalkingToNPC = null;
        holder.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            player.ActivatePlayer(true);
            player.TalkingToNPC.TalkedTo = false;
            player.TalkingToNPC.AskEquipmentFix(ppe);
            player.TalkingToNPC = null;
            holder.gameObject.SetActive(false);
        }
    }
}
