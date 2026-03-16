using UnityEngine;

[CreateAssetMenu(fileName = "SpecialWord", menuName = "Word Quake/SpecialWord")]
public class WQSpecialWord : ScriptableObject
{
    public string word;
    public float m_LifeTime; // total time the special object will exist for depending on it's drama
    public float m_Force; // used for physics-based objects
    public GameObject specialWordPrefab;

}
