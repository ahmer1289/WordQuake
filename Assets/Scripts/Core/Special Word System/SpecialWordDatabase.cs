using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database for storing WordQuake's special words.
/// </summary>
[CreateAssetMenu(fileName = "SpecialWords Database", menuName = "Word Quake/SpecialWords Database")]
public class SpecialWordDatabase : ScriptableObject
{
    public List<WQSpecialWord> spawnableWords = new List<WQSpecialWord>();

    private Dictionary<string, WQSpecialWord> wordDictionary = new Dictionary<string, WQSpecialWord>();

    public void Initialize()
    {
        wordDictionary.Clear();
        foreach (var word in spawnableWords)
        {
            wordDictionary[word.word.ToLower()] = word;
        }
    }

    public WQSpecialWord GetWord(string word)
    {
        wordDictionary.TryGetValue(word.ToLower(), out WQSpecialWord specialWord);
        return specialWord;
    }

    public bool CheckIfWordIsSpecial(string word){

        return GetWord(word);
    }
}

