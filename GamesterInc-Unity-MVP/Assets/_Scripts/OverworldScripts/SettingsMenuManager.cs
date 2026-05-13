using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuManager : MonoBehaviour
{
    private const string qualityKey = "GraphicsQuality";

    void Start()
    {
        // Load the saved quality level or set default
        int savedQuality = PlayerPrefs.GetInt(qualityKey, QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQuality);
    }

    public void SetQuality(int qualityIndex)
    {
        // Set the quality level
        QualitySettings.SetQualityLevel(qualityIndex);
        // Save the selected quality level
        PlayerPrefs.SetInt(qualityKey, qualityIndex);
        PlayerPrefs.Save(); // Save PlayerPrefs immediately
    }

    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}