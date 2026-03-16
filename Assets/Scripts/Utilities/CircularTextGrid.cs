/*I scrapped this script cause i used another technique however i think this will help for later use */

/*
using UnityEngine;
using TMPro;

public class CircularTextGrid : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public int maxlines = 12;

    private string[] lines;
    private int currentIndex = 0;

    void Start()
    {
        lines = new string[maxlines];
        for (int i = 0; i < maxlines; i++)
        {
            lines[i] = ""; // Start with empty lines
        }
        UpdateText();
    }

    public void AddWord(string newWord)
    {
        // Insert the word at the current index
        lines[currentIndex] = newWord;

        // Move to the next line (circular)
        currentIndex = (currentIndex + 1) % maxlines;

        UpdateText();
    }

    void UpdateText()
    {
        // Rearrange text so new words appear in a circular way
        string displayText = "";
        for (int i = 0; i < maxlines; i++)
        {
            int index = (currentIndex + i) % maxlines; // Adjust order
            displayText += lines[index] + "\n";
        }

        textMeshPro.text = displayText;
    }
}

 */
