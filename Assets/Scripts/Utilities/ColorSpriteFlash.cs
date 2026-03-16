using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColorSpriteFlash : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [Header("Color Ref"), SerializeField] Color color;

    [Header("Timing")]
    [SerializeField] private float animDuration;

    [Header("Ease Settings")]
    [SerializeField] private Ease easeType;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space)){

            Play();
        }
    }

    public void PlayWithoutReset(){

        spriteRenderer.DOColor(color, animDuration);
    }
    
    public void Play(){

        spriteRenderer.DOColor(color, animDuration)
    
        .OnComplete(() =>
        {
            Invoke(nameof(Reset), animDuration);
        });
    }

    void Reset(){

        spriteRenderer.DOColor(Color.white, animDuration).SetEase(easeType)

        .OnComplete(() =>
        {
            //OnComplete?.Invoke();
        });
    }
}
