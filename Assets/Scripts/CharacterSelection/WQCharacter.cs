using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Word Quake/Character")]
public class WQCharacter : ScriptableObject
{
    public string m_CharacterName;
    public Sprite m_Sprite;
    public RuntimeAnimatorController m_Animator;
    public RuntimeAnimatorController m_AnimatorWithGlow;
    public Vector2 m_SittingPositionOnCushion;
}
