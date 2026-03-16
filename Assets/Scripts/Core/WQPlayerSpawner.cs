using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WQUtilities;

public class WQPlayerSpawner : MonoBehaviour
{   
    public static WQPlayerSpawner Instance;
    [SerializeField] WQPlayer m_PlayerPrefab;
    public List<WQPlayer> m_Players = new List<WQPlayer>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Spawns a player with the selected character key.
    /// <para>- Retrieves the character and playerName from PlayerPrefs before loading the corresponding WQCharacter. </para>
    /// </summary>
    // !!! IMPORTANT: Character selection is stored as 'CharacterName+PlayerIndex' and the "charactername" must match the names of characters in the Resources folder.
    
    public void SpawnPlayers(Transform[] spawnPositions){

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            int playerIndex = i+1;
            string selectedCharacterData = PlayerPrefs.GetString(WQStorageKeys.SELECTED_PLAYER_CHARACTER_BASEKEY + playerIndex);
            string[] splitKey = selectedCharacterData.Split("_"); //split using an underscore as we used ealier to set 
            string selectedCharacterName = splitKey[0]; 
            string playerName = splitKey[1]; 

            WQCharacter selectedCharacter = 
                Resources.Load<WQCharacter>($"Scriptable Objects/WQ Characters/{selectedCharacterName}"); // character path in the Resources folder

            if (selectedCharacter != null)
            {
                Transform spawnPosition = spawnPositions[i];
                WQPlayer spawnedPlayer = Instantiate(m_PlayerPrefab, spawnPosition);
                spawnedPlayer.Initialize(playerName, selectedCharacter, playerIndex);

                m_Players.Add(spawnedPlayer);
            }

            else
            {
                Debug.LogError("Character not found in Resources!");
            }
        }
    }

    //TODO: (tempoary)
    public WQPlayer GetWQPlayerByIndex(int index){

        return m_Players[index-1];
    }

    public WQSpecialWordReceiver GetWQPlayerSpecialWordReceiver(int index){

        return GetWQPlayerByIndex(index).GetWQPlayerSpecialWordReceiver();
    }
}
