using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WQPointSystem : MonoBehaviour
{
    public static WQPointSystem Instance { get; private set; }
    public List<string> m_AwardedPointsList = new List<string>();

    [Header("Shiritori Scoring")]
    [SerializeField] int shiritoriPoints = 50;
    [SerializeField] int shiritoriPenalty = -10;

    [Header("Word Length Bonuses")]
    [SerializeField] int wordBonus1to3 = 10;
    [SerializeField] int wordBonus4to6 = 20;
    [SerializeField] int wordBonus7to9 = 40;
    [SerializeField] int wordBonus10Plus = 100;

    [Header("Time Remaining Bonuses")]
    [SerializeField] int timeRemainingBonus0to2 = 5;
    [SerializeField] int timeRemainingBonus2to5 = 15;
    [SerializeField] int timeRemainingBonus5to8 = 30;
    [SerializeField] int timeRemainingBonus8to10 = 50;

    [Space] public int m_TotalPointsToBeAwarded = 0;
    
    void Awake(){
        
        Instance = this;
    }

    public void AwardShiritoriLogicPoints(){

        AddPoints(shiritoriPoints);
        m_AwardedPointsList.Add($"+ {shiritoriPoints} Shiritori Logic Bonus");
    }

    public void AwardTimeBonusPoints(float timeRemaining)
    {
        int awardedTimeBonus = 0;

        if (timeRemaining <= 2)
        {
            awardedTimeBonus = timeRemainingBonus0to2;
        }
        else if (timeRemaining <= 5)
        {
            awardedTimeBonus = timeRemainingBonus2to5;
        }
        else if (timeRemaining <= 8)
        {
            awardedTimeBonus = timeRemainingBonus5to8;
        }
        else if (timeRemaining <= 10)
        {
            awardedTimeBonus = timeRemainingBonus8to10;
        }

        AddPoints(awardedTimeBonus);
        m_AwardedPointsList.Add($"+ {awardedTimeBonus} Time Bonus");
    }

    public void AwardWordLengthBonusPoints(string word)
    {
        int wordLength = word.Length;
        int awardedWordBonus;

        if (wordLength <= 3)
        {
            awardedWordBonus = wordBonus1to3;
        }
        else if (wordLength <= 6)
        {
            awardedWordBonus = wordBonus4to6;
        }
        else if (wordLength <= 9)
        {
            awardedWordBonus = wordBonus7to9;
        }
        else
        {
            awardedWordBonus = wordBonus10Plus;
        }

        AddPoints(awardedWordBonus);
        m_AwardedPointsList.Add($"+ {awardedWordBonus} Word Length Bonus");
    }

    public void AddPoints(int val){

        m_TotalPointsToBeAwarded += val;
    }
    
    public void ResetPoints(){

        m_AwardedPointsList.Clear();
        m_TotalPointsToBeAwarded = 0;
    }
}
