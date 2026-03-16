using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextNotifyPopup : MonoBehaviour
{
    public TMP_Text notificationText; //John used <color=#FF00FF>Theme Shuffle!</color>

    public void Notify(string playerName, string abilityName){

        notificationText.text = $"{playerName} used <color=#FF00FF>{abilityName}!</color>";
        gameObject.SetActive(true);
    }
}
