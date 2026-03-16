using UnityEngine;

public class ScreenModeToggle : MonoBehaviour
{
    public void SetFullscreen()
    {
        Screen.fullScreen = true;
    }

    public void SetWindowed()
    {
        Screen.fullScreen = false;
    }
}
