using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using TMPro;

[Serializable]
public class WQTheme{

    public string ThemeName;
    public string[] ThemeSlugs;
    
    public WQTheme(string themeName){

        ThemeName = themeName;
    }
}

public class WQThemes : MonoBehaviour
{
    
    public static WQThemes Instance;
    private WQTheme currentWQTheme; // variable to hold the current theme
    public ThemesDatabase themesDatabase;
    
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
    /// This method iterates thorugh the array of themes and returns one theme at random
    /// </summary>
    public WQTheme GetRandomWQTheme(){

        if (themesDatabase == null)
        {
            Debug.LogWarning("ThemesDatabase is null!");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, themesDatabase.AllWQThemes.Count);
        currentWQTheme = themesDatabase.AllWQThemes[randomIndex];
        return currentWQTheme;
    }
    
    /// <summary>
    /// Returns the currently selected WQTheme.
    /// </summary>
    public WQTheme GetCurrentWQTheme()
    {
        return currentWQTheme;
    }
    public void SetCurrentWQTheme(int index)
    {
        currentWQTheme = themesDatabase.AllWQThemes[index];
        Debug.Log(" " + currentWQTheme.ThemeName);
    }

    /// <summary>
    /// Checks if the current theme is the Geography theme
    /// </summary>
    public bool IsInCountryTheme()
    {
        return currentWQTheme.ThemeName.Equals("Countries");
    }
    

    
}
