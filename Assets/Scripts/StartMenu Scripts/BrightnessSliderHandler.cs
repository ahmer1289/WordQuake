using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrightnessSliderHandler : MonoBehaviour
{
    public Slider brightnessSlider;
    public TMP_Text brightnessText;

    void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        brightnessSlider.value = savedBrightness;
        brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        UpdateBrightnessText(savedBrightness);
    }

    public void ChangeBrightness(float value)
    {
        if (BrightnessController.instance != null)
        {
            BrightnessController.instance.SetBrightness(value);
        }
        UpdateBrightnessText(value);
    }

    void UpdateBrightnessText(float value)
    {
        int brightnessPercentage = Mathf.RoundToInt(value * 100);
        brightnessText.text = brightnessPercentage + "%";
    }
}
