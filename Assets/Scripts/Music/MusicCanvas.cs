using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicCanvas : MonoBehaviour
{
    public RectTransform uiElement;
    private float moveDuration = 1f;
    private float displayTime = 3f;

    void Start()
    {
        StartCoroutine(ShowandHideUI());
    }
    
    IEnumerator ShowandHideUI(){
        float screenWidth = Screen.width;

        Vector2 offScreenPos = new Vector2(screenWidth, 600); 
        Vector2 onScreenPos = new Vector2(screenWidth - 100, 600); 
        uiElement.anchoredPosition = offScreenPos;
        yield return StartCoroutine(MoveUI(uiElement, offScreenPos, onScreenPos, moveDuration));
        yield return new WaitForSeconds(displayTime);
        yield return StartCoroutine(MoveUI(uiElement, onScreenPos, offScreenPos, moveDuration));
    }

    IEnumerator MoveUI(RectTransform image, Vector2 StartPos, Vector2 EndPos, float moveDuration){
        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            image.anchoredPosition = Vector2.Lerp(StartPos, EndPos, elapsedTime / moveDuration);
            yield return null;
        }

        image.anchoredPosition = EndPos;
    }
}
