/* WordValidatorAPI.cs
   version 1.0 - Jan 31st, 2025
   Copyright (C) CheddarCat, WordQuake.
*/

using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WordValidatorAPI : MonoBehaviour
{
    
    public static WordValidatorAPI Instance;
    private const string baseUrl = "https://api.wordnik.com/v4/word.json/";
    private const string apiKey = "uu4u3mvg0t90noqg17wymmbwrqosx7m43tcxqhryaochflhl2";
    string jsonResponse;
    string definition;
    JArray jsonArray;

    private void Awake()
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

    /// <summary>
    /// This calls a coroutine that runs the check with the given word. 
    /// Since we're making web requests to the API, the results won't be instant, 
    /// which is why we use a callback function to return the result once the request completes.
    /// The callback will return `true` if the word exists in the dictionary, and `false` otherwise.
    /// </summary>
    /// <param name="word">The word to check against the dictionary API.</param>
    /// <param name="doesWordExistCallback">A function that can contain actions if the word exists and false if it doesn't.</param>
    /// <param name="isWordPluralCallback">Same as the doesWordExistCallback, but to handle actions when the plural check returns.</param>

    public void CheckIfWordExists(string word, Action<bool> doesWordExistCallback, Action<bool> isWordPluralCallback, bool IsInCountryTheme = false)
    {
        StartCoroutine(CheckWordCoroutine(word, doesWordExistCallback, isWordPluralCallback, IsInCountryTheme));
    }

    private IEnumerator CheckWordCoroutine(string word, Action<bool> doesWordExistCallback, 
        Action<bool> isWordPluralCallback, bool IsInCountryTheme = false)
    {
        
        if(IsInCountryTheme){

            word = (char.ToUpper(word[0]) + word.Substring(1).ToLower()).Trim(); //make the first letter of the string a capital. E.g nigeria to Nigeria
        }

        else{

            word = word.ToLower().Trim(); //clean the string
        }
    
        string url = $"{baseUrl}{word}/definitions?partOfSpeech=noun&sourceDictionaries=wordnet&api_key={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try //using a try catch block to avoid crashes and log errors correctly
            {
                jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse);
                jsonArray = JArray.Parse(jsonResponse); //parsing the returned json from the api into a JArray to properly access it's contents
            
                if (jsonArray.Count > 0) // the JArray should have contents
                {
                    //? Example of what jsonArray might contain for the word: "dogs":
                    /*
                    [
                        {
                            "partOfSpeech": "noun",
                            "text": "Plural form of dog.",
                            "word": "dogs"
                        },
                        {
                            "partOfSpeech": "verb",
                            "text": "Third-person singular simple present indicative form of dog.",
                            "word": "dogs"
                        }
                    ]
                    */

                    var entry = jsonArray[0]; // we only need the first definition of the word, so we only need the first JSON entry (it's like a dictionary) in the array. 
                    
                    if (entry["partOfSpeech"] != null) // checking if "partOfSpeech" content exists, then the word exists 
                    {
                        doesWordExistCallback(true);

                        if (entry["text"] != null) // checking if "text" content exists in the JSON entry 
                        {
                            definition = entry["text"].ToString().ToLower(); //same here
                            Debug.Log(definition);
                            isWordPluralCallback(definition.Contains("plural form of")); //check if the word is a noun and described as a plural form.
                        }
                        else
                        {
                            isWordPluralCallback(false);
                        }
                    } 

                    else{

                        doesWordExistCallback(false);
                    }
                }
            }
            
            catch 
            {
                Debug.LogError("Error parsing response");
            }
        }

        else
        {
            doesWordExistCallback(false);
            
            if (request.responseCode != 404)
            {
                Debug.LogError($"Error: {request.error}");
            }
        }
    }

    /// <summary>
    /// Bool method to check if entered word exists in the referenced theme.
    /// The will return `true` if the word exists in the theme, and `false` otherwise.
    /// This method essentially iterates through all the theme slugs in the referenced theme to check if the word's definition (json response) contains it.
    /// If it does, then we can return true saying this "word" exists in the theme because the word's definition contained the theme's slugs
    /// </summary>
    /// <param name="wordToCheck">The word to check.</param>
    /// <param name="m_CurrentTheme">The current theme.</param>
    public bool CheckIfWordExistsInTheme(string wordToCheck, WQTheme m_CurrentTheme){
        
        bool doesExist = false;
        wordToCheck = wordToCheck.ToLower().Trim();
        string definitionToCheck = null;
        
        foreach (var entry in jsonArray)
        {
            if (entry["partOfSpeech"] != null)
            {
                definitionToCheck += entry["text"].ToString().ToLower();
            }
            
        }

        foreach (var slug in m_CurrentTheme.ThemeSlugs)
        {
            if(definitionToCheck.Contains(slug.ToLower().Trim())){

                doesExist = true;
                Debug.Log("Slug: " + slug);
                break;
            }
        }
        
        Debug.Log(definitionToCheck);

        Debug.Log(doesExist 
            ? $"<color=#5EFF69FF>{wordToCheck} exists in the '{m_CurrentTheme.ThemeName}' theme</color>" 
                : $"<color=#FF4747FF>{wordToCheck} does not exist in the '{m_CurrentTheme.ThemeName}' theme</color>");

        return doesExist;
    }
}


