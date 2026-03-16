/* SpecialWordsLoader.cs
   version 1.0 - Jan 31st, 2025
   Copyright (C) CheddarCat, WordQuake.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThemedWordsLoader : MonoBehaviour
{   
    public static ThemedWordsLoader Instance;
    [SerializeField] private TextAsset csvFile;
    public Dictionary<string, WQTheme> m_ThemedWordsDictionary = new Dictionary<string, WQTheme>();

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

        LoadCSV();
    }

    private void LoadCSV()
    {

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length-1; i++) //starting from 1 to skip the header row (words, theme, etc.)
        {
            string[] row = lines[i].Split(','); //example first row: Disc, Sports, Damage

            string word = row[0].Trim().ToLower(); //first column = "Disc"

            string theme = row[1].Trim().ToLower(); //second column = "Sports"

            WQTheme wqTheme = new WQTheme(theme);

            if (!m_ThemedWordsDictionary.ContainsKey(word))
            {
                m_ThemedWordsDictionary.Add(word, wqTheme);
            }
            
        }

        Debug.Log($"<b>Themed words from excel/csv file loaded successfully! {m_ThemedWordsDictionary.Count} words added.</b>");
    }

    public bool CheckIfWordExistsLocally(string word)
    {
        word = word.ToLower().Trim();
        return m_ThemedWordsDictionary.ContainsKey(word);
    }

    /// <summary>
    /// Checks if a word exists in a specific theme.
    /// </summary>
    /// <param name="word">The word to check</param>
    /// <param name="wqTheme">The theme to check in</param>
    /// <returns>True if the word exists in the theme, otherwise false.</returns>
    public bool CheckIfWordExistsInThemeLocally(string word, WQTheme wqTheme)
    {
        word = word.ToLower().Trim();
        return GetAllWordsInTheme(wqTheme).Contains(word);
    }

    public List<string> GetAllWordsInTheme(WQTheme wqTheme){

        List<string> validWordsInTheme = new List<string>();

        foreach (var word in m_ThemedWordsDictionary.Keys)
        {
            string wordthemeName = m_ThemedWordsDictionary[word].ThemeName.ToLower();

            if(wordthemeName.Equals(wqTheme.ThemeName.ToLower())){
                
                validWordsInTheme.Add(word);
            }
        }         
        
        return validWordsInTheme;
    }
}
