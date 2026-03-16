using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database for storing WordQuake's characters.
/// </summary>
[CreateAssetMenu(fileName = "Characters Database", menuName = "Word Quake/Characters Database")]
public class WQCharacters : ScriptableObject
{
    public WQCharacter[] availableWQCharacters;

}
