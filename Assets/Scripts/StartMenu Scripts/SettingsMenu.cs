using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Button musicButton;
    public TMP_Text buttonText;
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public CanvasGroup sliderCanvasGroup;


    void Start()
    {
        musicButton.onClick.AddListener(ToggleMusic);
        UpdateButtonText();

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        volumeSlider.value = savedVolume * 10;

       
        int volumePercentage = Mathf.RoundToInt(volumeSlider.value * 10);
        volumeText.text = volumePercentage + "%";

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("StartMenu");
    }

    void ToggleMusic()
    {
        MusicPlayer.Instance.ToggleMusic();
        //AudioManager.instance.ToggleMusic();
        UpdateButtonText();
        UpdateSliderState(); 
    }

    void UpdateButtonText()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicState", 1) == 1;
        if(isMusicOn) 
        {
            buttonText.text = "Music: ON";
        }
        else
        {
            buttonText.text = "Music: OFF";
        }
        PlayerPrefs.Save();
    }

    void UpdateSliderState()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicState", 1) == 1;
        volumeSlider.interactable = isMusicOn;
        if (isMusicOn)
        {
            sliderCanvasGroup.alpha = 1f;
        }
        else
        {
            sliderCanvasGroup.alpha = 0.5f;
        }
     }
    
    void ChangeVolume(float value)
    {
        float newVolume = value / 10; 
       // AudioManager.instance.SetVolume(newVolume);
		MusicPlayer.Instance.SetVolume(newVolume);

		int volumePercentage = Mathf.RoundToInt(volumeSlider.value * 10);
        volumeText.text = volumePercentage + "%";

        PlayerPrefs.SetFloat("MusicVolume", newVolume);
        PlayerPrefs.Save();
    }

}
