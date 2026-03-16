using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using WQUtilities;
using TMPro;
using System;
using System.Threading.Tasks;

public class WQLobbyManager : MonoBehaviour
{
    public static WQLobbyManager Instance;
    [SerializeField] private GameObject characterCardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Button m_PlayButton; 

    [Space] public WQCharacters m_WQCharacters;
    [HideInInspector] public List<string> m_CharacterOptions = new List<string>();
    private int playerCount;
    private List<bool> playersReady = new List<bool>();
    private List<GameObject> playerCards = new List<GameObject>(); 
    private int currentPlayerIndex = 0; 
    public TMP_InputField timerDuration;
    public Button timerDurationSet;
    public string turnDuration;
    private float f_timerDuration;
    public TMP_Dropdown themeDropdown;
    string ThemeDefaultString = "Random Theme";
    public TMP_Text LobbyThemeDisplay;
    public TMP_Text m_LobbyID;
    string themetemp;
    public Button SettingsButton;
    public Button BackToMenuButton;
    public GameObject SettingsPanel;
    public Button ExitfromSettingsButton;
    [SerializeField] GameObject m_PlayerSelectionCanvas;
    [SerializeField] GameObject m_MainLobbyCanvas;
    [SerializeField] string roomId = "";

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        WQEventSystem.Subscribe(WQEventNames.PlayerJoined, GetPlayerJoinedData);
        WQEventSystem.Subscribe(WQEventNames.RoomCreated, UpdateRoomID);
    }

    void OnDisable()
    {
        WQEventSystem.Unsubscribe(WQEventNames.PlayerJoined, GetPlayerJoinedData);
        WQEventSystem.Unsubscribe(WQEventNames.RoomCreated, UpdateRoomID);
    }

    #region Event Handling

    void UpdateRoomID(object data){

        WQRoomCreatedResponse roomData = data as WQRoomCreatedResponse;

        if (roomData != null)
        {
            if (m_LobbyID != null)
            {
                roomId = roomData.roomId;

                PlayerPrefs.SetString("ROOM_ID", roomId);
                m_LobbyID.text = roomId;
            }

            else
            {
                Debug.LogError("m_LobbyID is null!");
            }
        }

        else
        {
            Debug.LogError("Failed to parse room created response!");
        }
    }

    void GetPlayerJoinedData(object data){

        WQPlayerJoinedResponse playerJoinedData = data as WQPlayerJoinedResponse;

        //TODO: Confirm if the roomID of the incoming player matches the existing roomID

        if (roomId.Equals(playerJoinedData.roomId))
        {
            if (playerJoinedData != null)
            {
                string m_CharacterName = playerJoinedData.charactername;
                string m_PlayerName = playerJoinedData.name;

                Debug.Log(m_CharacterName);
                Debug.Log(m_PlayerName);

                Debug.Log("Parsed player info response successfully!");

                SpawnPlayerCard(m_CharacterName, m_PlayerName);
            }

            else
            {
                Debug.LogError("Failed to parse player info response!");
            }
        }

        else
        {
            Debug.Log("Player can't join this room!");
        }
    }

    #endregion

    void Start()
    {   

        m_PlayerSelectionCanvas.SetActive(true);
        

        m_PlayButton.interactable = false;

        LobbyThemeDisplay.text = ThemeDefaultString;

        AssignThemefromLobby();
        
        SettingsButton.onClick.AddListener(ActivateSettingsPanel);
        BackToMenuButton.onClick.AddListener(BacktoStartMenu);
        ExitfromSettingsButton.onClick.AddListener(DeactiveSettingsPanel);
        timerDurationSet.onClick.AddListener(TimerDurationSet);
        m_PlayButton.onClick.AddListener( async () => await StartGameAsync());
    }

    public void SelectPlayers(int count)
    {
        m_PlayerSelectionCanvas.SetActive(false);
        

        PlayerPrefs.DeleteAll();

        playerCount = count;

        PlayerPrefs.SetInt("PlayerCount", count);

        SocketIOManager.Instance.CreateRoomWithSize(count);
    }

    void SpawnPlayerCard(string characterName, string playerName){

        Debug.Log("Spawning Player Card: " + characterName + " for " + playerName);

        GameObject card = Instantiate(characterCardPrefab, cardContainer);
        playerCards.Add(card);
        int playerIndex = playerCards.IndexOf(card);

        PlayerCard playerCardScript = card.GetComponent<PlayerCard>();
        if (playerCardScript != null)
        {
            playerCardScript.SetPlayerNumber(playerIndex);
            playerCardScript.PopulateCard(characterName, playerName);
        }

        if(playerCards.Count == playerCount){

            Debug.Log("<color=green>All players have joined. Click 'Play' to start the game...</color>");
            m_PlayButton.interactable = true;
        }

        else{

            m_PlayButton.interactable = false;
        }
    }

    private void ActivateSettingsPanel(){

        SettingsPanel.SetActive(true);
    }   

    private void DeactiveSettingsPanel(){

        SettingsPanel.SetActive(false);
    }

    // void SetCurrentPlayer(int index)
    // {
       
    //     foreach (var card in playerCards)
    //     {
    //         card.GetComponent<CanvasGroup>().interactable = false;
    //         card.transform.Find("Highlight").gameObject.SetActive(false); 
    //     }

    //     playerCards[index].GetComponent<CanvasGroup>().interactable = true;
    //     playerCards[index].transform.Find("Highlight").gameObject.SetActive(true);
    // }

    // void SetPlayerReady(int index)
    // {
        
    //     if (index != currentPlayerIndex) return; 

    //     playersReady[index] = true;
      
    //     playerCards[index].GetComponent<CanvasGroup>().interactable = false;

    //     PlayerCard playerCardScript = playerCards[index].GetComponent<PlayerCard>();
    //     if (playerCardScript != null)
    //     {
    //         string selectedCharacter = playerCardScript.characterDropdown.options[playerCardScript.characterDropdown.value].text;
    //         Debug.Log("Player " + (index + 1) + " is ready!");
    //         Debug.Log("Player " + (index + 1) + " selected character: " + selectedCharacter);

    //         playerCardScript.ReadyUp();
    //     }
        
    //     if (currentPlayerIndex < playerCount - 1)
    //     {
    //         currentPlayerIndex++;
    //         SetCurrentPlayer(currentPlayerIndex);
    //     }
    //     else
    //     {
            
    //     }
    // }

    private void TimerDurationSet()
    {
        if (timerDuration != null)
        {
            AssignTurnDuration(timerDuration.text);
        }
    }
    void CheckAllPlayersReady()
    {
        if (playersReady.TrueForAll(ready => ready))
        {
            Debug.Log("All players ready! Enabling Play button.");
            

            for (int i = 1; i < playerCards.Count + 1; i++)
            {
                Debug.Log($"Player {i} Saved Character Name: <color=#73FF5DFF>{PlayerPrefs.GetString(WQStorageKeys.SELECTED_PLAYER_CHARACTER_BASEKEY + i)}</color>");
            }
        }
    }

    public void ClearAllSelections()
    {
        Debug.Log("Resetting all selections.");
        for (int i = 0; i < playerCards.Count; i++)
        {
            PlayerCard playerCardScript = playerCards[i].GetComponent<PlayerCard>();
            if (playerCardScript != null)
            {
            
            }

            playersReady[i] = false;

            playerCards[i].GetComponent<CanvasGroup>().interactable = true;
            playerCards[i].transform.Find("Highlight").gameObject.SetActive(false);
        }

        currentPlayerIndex = 0;
        //SetCurrentPlayer(0);

        GameObject PlayButton = GameObject.Find("PlayButton");
        if(PlayButton != null)
        {
            PlayButton.GetComponent<Button>().interactable = false;
        }
    }
    
    public void AssignTurnDuration(string duration)
    {
        if(float.TryParse(duration, out f_timerDuration) && f_timerDuration > 0){
            PlayerPrefs.SetFloat("GAME_TIMER", f_timerDuration);
        }else{
            f_timerDuration = 10;
            PlayerPrefs.SetFloat("GAME_TIMER", f_timerDuration);
        }
    }    
    
    public void AssignThemefromLobby()
    {
        themeDropdown.ClearOptions();
        List<string> themeNames = new List<string>();
        foreach(WQTheme theme in WQThemes.Instance.themesDatabase.AllWQThemes)
        {
            themeNames.Add(theme.ThemeName);
        }
        themeDropdown.AddOptions(themeNames);
        themeDropdown.onValueChanged.AddListener((index)=> WQThemes.Instance.SetCurrentWQTheme(index));
        themetemp = themeDropdown.value.ToString();
        themeDropdown.onValueChanged.AddListener(SetThemeDisplay);
    }

    public void SetThemeDisplay(int index){

        LobbyThemeDisplay.text = WQThemes.Instance.GetCurrentWQTheme().ThemeName;
    }

    public async Task StartGameAsync()
    {
        await SocketIOManager.Instance.SendGameState("GameStarted", roomId);
        SceneManager.LoadSceneAsync("WQ_Demo");
    }
     public void BacktoStartMenu()
     {
        SceneManager.LoadScene("StartMenu");
        // destroy lobby code  
     }
}
