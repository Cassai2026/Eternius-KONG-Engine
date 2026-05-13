using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfernoRegions : MonoBehaviour
{
    private InfernoManager manager;
    public InfernoManager Manager
    {
        get { return manager; }
        set { manager = value; }
    }

    [SerializeField] private float workRadius;
    [SerializeField] private float workDuration;
    [SerializeField] private InfernoNPC.PpeItems requiredItems;
    [SerializeField] [TextArea] private string taskDescription;
    [SerializeField] [TextArea] private string fireDescription;
    
    [Space(5)]
    [SerializeField] private InfernoManager.ExtinguisherTypes[] effectiveExtinguishers;

    public InfernoManager.ExtinguisherTypes[] EffectiveExtinguishers => effectiveExtinguishers;
    public string TaskDescription => taskDescription;
    public string FireDescription => fireDescription;


    public InfernoNPC.PpeItems RequiredItems => requiredItems;
    private System.Random rand = new ();
    public float WorkDuration => workDuration;
    [NonSerialized] public bool burning = false;

    private void OnEnable()
    {
        InfernoFire.OnExtinguished += FireExtinguised;
    }
    private void OnDisable()
    {
        InfernoFire.OnExtinguished -= FireExtinguised;
    }

    private void FireExtinguised()
    {
        burning = false;
    }

    public Vector2 GetRandomWorkPos()
    {
        int angle = rand.Next(360);

        float xPos = transform.position.x + workRadius * Mathf.Cos(angle);
        float yPos = transform.position.y + workRadius * Mathf.Sin(angle);

        return new Vector2(xPos, yPos);
    }
}
