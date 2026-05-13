using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfernoTextBox : MonoBehaviour
{
    public static InfernoTextBox instance;
    private InfernoPlayer player;

    private TMP_Text textObj;
    private void Awake()
    {
        if (instance is not null)
        {
            Debug.LogWarning("MORE THEN 1 TEXTBOX???");
            Destroy(this);
        }

        instance = this;
        player = FindObjectOfType<InfernoPlayer>();
        textObj = GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void SetText(string text)
    {
        var position = player.transform.position;
        Vector2 newPos = new (position.x, position.y - 2f);
        transform.position = Camera.main.WorldToScreenPoint(newPos);
        
        textObj.text = text;
        
        gameObject.SetActive(true);
    }
}
