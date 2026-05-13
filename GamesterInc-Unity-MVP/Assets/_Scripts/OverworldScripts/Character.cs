using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Character      // References to the character database points that are needed
{
    public string characterName;
    public Sprite characterSprite;
    public int price;
    public bool locked;
    public bool equipped;
}