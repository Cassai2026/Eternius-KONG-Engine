using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InfernoPlayer : MonoBehaviour
{
    
    public enum PlayerState
    {
        Talking,
        Walking,
        Helping
    }

    [NonSerialized] public PlayerState state = PlayerState.Walking;

    [SerializeField] private InfernoCorrectionHolder correctionHolder;
    [SerializeField] private InfernoButtonHolder extinguisherHolder;
    private PlayerMovement movement;
    private InfernoNPC talkingToNPC = null;
    private InfernoFire fireObj;
    public bool handleFire = false;

    public InfernoNPC TalkingToNPC
    {
        get { return talkingToNPC; }
        set { talkingToNPC = value; }
    }

    private void Awake()
    {
        fireObj = FindObjectOfType<InfernoFire>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        switch (state)
        {
            case(PlayerState.Helping):
                if (Input.GetMouseButtonDown(0))
                {
                    state = PlayerState.Walking;
                    TalkingToNPC.Help(false);
                }


                if (talkingToNPC is not null && talkingToNPC.beingHelped) return;
                ActivatePlayer(true);
                TalkingToNPC = null;
                break;
            case (PlayerState.Talking):
                break;
            case(PlayerState.Walking):
                HandleClick();
                CheckTalkingToNPC();
                CheckHandlingFire();
                break; 
        }
    }
    
    /// <summary>
    /// Check if player clicked on NPC
    /// </summary>
    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        talkingToNPC = null;
        handleFire = false;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Collider2D collider = Physics2D.OverlapPoint(mousePos);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        if (colliders.Length == 0) return;
        foreach (Collider2D col in colliders) {
            if (col.gameObject.CompareTag("NPC"))
            {
                talkingToNPC = col.GetComponent<InfernoNPC>();
            }
            else if (col.gameObject.CompareTag("Fire"))
            {
                handleFire = true;
            }
        }
    }
    
    private void CheckTalkingToNPC()
    {
        if (talkingToNPC is null) return;
        //Will go to talking state if player is close enough to npc
        if (Vector2.Distance(talkingToNPC.transform.position, transform.position) < 1.3f)
        {
            correctionHolder.gameObject.SetActive(true);
            InfernoTextBox.instance.SetText(talkingToNPC.GetCurrentTask().TaskDescription);
            ActivatePlayer(false, true);
            talkingToNPC.TalkedTo = true;
        }
    }

    private void CheckHandlingFire()
    {
        if (!handleFire) return;
        if (Vector2.Distance(fireObj.transform.position, transform.position) > 2f) return;
        extinguisherHolder.gameObject.SetActive(true);
        InfernoTextBox.instance.SetText(fireObj.BurningRegion.FireDescription);
        ActivatePlayer(false, true);
    }

    /// <summary>
    /// Sets the player's ability to move
    /// </summary>
    /// <param name="pCanMove">bool to determine player's input</param>
    /// <param name="resetWalk">true if player's point-and-click walk should also stop</param>
    public void ActivatePlayer(bool pCanMove, bool resetWalk = false)
    {
        state = pCanMove ? PlayerState.Walking : PlayerState.Talking;
        movement.canMove = pCanMove;
        if (resetWalk)
            movement.moveToPos = transform.position;
    }
}
