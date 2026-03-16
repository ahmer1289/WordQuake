using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnTimer : MonoBehaviour
{
    public float turnDuration = 10f;
    public float timeRemaining;
    private bool isTimerRunning = false;

    public GameObject m_TimerObject;
    public Image timerImage;
    public Image timerExtra;
    public TextMeshProUGUI timerText;

    public delegate void TurnEndHandler();
    public static event TurnEndHandler OnTurnEnd;

    public static TurnTimer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetTurnDurationFromLobby();
        StartTimer();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                EndTurn();
            }
        }
    }

    public void StartTimer()
    {   
        
        timeRemaining = turnDuration;
        isTimerRunning = true;
        UpdateTimerUI();
        SetTimerObjectVisibility(true);
    }

    public void PauseTimer(){

        SetTimerObjectVisibility(false);
        isTimerRunning = false;
    }

    public void ContinueTimer(){

        SetTimerObjectVisibility(true);
        isTimerRunning = true;
    }

    public void ResetTimer()
    {
        StartTimer();
    }

    void EndTurn()
    {
        isTimerRunning = false;
        if (OnTurnEnd != null)
        {
            OnTurnEnd.Invoke();
        }
        else
        {
            Debug.LogWarning("No subscribers for OnTurnEnd event.");
        }
    }

    void UpdateTimerUI()
    {
        if (timerImage != null)
        {
            timerImage.fillAmount = timeRemaining / turnDuration;
        }

        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timeRemaining).ToString();
        }
        if (timerExtra != null)
        {
            timerExtra.enabled = timeRemaining > 10f;
        }
    }

    public void ExtendTime(float extraTime)
    {
        timeRemaining += extraTime;
        UpdateTimerUI();
    }

    /// <summary>
    ///  This method controls the visibility of the timer object.
    /// </summary>
    void SetTimerObjectVisibility(bool isVisible){
        
        m_TimerObject.SetActive(isVisible);
        Debug.Log($"Messed with Timer visibility: " + isVisible);
    }
    
    public void SetTurnDurationFromLobby()
    {
        turnDuration = PlayerPrefs.GetFloat("GAME_TIMER", turnDuration);
    }

}
