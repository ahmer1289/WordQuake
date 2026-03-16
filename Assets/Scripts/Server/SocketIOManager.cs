using UnityEngine;
using SocketIOClient;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.U2D.IK;
using Unity.VisualScripting;

public class SocketIOManager : MonoBehaviour
{
	private SocketIOUnity m_serverSocket;
	public string currentRoomId = "";
	[HideInInspector] public string m_serverAddress = "https://wordquake-090cab5a8c9f.herokuapp.com/";
	// [HideInInspector] public string m_serverAddress = "http://localhost:5000/";
	public List<WQPlayerJoinedResponse> S_PlayerInformation = new List<WQPlayerJoinedResponse>();

  	public static SocketIOManager Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{

		m_serverSocket = new SocketIOUnity(new Uri(m_serverAddress), new SocketIOOptions
		{
			Query = new Dictionary<string, string>
			{
				{"token", "UNITY"}
			},
			Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
		});

		m_serverSocket.OnConnected += (sender, err) =>
		{
			Debug.Log("Connected to server!");
		};

		// m_serverSocket.OnAnyInUnityThread((name, response) =>
		// {
		// 	Debug.Log("Server received data on '" + name + "': " + response.ToString());
		// });

		// Notify when room is created
		m_serverSocket.On("roomCreated", response =>
		{
			Debug.Log("Room Created: " + response.ToString());
			
			// Deserialize JSON array into our WQRoomCreatedResponse
			List<WQRoomCreatedResponse> roomDataList = JsonConvert.DeserializeObject<List<WQRoomCreatedResponse>>(response.ToString());

			if (roomDataList != null && roomDataList.Count > 0 && currentRoomId == "") 
			{
				WQRoomCreatedResponse roomData = roomDataList[0];

				currentRoomId = roomData.roomId;

				S_PlayerInformation.Clear();

				Debug.Log($"Room ID: {roomData.roomId}, Max Players: {roomData.maxPlayers}, Current Players: {roomData.currentPlayers}, Room Name: {roomData.roomName}");

				WQEventSystem.TriggerEvent(WQEventNames.RoomCreated, roomData);
            	
			}
		});

		// Notify when player Joins the room
		m_serverSocket.On(WQEventNames.PlayerJoined, async response =>
		{
			// Deserialize JSON array into our WQPlayerJoinedResponse
			List<WQPlayerJoinedResponse> playerDataList 
				= JsonConvert.DeserializeObject<List<WQPlayerJoinedResponse>>(response.ToString());

			if (playerDataList != null && playerDataList.Count > 0)
			{
				
				WQPlayerJoinedResponse playerData = playerDataList[0];

				if(currentRoomId.Equals(playerData.roomId)){

					Debug.Log($"Player ID: {playerData.playerId}, Current Players: {playerData.currentPlayers}, Room ID: {playerData.roomId}");

					S_PlayerInformation.Add(playerData);

					//await SendTurnUpdate(currentRoomId, S_PlayerInformation[0].playerId);

					WQEventSystem.TriggerEvent(WQEventNames.PlayerJoined, playerData);

				}
			}
		});

		// Handle error
		m_serverSocket.On("error", response =>
		{
			Debug.LogError("Error: " + response.ToString());
		});

		m_serverSocket.On(WQEventNames.ReceiveWord, response =>
		{
			// Deserialize JSON array into our WQWordReceivedResponse
			List<WQWordReceivedResponse> wordDataList 
				= JsonConvert.DeserializeObject<List<WQWordReceivedResponse>>(response.ToString());

			if (wordDataList != null && wordDataList.Count > 0)
			{
				WQWordReceivedResponse wordData = wordDataList[0];
				
				Debug.Log($"Word: {wordData.word}, Player ID: {wordData.playerId}, Room ID: {wordData.roomId}");

				if(currentRoomId.Equals(wordData.roomId)){

					WQEventSystem.TriggerEvent(WQEventNames.ReceiveWord, wordData);
				}
			}
		});

		m_serverSocket.On("abilityInfo", response =>
		{
			Debug.Log("ABILITY USED FROM SERVER " + response.ToString());

			List<WQAbilityUsedResponse> abilityUsedDataList 
				= JsonConvert.DeserializeObject<List<WQAbilityUsedResponse>>(response.ToString());

			if (abilityUsedDataList != null && abilityUsedDataList.Count > 0)
			{
				WQAbilityUsedResponse abilityUsedData = abilityUsedDataList[0];

				Debug.Log($"Ability Used: {abilityUsedData.abilityUsed}, Player ID: {abilityUsedData.playerID}, Room ID: {abilityUsedData.roomId}");

				if(currentRoomId.Equals(abilityUsedData.roomId)){

					WQEventSystem.TriggerEvent(WQEventNames.AbilityUsed, abilityUsedData);
				}
			}
		});

		m_serverSocket.Connect();

		DontDestroyOnLoad(gameObject);
	}

	// Create Room Method
	public void CreateRoom(string roomName, int maxPlayers)
	{
		var roomData = new Dictionary<string, object>
		{
			{ "roomName", roomName },
			{ "maxPlayers", maxPlayers }
		};

		m_serverSocket.EmitAsync("createRoom", roomData);
	}

	public void CreateRoomWithSize(int size){

		//if(currentRoomId != null) Reset();
		CreateRoom("TESTINGGG", size);
	}
	
	// Send Game State to server
	public async Task SendGameState(string state, string roomId)
    {
		if(state != null && roomId != null){

			//WQGameStateRequest gameStateData = new WQGameStateRequest(state, roomId);

			//string jsonGameStateData = JsonConvert.SerializeObject(gameStateData);

			string test = $"{state},{roomId}";

			await m_serverSocket.EmitAsync("gameStateInfo", test);

			Debug.Log($"<color=#FFC430FF> Sending GameState: '{state}' to server! </color>");
		}
    }

	// Send Turn Update to server
	public async Task SendTurnUpdate(string roomID, string currentAllowedPlayerId, 
    	string currentPlayerPoints, string opponentPlayerPoints)
    {
		if(roomID != null && currentAllowedPlayerId != null){

			string test = $"{roomID},{currentAllowedPlayerId},{currentPlayerPoints},{opponentPlayerPoints}";

			Debug.Log($"Sent turn update {test} to server");
      
			await m_serverSocket.EmitAsync("checkturn", test);
		}
    }

	public void Reset(){

		SendGameState("GameOver", currentRoomId);
		currentRoomId = "";
		S_PlayerInformation.Clear();
	}

    void OnApplicationQuit()
    {
		Reset();
    }
}
