using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfernoFire : MonoBehaviour
{
    public enum FireState
    {
        Null,
        Burning,
        Extinguishing
    }
    public FireState fireState;
    public InfernoManager.ExtinguisherTypes[] effectiveExtinguishers;

    private InfernoManager manager;
    private InfernoRegions burningRegion;
    
    [SerializeField] private float burnSpeed;
    [SerializeField] private float extinguishSpeed;
    private float fireProgress = 0.0f;
    
    [SerializeField] private Slider progressSlider;

    public static System.Action OnExtinguished;
    public InfernoRegions BurningRegion => burningRegion;

    private void Awake()
    {
        manager = FindObjectOfType<InfernoManager>();
    }

    private void Update()
    {
        switch (fireState)
        {
            case (FireState.Null):
                return;
            case (FireState.Burning):
                fireProgress += burnSpeed * Time.deltaTime;
                if (fireProgress >= 100)
                {
                    manager.SetEndScreen(false);
                }
                break;
        }
        progressSlider.value = fireProgress;
    }

    public void SetBurn(InfernoRegions region)
    {
        burningRegion = region;
        region.burning = true;
        effectiveExtinguishers = region.EffectiveExtinguishers;
        fireProgress = 0.0f;
        fireState = FireState.Burning;
        transform.position = region.GetRandomWorkPos();
        float x = Mathf.Clamp(transform.position.x, -8, 8);
        float y = Mathf.Clamp(transform.position.y, -3, 4);
        transform.position = new Vector3(x, y, 5f);
        
        gameObject.SetActive(true);
    }

    public void ExtinguishFire()
    {
        fireProgress -= extinguishSpeed * Time.deltaTime;
        if (fireProgress <= 0.0f)
        {
            OnExtinguished?.Invoke();
            fireState = FireState.Null;
            fireProgress = 0;
            gameObject.SetActive(false);
        }
    }
}
