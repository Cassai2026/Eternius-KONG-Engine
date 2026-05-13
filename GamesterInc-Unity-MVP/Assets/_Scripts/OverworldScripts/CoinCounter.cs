using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter instance;

    public TMP_Text CoinText;
    public int CurrentCoins = 0;

    private string coinsSaveKey = "PlayerCoins";

    void Awake()
    {
        instance = this;
    }

    // Saves coin amount
    void Start()
    {
        if (PlayerPrefs.HasKey(coinsSaveKey))
        {
            CurrentCoins = PlayerPrefs.GetInt(coinsSaveKey);
        }

        UpdateUI();
    }

    // Updates coin text based on CurrentCoins
    void UpdateUI()
    {
        CoinText.text = CurrentCoins.ToString();
    }

    // Increases coins when getting coins from quest or coins in game
    public void IncreaseCoins(int amount)
    {
        Debug.Log("Attempting to increase coins by: " + amount);
        CurrentCoins += amount;
        Debug.Log("New coin count after increase: " + CurrentCoins);
        PlayerPrefs.SetInt(coinsSaveKey, CurrentCoins);
        PlayerPrefs.Save();
        UpdateUI();
    }

    // Decreases coins when item is bought
    public void DecreaseCoins(int amount)
    {
        CurrentCoins -= amount;
        PlayerPrefs.SetInt(coinsSaveKey, CurrentCoins);
        PlayerPrefs.Save();
        UpdateUI();
    }
}