using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } //create a singleton instance of gamemanager
    public GameState currentState = GameState.Player1Turn; //defaults to p1 turn
    [Space, SerializeField] private WQManager m_WQManager; //reference to shiritori logic script
    [SerializeField] SpecialWordDatabase wordDatabase;

    [Space, Tooltip("Array of Player Spawn positions according to the number of players for this scene")] 
    public Transform[] m_PlayerSpawnPositions;

    [Header("Player Objects")]
    [SerializeField] SlideDisplayPopUp m_Player1SlideDisplayPopUp;
    [SerializeField] Transform m_Player1TimePos;
    [SerializeField] SlideDisplayPopUp m_Player2SlideDisplayPopUp;
    [SerializeField] Transform m_Player2TimePos;

    [Header("UI Objects"), SerializeField] TMP_InputField playerInputField; //reference to TMP input field object
    public Button submitButton;
    public Button ExitButton;
    public GameObject ExitButtonPanel;
    public TMP_Text WinnerReveal;
    public GameObject ThemeRevealUI;
    public GameObject GameUI;
    public TurnTimer turnTimer;
    private GameState previousState;
    private string playerInput;
    [SerializeField] private TMP_Text finalScoresText;
    private WQPlayerSpawner playerSpawner;
    private WQPointSystem pointSystem;
    private float turnStartTime;

    #region Server variables
    private SocketIOManager socketIOManager;
    public string S_CurrentRoomId = null;
    private List<WQPlayerJoinedResponse> S_PlayerInformation = new List<WQPlayerJoinedResponse>();

    #endregion
    
    public Action OnTurnSwitch;
    
    private void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        WQEventSystem.Subscribe(WQEventNames.ReceiveWord, HandleWordReceivedData);
        WQEventSystem.Subscribe("WordPopUpAnimationComplete", CheckAndSpawnSpecialWord);
    }

    void OnDestroy()
    {
        WQEventSystem.Unsubscribe(WQEventNames.ReceiveWord, HandleWordReceivedData);
        WQEventSystem.Unsubscribe("WordPopUpAnimationComplete", CheckAndSpawnSpecialWord);

        TurnTimer.OnTurnEnd -= GameOver;

        socketIOManager.Reset();

        DOTween.KillAll();
    }

    async Task Start()
    {   

        turnTimer = TurnTimer.Instance;
        socketIOManager = SocketIOManager.Instance;
        playerSpawner = WQPlayerSpawner.Instance;
        pointSystem = WQPointSystem.Instance;

        S_PlayerInformation = socketIOManager.S_PlayerInformation;
        S_CurrentRoomId = PlayerPrefs.GetString("ROOM_ID");

        currentState = GameState.Player1Turn; //starts with p1 turn

        InitializeButtons();
        ShowTheme();

        //TODO: Re-enable SpawnPlayers() !Done;
        SpawnPlayers();
        
        // Subscribe to TurnTimer event
        TurnTimer.OnTurnEnd += GameOver;

        //? Start Game
        Debug.Log($"S_PlayerInformation[0]");
        await SendPlayerTurnUpdate(S_PlayerInformation[0]);
    }

    #region Initializations

    void InitializeButtons(){

        submitButton.onClick.AddListener( () => ProcessInput(playerInputField.text));
        submitButton.onClick.AddListener(ClearInput);
        ExitButton.onClick.AddListener(LeaveGame);
    }

    void ClearInput()
    {
        playerInputField.text = "";
    }

    #endregion

    #region Event Handling

        public void HandleWordReceivedData(object data){

            WQWordReceivedResponse wordData = data as WQWordReceivedResponse;
 
            if (wordData != null)
            {
                string word = wordData.word;
                ProcessInput(word);
                int playerIndex = GetCurrentPlayerIndex();
                playerSpawner.GetWQPlayerByIndex(playerIndex).IncrementWordsEntered();
                Debug.Log($"<color=#22CFFAFF> Word Received: '{word}'</color>");
            }
        }

    #endregion

    private void ProcessInput(string word)
    {
        ReadInput(word);
        m_WQManager.DisplayPreviousWord();
    }
    
    private void SpawnPlayers(){

        WQPlayerSpawner.Instance.SpawnPlayers(m_PlayerSpawnPositions);
    }

    //METHOD FOR THEME REVEAL--------------------------------------------------------
    private void ShowTheme()
    {
        //! here lies the show theme delays
        Invoke(nameof(DelayShowTheme), 0f); 
        Invoke(nameof(HideThemeReveal), 3f); // 1s delay + 2s duration
    }

    private void DelayShowTheme()
    {
        GameUI.SetActive(false);
        ThemeRevealUI.SetActive(true);
    }

    private void HideThemeReveal()
    {
        ThemeRevealUI.SetActive(false);
        GameUI.SetActive(true);

        StartGame();
    }

    private void StartGame(){ // base method to Start Game

        SetTimerPosition(m_Player1TimePos);
        playerSpawner.GetWQPlayerByIndex(1).ToggleHighlight();
        turnStartTime = Time.time;
        turnTimer.StartTimer();
    }

    private void SetTimerPosition(Transform pos){

        turnTimer.m_TimerObject.transform.SetParent(pos);
        turnTimer.m_TimerObject.transform.localPosition = Vector3.zero;
    }

    public void ReadInput(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            playerInput = input;
            turnTimer.PauseTimer();
            float timeTaken = Time.time - turnStartTime;
            int playerIndex = GetCurrentPlayerIndex();
            playerSpawner.GetWQPlayerByIndex(playerIndex).AddTimeTaken(timeTaken);
        }
            ReadPlayerInput(playerInput);
    }

    void ReadPlayerInput(string Input)
    {
        switch (currentState)
        {
            case GameState.Player1Turn:
                m_WQManager.ReadPlayerInput(Input);
                break;

            case GameState.Player2Turn:
                m_WQManager.ReadPlayerInput(Input);
                break;

            case GameState.GameOver:
                break;

            default:
                break;
        }
    }
    
    public void HandleWordIsValid(string word){

        //? Point system updates
        pointSystem.AwardWordLengthBonusPoints(word);
        pointSystem.AwardTimeBonusPoints(turnTimer.timeRemaining);

        int playerIndex = GetCurrentPlayerIndex();

        switch(playerIndex){

            case 1:
                m_Player1SlideDisplayPopUp.SetWordText(word);
                break;

            case 2:
                m_Player2SlideDisplayPopUp.SetWordText(word);
                break;
        }

        //? Calling Switch Turn with m_WordDisplayPopUp event!

        playerSpawner.GetWQPlayerByIndex(playerIndex).AddPoints(pointSystem.m_TotalPointsToBeAwarded);

        pointSystem.ResetPoints();
    }

    public void HandleWordError(string word, string errorMessage){

        int playerIndex = GetCurrentPlayerIndex();
        playerSpawner.GetWQPlayerByIndex(playerIndex).IncrementInvalidWords();

        switch (playerIndex){

            case 1:
                m_Player1SlideDisplayPopUp.SetFeedBackText(word, errorMessage);
                break;

            case 2:
                m_Player2SlideDisplayPopUp.SetFeedBackText(word, errorMessage);
                break;
        }
        
    }

    void CheckAndSpawnSpecialWord(object data){

        //TODO: (tempoary) //////////////////////////////////////////////////
        if (wordDatabase.CheckIfWordIsSpecial(playerInput))
        {
            int playerIndex = GetCurrentPlayerIndex();

            WQPlayer player = WQPlayerSpawner.Instance.GetWQPlayerByIndex(playerIndex);
            //player.TakeDamage();

            SpecialWordSpawner.Instance.SpawnWord(playerInput, playerIndex);
        }

        else{
            
            SwitchTurn();
        }
    }   

    //METHOD TO MANAGE TURN SWITCHING--------------------------------------------------------
    private void SwitchTurn() //manages switching back and forth game states between player 1 and player 2's turn
    {

        OnTurnSwitch?.Invoke(); // call this action to inform all subscribers that the gamemanager switched turns

        switch (currentState)
        {
            case GameState.Player1Turn:
                currentState = GameState.Player2Turn;
                SendPlayerTurnUpdate(S_PlayerInformation[1]);
                HighlightPlayerTurnGlow(2);
                SetTimerPosition(m_Player2TimePos);
                break;
            case GameState.Player2Turn:
                currentState = GameState.Player1Turn;
                SendPlayerTurnUpdate(S_PlayerInformation[0]);
                HighlightPlayerTurnGlow(1);
                SetTimerPosition(m_Player1TimePos);
                break;

            case GameState.GameOver:
                break;

            default:
                Debug.LogError("Invalid game state for switching.");
                break;
        }

        turnTimer.ResetTimer();
        turnStartTime = Time.time;
    }

    private IEnumerator SwitchTurnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchTurn();
    }

    public void SwitchTurnImmediately()
    {
        SwitchTurn();
    }

    public void SwitchTurnWithDelay(float delay = 0f)
    {
        StartCoroutine(SwitchTurnCoroutine(delay));
    }

    async Task SendPlayerTurnUpdate(WQPlayerJoinedResponse currentPlayerinfo){

        Debug.Log($" <color=#D56CFFFF> It's Player {currentPlayerinfo.name}'s turn! </color>");

        // Sends turn update to server
        await socketIOManager
            .SendTurnUpdate(S_CurrentRoomId, currentPlayerinfo.playerId, GetCurrentAllowedWQPlayerPoints(), GetOtherWQPlayerPoints());
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        previousState = currentState; //previousState to hold the last state before game over 

        EndGame();
    }

    public int GetWinningPlayer()
    {
        // If the game ends on Player 2's turn then Player 1 wins.

        int indexOfWinner = 1;
        
        if(playerSpawner.GetWQPlayerByIndex(1).Lives > 0 && playerSpawner.GetWQPlayerByIndex(2).Lives > 0){

            if (previousState == GameState.Player2Turn) 
            {
                WinnerReveal.text = $"{S_PlayerInformation[0].name} wins!"; // player 1 wins
                playerSpawner.GetWQPlayerByIndex(2).ToggleLoseOverlay(); //player 2 lost
                indexOfWinner = 1;
            }
            else if (previousState == GameState.Player1Turn)
            {
                WinnerReveal.text = $"{S_PlayerInformation[1].name} wins!"; // player 2 wins
                playerSpawner.GetWQPlayerByIndex(1).ToggleLoseOverlay(); // player 1 lost
                indexOfWinner = 2;
            }
        }

        else{
            
            for (int i = 1; i < playerSpawner.m_Players.Count + 1; i++)
            {
                if(playerSpawner.GetWQPlayerByIndex(i).Lives > 0){

                    indexOfWinner = i;
                    WinnerReveal.text = $"{S_PlayerInformation[indexOfWinner-1].name} wins!";
                    playerSpawner.GetWQPlayerByIndex(i).DisableHighlight();
                }

                else{

                    playerSpawner.GetWQPlayerByIndex(i).DisableHighlight();
                    playerSpawner.GetWQPlayerByIndex(i).EnableLoseOverlay();
                }
            }
        }

        return indexOfWinner;
    }

        public void EndGame()
        {
            currentState = GameState.GameOver;
            GetWinningPlayer();
            GameUI.SetActive(false);
            ThemeRevealUI.SetActive(true);
            ExitButtonPanel.SetActive(true);

            Debug.Log("=== GAME OVER: Final Scores ===");
            string finalResults = "Final Scores:\n";

            foreach (var player in WQPlayerSpawner.Instance.m_Players)
            {
                string playerName = player.GetPlayerName();
                int points = player.GetPoints();
                int wordsEntered = player.GetWordsEntered(); 
                int invalidWords = player.GetInvalidWordCount();
                float avgTime = player.GetAverageTimePerWord();
            
            Debug.Log($"{playerName}: {points} points | Words Entered: {wordsEntered} | Invalid Words: {invalidWords} | Avg Time: {avgTime:F2}s");
            finalResults += $"{playerName}: {points} points | Words Entered: {wordsEntered} | Invalid Words: {invalidWords} | Avg Time: {avgTime:F2}s\n";
        }

            finalScoresText.text = finalResults;
            //TODO: Work on play again feature. Prolly haveto move this to the button that leads back to the main menu
            socketIOManager.Reset();
        }

        public void LeaveGame()
        {
        SceneManager.LoadScene("StartMenu");
    }

    public enum GameState
    {
        Player1Turn,
        Player2Turn,
        GameOver
    }

    public int GetCurrentPlayerIndex(){

        switch (currentState)
        {
            case GameState.Player1Turn:
                return 1;
            case GameState.Player2Turn:
                return 2;
            default:
                return 0;
        }
    }

    string GetCurrentAllowedWQPlayerPoints(){

        return $"{playerSpawner.GetWQPlayerByIndex(GetCurrentPlayerIndex()).m_TotalPoints}";
    }

    string GetOtherWQPlayerPoints()
    {
        int currentIndex = GetCurrentPlayerIndex();
        int otherIndex = (currentIndex == 1) ? 2 : 1;

        return $"{playerSpawner.GetWQPlayerByIndex(otherIndex).m_TotalPoints}";
    }

    WQPlayer GetCurrentWQPlayer(){

        return playerSpawner.GetWQPlayerByIndex(GetCurrentPlayerIndex());
    }

    void HighlightPlayerTurnGlow(int playerIndex) {

        // Code to signify player's turn
        foreach (var player in playerSpawner.m_Players)
        {
            player.ToggleHighlight();
        }
    }

    #region GameManager linked Abilities

    public void FillInputFieldWithWord(string word){

        if(word != null)
        {
            ProcessInput(word);
        }
    }

    #endregion
}
