using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]

/// <summary>
/// Fades and Scales the gameobject up when enabled and down when disabled
/// </summary>
public class WQPopupAnimator : MonoBehaviour
{
    [Space, SerializeField, Tooltip("Allows this popup to fade out and be disabled immediately after popping up.")] 
    private bool shouldFadeOutAfterDisplay;
    [SerializeField] private bool useSlideAnimation;
    [SerializeField] private bool useFloatAnimation;
    [SerializeField] private bool useScaleAnimation;

    [Header("Timing")]
    [SerializeField] private float startDelay = 0;
    [SerializeField] private float animDuration;
    [SerializeField] private float hideDelay;

    [Header("Position Settings")]
    [SerializeField] private float slideDistance = 500f;
    [SerializeField] private float floatDistance = 500f;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 startScale = Vector3.zero;
    [SerializeField] private Vector3 endScale = Vector3.one;

    [Header("Ease Settings")]
    [SerializeField] private Ease easeType;

    public Action OnComplete;

    CanvasGroup canvasGroup;
    Vector3 originalPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        ShowPopupWithDelay(startDelay);
    }

    void OnDisable()
    {
        HidePopup();
    }

    public void SetStartDelay(float delay){

        startDelay = delay;
    }

    public void SetHideDelay(float delay){

        hideDelay = delay;
    }

    void ShowPopupWithDelay(float delay = 0){
        
        Invoke(nameof(ShowPopup), delay);
    }

    void ShowPopup(){
        
        // Reset();

        // DOTween.Play(this);

        canvasGroup.alpha = 0;

        if(useScaleAnimation){

            transform.localScale = startScale;
            transform.DOScale(endScale, animDuration).SetEase(easeType);
        } 

        if(useFloatAnimation){

            transform.localPosition = originalPosition + new Vector3(0, -floatDistance, 0);
            transform.DOLocalMove(originalPosition, animDuration).SetEase(easeType)
            .OnComplete(() =>
                {
                    transform.DOLocalMove(transform.localPosition + new Vector3(0, floatDistance, 0), animDuration).SetEase(easeType);
                    canvasGroup.DOFade(0, animDuration/2f).SetEase(easeType).OnComplete(() =>
                    {
                        gameObject.SetActive(false); 
                    });
                });

            canvasGroup.DOFade(1, animDuration).SetEase(easeType);
        }

        else{

            canvasGroup.DOFade(1, animDuration).SetEase(easeType)
            .OnComplete(() =>
            {
                if(shouldFadeOutAfterDisplay)
                Invoke(nameof(HidePopup), animDuration + hideDelay);
            });
        }

        if(useSlideAnimation){

            transform.localPosition = originalPosition + new Vector3(-slideDistance, 0, 0);
            transform.DOLocalMove(originalPosition, animDuration).SetEase(easeType);
        }

        
    }

    void HidePopup(){

        if(useSlideAnimation) transform.DOLocalMove(transform.localPosition + new Vector3(-slideDistance, 0, 0), animDuration).SetEase(easeType);

        // if(useFloatAnimation){

        //     //transform.localPosition = originalPosition - new Vector3(0, -floatDistance, 0);
        //     transform.DOLocalMove(rectTransform.localPosition + new Vector3(0, -floatDistance, 0), animDuration).SetEase(easeType);
        // } 

        if(useScaleAnimation) transform.DOScale(startScale, animDuration).SetEase(easeType);

        canvasGroup.DOFade(0, animDuration).SetEase(easeType)
            .OnComplete(() =>
            {
                if(shouldFadeOutAfterDisplay){

                    OnComplete?.Invoke();
                    gameObject.SetActive(false); 
                }   
                
            });
    }

    void Reset()
    {
        DOTween.Kill(this);
        gameObject.SetActive(false);
    }
}

