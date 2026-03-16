using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class Player
{
    public string Name;
    public int Lives;
    public bool IsOut => Lives == 0;


    public Player(string name)
    {
        Name = name;
        Lives = 2;
    }

    public void LoseLife()
    {
        if (Lives > 0)
        {
            Lives--;
        }
    }
}