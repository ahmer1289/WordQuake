using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ExpandOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform panel;
    public TextMeshProUGUI textMeshPro; 
    public Sprite sprite1;
    public Sprite sprite2;  
    public Image myImageComponent;
    private Vector2 originalSize;

    void Start()
    {
        
        textMeshPro.enabled = false; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ExpandPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MinimizePanel();
    }

    public void ExpandPanel()
    {
        
        myImageComponent.sprite = sprite1;
        textMeshPro.enabled = true; 
    }

    public void MinimizePanel()
    {   
        
        myImageComponent.sprite = sprite2;
        textMeshPro.enabled = false;
    }
}
