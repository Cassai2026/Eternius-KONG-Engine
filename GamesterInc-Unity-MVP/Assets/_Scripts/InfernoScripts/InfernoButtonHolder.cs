using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfernoButtonHolder : MonoBehaviour
{
    protected InfernoManager manager;
    protected InfernoPlayer player;
    
    [SerializeField] protected Button leaveButton;

    protected virtual void Awake()
    {
        player = FindObjectOfType<InfernoPlayer>();
        manager = FindObjectOfType<InfernoManager>();
        
        
        leaveButton.onClick.AddListener(OnLeaveButtonPress);
    }

    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        var position = player.transform.position;
        Vector2 newPos = new (position.x, position.y + 1.5f);

        transform.position = Camera.main.WorldToScreenPoint(newPos);
    }

    protected virtual void OnDisable()
    {
        InfernoTextBox.instance.gameObject.SetActive(false);
    }

    protected virtual void OnLeaveButtonPress()
    {
        //Make player playable, breaks connection with the NPC and, if required, gives the NPC new tasks
        player.handleFire = false;
        player.ActivatePlayer(true);
        gameObject.SetActive(false);
    }
}
