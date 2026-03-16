using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BrightnessController : MonoBehaviour
{
    public static BrightnessController instance;
    private Image brightnessOverlay;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FindBrightnessOverlay();
    
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        SetBrightness(savedBrightness);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindBrightnessOverlay();
        SetBrightness(PlayerPrefs.GetFloat("Brightness", 0.5f)); 
    }

    void FindBrightnessOverlay()
    {
     
      GameObject overlayObject = GameObject.Find("BrightnessOverlay");

    if (overlayObject != null)
    {
        brightnessOverlay = overlayObject.GetComponent<Image>();
    }
    else
    {
        Debug.LogWarning("BrightnessOverlay not found in the scene!");
    }

    }

    public void SetBrightness(float value)
    {
       
        if (brightnessOverlay == null)
        {
            FindBrightnessOverlay();
        }

        if (brightnessOverlay != null)
        {
            Color color = brightnessOverlay.color;
            color.a = 1 - value; 
            brightnessOverlay.color = color;
        }

        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }
}
