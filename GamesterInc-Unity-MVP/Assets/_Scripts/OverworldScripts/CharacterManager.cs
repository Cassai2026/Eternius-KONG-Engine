using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;       // Scriptable object
    public TMP_Text nameText; 
    public RawImage artworkRawImage;
    public Button buyButton;

    private Character[] characters;
    private int selectedOption = 0;
    private int equippedIndex = -1;

    // If character is selected it gives the key "selectedOption" and loads this character in game ( Does not work with animations yet ! )
    void Start()        
    {
        characters = characterDB.character;
        LoadCharacterStates();
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);
        UpdateBuyButton();
    }

    // Able to scroll through characters and look at the next character by pressing a button
    public void NextOption()      
    {
        selectedOption++;
        if (selectedOption >= characters.Length)
        {
            selectedOption = 0;
        }
        UpdateCharacter(selectedOption);
        UpdateBuyButton();
    }

    // Able to scroll through characters and look at the previous character by pressing a button
    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characters.Length - 1;
        }
        UpdateCharacter(selectedOption);
        UpdateBuyButton();
    }

    // System to buy and equip characters when clicking the button
    public void BuyOrEquipCharacter()
    {
        Character character = characters[selectedOption];
        if (character.locked)
        {
            // Connected to CoinCounter.cs
            if (CoinCounter.instance.CurrentCoins >= character.price)   
            {
                // If character is bought, decreases coins from CoinCounter.cs
                CoinCounter.instance.DecreaseCoins(character.price);    
                character.locked = false;
                SaveCharacterState(selectedOption, true);
                EquipCharacter(selectedOption);
                UpdateCharacter(selectedOption);
                UpdateBuyButton();
            }
            else    
            {
                // If not enough coins to buy character
                Debug.Log("Not enough coins to buy this character.");
            }
        }
        else
        {
            // Unequips other character if you equip another character
            if (character.equipped) 
            {
                UnEquipCharacter();
            }
            else
            {
                UnEquipCharacter();
                EquipCharacter(selectedOption);
            }
            UpdateCharacter(selectedOption);
            UpdateBuyButton();
        }
    }

    // Related to equipping characters
    private void EquipCharacter(int index)
    {
        UnEquipCharacter();

        equippedIndex = index;
        characters[index].equipped = true;

        UpdateCharacter(equippedIndex);

        Save();
    }

    // Related to unequipping characters
    private void UnEquipCharacter()
    {
        if (equippedIndex != -1)
        {
            characters[equippedIndex].equipped = false;
            UpdateCharacter(equippedIndex);
            equippedIndex = -1;
        }
    }

    // Updates character and text when scrolling through characters
    private void UpdateCharacter(int selectedOption)
    {
        Character character = characters[selectedOption];

        if (character != null && character.characterSprite != null)
        {
            artworkRawImage.texture = character.characterSprite.texture;
        }

        if (!string.IsNullOrEmpty(character.characterName))
        {
            nameText.text = character.characterName;
        }

        else
        {
            Debug.LogWarning("Character is null at index: " + selectedOption);
        }
    }

    // Updates the buy button when character is bought to equip button
    private void UpdateBuyButton()
    {
        Character character = characters[selectedOption];

        if (character != null)
        {
            if (character.equipped)
            {
                buyButton.interactable = false;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equipped";
            }
            else if (character.locked)
            {
                buyButton.interactable = true;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy for " + character.price + " coins";
            }
            else
            {
                buyButton.interactable = true;
                buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }
        }
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    // Saves selected character ( Does not work with animations! )
    private void Save()
    {
        PlayerPrefs.SetInt("selectedOption", selectedOption);
    }

    // Loads character in scene ( Does not work with animations! )
    private void LoadCharacterStates()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            bool bought = PlayerPrefs.GetInt("Character_" + i.ToString(), 0) == 1;
            characters[i].locked = !bought;
            characters[i].equipped = false;
        }
    }

    private void SaveCharacterState(int index, bool bought)
    {
        PlayerPrefs.SetInt("Character_" + index.ToString(), bought ? 1 : 0);
    }
}