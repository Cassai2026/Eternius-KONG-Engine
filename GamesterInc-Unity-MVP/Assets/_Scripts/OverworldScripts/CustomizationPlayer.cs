using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationPlayer : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSpriteRenderer; // Reference to the SpriteRenderer component


    private int selectedOption = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }

        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);
        
        // Check if character is null
        if (character != null)
        {
            // Check if characterSprite is not null
            if (character.characterSprite != null)
            {
                // Assign sprite to SpriteRenderer
                artworkSpriteRenderer.sprite = character.characterSprite;
            }
        }
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }
}