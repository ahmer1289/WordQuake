using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWordsSet : MonoBehaviour
{
    private HashSet<string> specialWords = new HashSet<string>()
    {
        "blockparty", "gravriders", "chessrancher", "ascension", "disc",
        "racket", "medal", "sailing", "hockey", "soccer", "turtle", "elephant",
        "armadillo", "eagle", "octopus", "panda", "edamame", "milk", "asparagus", 
        "aubergine", "kebab", "tequila", "england", "norway", "australia",
        "egypt", "wales"
    };

    public bool IsSpecialWord(string word)
    {
        return specialWords.Contains(word.ToLower().Trim());
    }
    

}
