using UnityEngine;
using UnityEngine.UI;

public class PlayerLivesHolder : MonoBehaviour
{
    [SerializeField] Image[] cherryBlossoms;
    [SerializeField] Sprite m_Fill;
    [SerializeField] Sprite m_NoFill;
    [SerializeField] int m_TotalLives = 2;

    void Start()
    {
        Reset();
    }

    public void ReduceLife(int currentLife)
    {
        if(currentLife == 0){

            LoseAllLives();
            return;
        }
        
        cherryBlossoms[currentLife].sprite = m_NoFill;
    }

    void LoseAllLives(){

        foreach (var item in cherryBlossoms)
        {
            item.sprite = m_NoFill;
        }
    }

    void Reset()
    {
        foreach (var item in cherryBlossoms)
        {
            item.sprite = m_Fill;
        }
    }
}