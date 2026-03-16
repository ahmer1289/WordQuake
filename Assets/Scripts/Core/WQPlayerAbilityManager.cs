using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WQPlayerAbilityManager : MonoBehaviour
{

    public static WQPlayerAbilityManager Instance;

    #region Important Managers
    private WQThemes themesManager;
    private WQManager wqManager;
    private GameManager gameManager;
    private TurnTimer turnTimer;

    #endregion

    const string m_SkipTurn = "Skip Turn";
    const string m_ExtendTimer = "Extend Timer";
    const string m_SuggestValidWord = "Suggest Valid Word";
    const string m_ShuffleTheme = "Shuffle Theme";
    public string[] AllAbilities = {m_SkipTurn, m_ExtendTimer, m_SuggestValidWord, m_ShuffleTheme};


    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        WQEventSystem.Subscribe(WQEventNames.AbilityUsed, HandleAbilityUsed);
    }

    void OnDestroy()
    {
        WQEventSystem.Unsubscribe(WQEventNames.AbilityUsed, HandleAbilityUsed);
    }

    void Start()
    {
        wqManager = WQManager.Instance;
        themesManager = WQThemes.Instance;
        gameManager = GameManager.Instance;
        turnTimer = TurnTimer.Instance;
    }

    void HandleAbilityUsed(object data){

        WQAbilityUsedResponse abilityUsedData = data as WQAbilityUsedResponse;
 
        if (abilityUsedData != null)
        {
            string abilityName = abilityUsedData.abilityUsed;
            Debug.Log($"<color=#FFE600FF> Ability Used Received: '{abilityName}'</color>");
        
            TryUseAbility(abilityName);
        }
        
        else{

            Debug.Log($"<color=#CE3B3BFF> Ability Parse failed! </color>");
        }
    }
    
    void TryUseAbility(string abilityInput)
    {
        if (AllAbilities.Contains(abilityInput)){
            
            WQPlayerSpawner.Instance.GetWQPlayerByIndex(gameManager.GetCurrentPlayerIndex()).Notify(abilityInput);
            UseAbility(abilityInput);
        }   
            
        else
            Debug.LogWarning($"'{abilityInput}' is not a valid ability.");
    }

    public void UseAbility(string abilityInput)
    {
        switch (abilityInput)
        {
            case m_SkipTurn:
                SkipTurn();
                break;

            case m_ShuffleTheme:
                ShuffleTheme();
                break;

            case m_ExtendTimer:
                ExtendTimer();
                break;

            case m_SuggestValidWord:
                SuggestValidWord();
                break;
        }
    }

    public void SkipTurn()
    {
        Debug.Log("Ability Used! Skipping turn.");
        if (gameManager != null)
        {
            gameManager.SwitchTurnImmediately();
        }
        else
        {
            Debug.Log("GameManager not connected.");
        }
    }

    public void ShuffleTheme()
    {
        Debug.Log("Ability Used! Shuffle theme.");

        if (themesManager != null && wqManager != null)
        {
            wqManager.SetNewTheme(themesManager.GetRandomWQTheme());
        }
        else
        {
            Debug.Log("WQManager & ThemesManager not connected");
        }
    }

    public void SuggestValidWord()
    {

        if (themesManager != null && wqManager != null)
        {
            string suggestableWord = wqManager.GetSuggestableWord();

            if (suggestableWord != null)
            {

                gameManager.FillInputFieldWithWord(suggestableWord);
                Debug.Log("Ability Used! Suggesting Valid Word.");
            }

            else
            {

                Debug.Log("<color=#FF5E5EFF>Ability Failed:</color> SuggestValidWord.");
            }
        }

        else
        {
            Debug.Log("WQManager & ThemesManager not connected");
        }
    }

    public void ExtendTimer()
    {
        Debug.Log("Ability Used! 10s added to timer.");
        if (turnTimer != null)
        {
            turnTimer.ExtendTime(10f);
        }
        else
        {
            Debug.Log("TurnTimer not connected.");
        }
    }
}


