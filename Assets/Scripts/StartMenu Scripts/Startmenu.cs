using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator backgroundAnimator; // Assign the background animator in the Inspector
    public float animationDuration = 0.1f; // Adjust based on animation length
    public GameObject sB;
    public GameObject SB;
    public GameObject eB;
    public void StartGame()
    {

        StartCoroutine(PlayAnimationThenLoadScene("WQ_Lobby"));
    }
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
 
    public void ExitGame()
    {
        Application.Quit();
    }
    IEnumerator PlayAnimationThenLoadScene(string sceneName)
    {
        if (backgroundAnimator != null)
        {
            sB.SetActive(false);
            SB.SetActive(false);
            eB.SetActive(false);
            backgroundAnimator.enabled = true;
            yield return new WaitForSeconds(animationDuration); 
        }

        SceneManager.LoadScene(sceneName);
    }
}
