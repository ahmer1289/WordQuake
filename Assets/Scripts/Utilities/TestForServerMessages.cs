using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;


//  JSON TEMPLATE
//  {
//    "word": "soccer",
//    "playerName": "Promise"
//  } 

public class TestForServerMessages : MonoBehaviour
{
    
    [SerializeField] TextAsset textAsset;
    [SerializeField] TMP_Text wordDisplay;
    GameManager gameManager;
    WQManager wqManager;

    void OnDisable()
    {
        gameManager.OnTurnSwitch -= () => SetWordTextVisibility(false);
    }

    void Start()
    {  
        gameManager = GameManager.Instance;
        wqManager = WQManager.Instance;

        gameManager.OnTurnSwitch += () => SetWordTextVisibility(false);

        SetWordTextVisibility(false); 
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){

            ParseJSON();
        }    
    }

    void ParseJSON(){

        JObject parsedJson = JObject.Parse(textAsset.text); 

        string word = parsedJson["word"].ToString();
        string playerName = parsedJson["playerName"].ToString();

        Debug.Log("word: " + word);
        Debug.Log("playerName: " + playerName);

        gameManager.ReadInput(word);
        wqManager.DisplayPreviousWord();

        wordDisplay.text = $"{playerName} : {word}";
        SetWordTextVisibility(true);
    }

    void SetWordTextVisibility(bool isVisible){

        wordDisplay.gameObject.SetActive(isVisible);        
    }
}
