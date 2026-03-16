using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class TestWithWikipedia : MonoBehaviour
{

    [SerializeField] string wordToCheck;
    [Space, SerializeField] WQTheme m_CurrentTheme;

    void Start()
    {
        StartCoroutine(Check());
    }

    IEnumerator Check()
    {
        wordToCheck = wordToCheck.ToLower().Trim();
        string url = "https://en.wikipedia.org/wiki/" + wordToCheck;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
 
            string htmlContent = request.downloadHandler.text;
            Debug.Log(htmlContent);

            ExtractDefinition(htmlContent);
        }
    }

    void ExtractDefinition(string html)
    {

        int start = html.IndexOf("<p>"); 
        int end = html.IndexOf("</p>", start + 500); 

        if (start != -1 && end != -1)
        {
            string definitionText = html.Substring(start + 3, end - start); 
            
            if(!definitionText.Contains("<ul>")){

                end = html.IndexOf("</p>", start + 10000);
                definitionText = html.Substring(start + 3, end - start);
            }

            Debug.Log("Definition: " + definitionText);
            
            #region CheckForThemes

            string result;

            result = CheckIfSpecialWordExistsInTheme(definitionText) 
                ? $"<color=#5EFF69FF>{wordToCheck} exists in the '{m_CurrentTheme.ThemeName}' theme</color>" 
                    : $"<color=#FF4747FF>{wordToCheck} does not exist in the '{m_CurrentTheme}' theme</color>"; 
            
            Debug.Log(result);

            #endregion

        }

        else
        {
            Debug.Log("Definition not found.");
        }
    }

    bool CheckIfSpecialWordExistsInTheme(string deinitionToCheckIn){
        
        bool itContains = false;

        foreach (var slug in m_CurrentTheme.ThemeSlugs)
        {
            if(deinitionToCheckIn.Contains(slug.ToLower())){

                itContains = true;
                break;
            }
            
        }

        return itContains;
        
    }

}