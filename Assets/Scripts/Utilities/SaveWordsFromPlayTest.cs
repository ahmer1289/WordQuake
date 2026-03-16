using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveWordsFromPlayTest : MonoBehaviour
{
    private Dictionary<string, (string, int)> wordFrequency = new Dictionary<string, (string, int)>();
    private string filePath;
    
    public static SaveWordsFromPlayTest Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "wordFrequency.csv");

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Word,Theme,Frequency\n");
        }
        else
        {
            var lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                var parts = lines[i].Split(',');
                if (parts.Length == 3)
                {
                    string word = parts[0];
                    string theme = parts[1];
                    int freq = int.Parse(parts[2]);
                    wordFrequency[word] = (theme, freq);
                }
            }
        }
    }


    public void TrackWord(string word)
    {
        word = word.ToLower();

        string theme = WQThemes.Instance.GetCurrentWQTheme().ThemeName;

        if (wordFrequency.ContainsKey(word))
        {
            var existingWord = wordFrequency[word];
            existingWord.Item1 = theme;
            wordFrequency[word] = (existingWord.Item1, existingWord.Item2 + 1);
        }
        else
        {
            wordFrequency[word] = (theme, 1);
        }
    }

    public void SaveToFile()
    {
        List<string> lines = new List<string>
        {
            "Word,Theme,Frequency"
        };

        foreach (var entry in wordFrequency)
        {
            lines.Add($"{entry.Key},{entry.Value.Item1},{entry.Value.Item2}");
            Debug.Log($"{entry.Key},{entry.Value.Item1},{entry.Value.Item2}");
        }

        File.WriteAllLines(filePath, lines); // Overwrite is okay now
    }


    void OnApplicationQuit()
    {
        SaveToFile();
    }
}