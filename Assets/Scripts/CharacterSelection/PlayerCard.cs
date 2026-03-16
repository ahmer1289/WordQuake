using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using WQUtilities;

public class PlayerCard : MonoBehaviour
{
    public TMP_Text playerNameText;
    public Image characterImage; 
    private string playerName;
    private int playerIndex;
    private bool isNameStored = false;
    private Color originalButtonColor;
    static private int anonymousIndex = 0;
    WQCharacters wqCharacters;
    WQCharacter wqCharacter;

    public void SetPlayerNumber(int index)
    {
        playerIndex = index + 1;
    }

    public void PopulateCard(string characterName, string playerName)
    {
        if (isNameStored) return;

        characterImage.sprite = GetWQCharacterSpriteByName(characterName);
        playerNameText.text = playerName;

        PlayerPrefs.SetString($"{WQStorageKeys.SELECTED_PLAYER_CHARACTER_BASEKEY}{playerIndex}", $"{characterName}_{playerName}"); // stored name of selected character
        isNameStored = true;

        //nameInputField.interactable = false;
        //characterDropdown.interactable = false;
        //readyButton.interactable = false;

        //readyButton.image.color = Color.green;
    }

    Sprite GetWQCharacterSpriteByName(string selectedCharacterName){

        WQCharacter character = 
            Resources.Load<WQCharacter>($"Scriptable Objects/WQ Characters/{selectedCharacterName}"); // character path in the Resources folder

        if (character == null)
        {
            return null;
        }

        else{

            return character.m_Sprite;
        }

        
    }
}
