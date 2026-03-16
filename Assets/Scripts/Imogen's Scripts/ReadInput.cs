using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInput : MonoBehaviour
{
    //script no longer used as I've put this in the main Shiritori Rules Logic, keeping in case we need this in the future
    private string input;

    public void ReadStringInput(string current)
    {
        input = current; //assigns the input string to currentString
        Debug.Log("Player used word: " + input);
    }
}
