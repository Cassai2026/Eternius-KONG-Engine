using UnityEngine;
using UnityEngine.UI;

public class SliderInitializer : MonoBehaviour
{
    public Slider slider; // Reference to the Slider component in Unity Editor
    [Range(0f, 1f)]
    public float initialValue = 0.5f; // Initial value of the slider (between 0 and 1)

    void Start()
    {
        // Check if the slider reference is not null
        if (slider != null)
        {
            // Set the value of the slider to the initial value
            slider.value = initialValue;
        }
        else
        {
            Debug.LogWarning("Slider reference is not set in SliderInitializer script!");
        }
    }
}