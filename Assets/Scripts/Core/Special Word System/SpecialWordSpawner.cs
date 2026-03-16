using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWordSpawner : MonoBehaviour
{

    public static SpecialWordSpawner Instance;
    [SerializeField] SpecialWordDatabase wordDatabase;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        wordDatabase.Initialize();
    }

    public void SpawnWord(string inputWord, int playerIndex)
    {

        GameObject wQSpecialWordObject;
        WQSpecialWordEffect wQSpecialWordEffect;
        WQSpecialWord specialWordAsset;
        Transform spawnPoint;
        float specialWordLifeTime;
        
        specialWordAsset = wordDatabase.GetWord(inputWord);

        spawnPoint = WQPlayerSpawner.Instance.GetWQPlayerSpecialWordReceiver(playerIndex).throwTargetLR;

        if (specialWordAsset != null)
        {
            specialWordLifeTime = specialWordAsset.m_LifeTime;

            wQSpecialWordObject = Instantiate(specialWordAsset.specialWordPrefab, spawnPoint);

            wQSpecialWordEffect = wQSpecialWordObject.GetComponent<WQSpecialWordEffect>();

            wQSpecialWordEffect.isPlayer1 = playerIndex == 1;
            wQSpecialWordEffect.m_Force = specialWordAsset.m_Force;
            wQSpecialWordEffect.m_LifeTime = specialWordAsset.m_LifeTime;
            wQSpecialWordEffect.Init();

            Debug.Log($"<color=#5EFF69FF> Spawning {inputWord} object</color>");

            GameManager.Instance.SwitchTurnWithDelay(specialWordLifeTime); // to switch turns after the special word's lifetime has elapsed
        }

        else{

            Debug.LogWarning($"No spawnable object found for word: {inputWord}");
        }
    }
}
