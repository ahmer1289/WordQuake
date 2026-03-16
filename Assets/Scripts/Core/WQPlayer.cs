using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To make sure all player objets have these very importnt components
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]

public class WQPlayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private BoxCollider2D m_BoxCollider2D;
    [SerializeField] private TextNotifyPopup abilityNotificationPopup;
    [SerializeField] private PlayerLivesHolder playerLivesHolder;
    [Space, SerializeField] string m_PlayerName;
    [Space] public int m_TotalPoints;
    [Space] public int Lives;
    [Space] public int m_TotalWordsEntered;
    [Space] public int m_InvalidWordCount;

    int playerIndex;
    Vector2 newPosition;
    WQSpecialWordReceiver WQSpecialWordReceiver;
    GameManager gameManager;
    ColorSpriteFlash colorSpriteFlash;
    RuntimeAnimatorController defaultAnimController;
    RuntimeAnimatorController glowAnimController;

    private float totalTimeTaken = 0f;
    private int turnsTaken = 0;

    void Start()
    {
        gameManager = GameManager.Instance;
        WQSpecialWordReceiver = GetComponentInChildren<WQSpecialWordReceiver>();
        colorSpriteFlash = GetComponentInChildren<ColorSpriteFlash>();
    }

    public WQSpecialWordReceiver GetWQPlayerSpecialWordReceiver(){

        return WQSpecialWordReceiver;
    }

    public void Initialize(string name, WQCharacter character, int index)
    {
        m_PlayerName = name;
        playerIndex = index;

        defaultAnimController = character.m_Animator;
        glowAnimController = character.m_AnimatorWithGlow;

        m_SpriteRenderer.sprite = character.m_Sprite;
        m_SpriteRenderer.color = Color.white;
        m_Animator.runtimeAnimatorController = defaultAnimController;

        newPosition = transform.position;
        newPosition += character.m_SittingPositionOnCushion;
        transform.position = newPosition;

        if (playerIndex == 2)
        {
            // flip the player
            transform.rotation = Quaternion.Euler(0, -180, 0);
            abilityNotificationPopup.transform.localRotation = Quaternion.Euler(0, -180, 0);
        }
    }

    public void Notify(string abilityName){

        abilityNotificationPopup.Notify(m_PlayerName, abilityName);
    }

    public void ToggleHighlight(){

        m_Animator.runtimeAnimatorController = m_Animator.runtimeAnimatorController == defaultAnimController 
            ? glowAnimController : defaultAnimController;
    }

    public void DisableHighlight(){

        m_Animator.runtimeAnimatorController = defaultAnimController;   
    }

    public void TakeDamage()
    {
        if (Lives > 0)
        {
            Lives--;

            playerLivesHolder.ReduceLife(Lives);

            if(Lives == 0){

                gameManager.GameOver();
            }
        }

        Debug.Log($"{m_PlayerName} lost a life! Remaining: {Lives}");
    }

    //TODO: (tempoary)
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("SpecialWordObject"))
        {
            WQSpecialWordEffect wQSpecialWordObject = other.collider.GetComponent<WQSpecialWordEffect>();

            if(wQSpecialWordObject == null) wQSpecialWordObject = other.transform.parent.GetComponent<WQSpecialWordEffect>();

            if(wQSpecialWordObject.CanDamagePlayer(playerIndex)){

                colorSpriteFlash.Play();
                TakeDamage();
                Destroy(other.gameObject, 0.5f); //destroy the special word
            }
        }
    }

    public void ToggleLoseOverlay(){

        ToggleHighlight();
        colorSpriteFlash.PlayWithoutReset();
    }

    public void EnableLoseOverlay(){
        
        playerLivesHolder.ReduceLife(0);
        colorSpriteFlash.PlayWithoutReset();
    }

    public void AddPoints(int val){
        m_TotalPoints += val;
        Debug.Log($"{m_PlayerName} gained {val} points! Total: {m_TotalPoints}");
    }

    public void SubtractPoints(int val){

        m_TotalPoints -= val;
        Debug.Log($"{m_PlayerName} lost {val} points! Total: {m_TotalPoints}");
    }

    public void DebugScore()
    {
        Debug.Log($"{m_PlayerName} has {m_TotalPoints} points.");
    }

    public string GetPlayerName() => m_PlayerName;
    public int GetPoints() => m_TotalPoints;

    public int GetWordsEntered() => m_TotalWordsEntered;
    public int GetInvalidWordCount() => m_InvalidWordCount;
    public void IncrementWordsEntered()
    {
        m_TotalWordsEntered++;
        Debug.Log($"{m_PlayerName} entered a word. Total words entered: {m_TotalWordsEntered}");
    }
    public void IncrementInvalidWords()
    {
        m_InvalidWordCount++;
        Debug.Log($"{m_PlayerName} entered an invalid word. Total invalid: {m_InvalidWordCount}");
    }

    public void AddTimeTaken(float time)
    {
        Debug.Log($"{m_PlayerName} used {time} seconds this turn.");
        totalTimeTaken += time;
        turnsTaken++;
    }
    public float GetAverageTimePerWord()
    {
        return turnsTaken > 0 ? totalTimeTaken / turnsTaken : 0f;
    }
}


