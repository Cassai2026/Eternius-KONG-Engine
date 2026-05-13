using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{

    [SerializeField] private Sprite tutorialSprite;
    [SerializeField] private Image tutorialRenderer;
    [SerializeField] private bool showOnStart;
    
    private Button tutorialButton;
    private bool tutorialActive = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        tutorialButton = GetComponent<Button>();
        tutorialButton.onClick.AddListener(OnTutorialPress);
        
        tutorialRenderer.sprite = tutorialSprite;
        tutorialRenderer.color = Color.white;
    }

    private void Start()
    {
        if (showOnStart)
        {
            Invoke("OnTutorialPress", 0.1f);
        } else tutorialRenderer.enabled = false;
    }

    private void OnTutorialPress()
    {
        tutorialActive = !tutorialActive;
        
        tutorialRenderer.enabled = tutorialActive;
        Time.timeScale = tutorialActive ? 0.0f : 1.0f;

    }
}
