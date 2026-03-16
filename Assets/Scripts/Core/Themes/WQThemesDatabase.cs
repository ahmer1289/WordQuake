using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database for storing WordQuake's special words.
/// </summary>
[CreateAssetMenu(fileName = "WQ Themes Database", menuName = "Word Quake/Themes Database")]
public class ThemesDatabase : ScriptableObject
{
    public List<WQTheme> AllWQThemes = new List<WQTheme>();
}

