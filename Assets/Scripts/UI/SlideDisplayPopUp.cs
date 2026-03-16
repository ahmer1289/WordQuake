using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SlideDisplayPopUp : MonoBehaviour
{
    
    public Action hasCompletedPopUp;
    WQPopupAnimator wqPopupAnimator;

    [Space, SerializeField] GameObject m_GreenTextContent;
    [SerializeField] TMP_Text m_GreenWordText;
    [SerializeField] Transform m_PointsListContainer;
    [SerializeField] TMP_Text m_PointsListTextPrefab;
    [SerializeField] float m_HideDelayForGreen;
    [Space, SerializeField] GameObject m_RedTextContent;
    [SerializeField] TMP_Text m_RedWordText;
    [SerializeField] TMP_Text m_FeedbackText;
    [SerializeField] float m_HideDelayForRed;
    bool isValidWord;
    WQPointSystem wQPointSystem;

    void Awake(){

        wqPopupAnimator = GetComponentInChildren<WQPopupAnimator>();
    }

    void Start()
    {
        Reset();

        wQPointSystem = WQPointSystem.Instance;
    }

    void OnEnable()
    {
        wqPopupAnimator.OnComplete += HandlePopUpCompletion;
    }
    void OnDisable()
    {
        wqPopupAnimator.OnComplete -= HandlePopUpCompletion;
    }

    public void SetWordText(string input){

        wqPopupAnimator.SetHideDelay(m_HideDelayForGreen);

        m_GreenWordText.text = input;
        SetPointsText(wQPointSystem.m_AwardedPointsList);
        
        gameObject.SetActive(true);

        m_GreenTextContent.SetActive(true);
        m_RedTextContent.SetActive(false);

        isValidWord = true;
    }

    public void SetFeedBackText(string word, string errorMessage){

        wqPopupAnimator.SetHideDelay(m_HideDelayForRed);

        m_RedWordText.text = word;

        m_FeedbackText.text = errorMessage;

        gameObject.SetActive(true);

        m_GreenTextContent.SetActive(false);
        m_RedTextContent.SetActive(true);

        isValidWord = false;
    }

    void SetPointsText(List<string> inputs){

        for (int i = 0; i < inputs.Count; i++)
        {
            TMP_Text pointText = Instantiate(m_PointsListTextPrefab, m_PointsListContainer);
            WQPopupAnimator pointTextAnimator = m_PointsListTextPrefab.GetComponent<WQPopupAnimator>();

            pointTextAnimator.SetStartDelay(i * 0.25f);
            pointText.text = inputs[i];
            pointText.gameObject.SetActive(true);
        }
    }

    void HandlePopUpCompletion(){

        if(isValidWord) WQEventSystem.TriggerEvent("WordPopUpAnimationComplete");
        
        gameObject.SetActive(false);
        Reset();
    }

    void Reset(){

        m_GreenTextContent.SetActive(false);
        m_RedTextContent.SetActive(false);

        foreach (Transform child in m_PointsListContainer){

            Destroy(child.gameObject);
        }
    }
}
