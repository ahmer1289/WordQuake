// /* Utility Examples
//    version 1.0 - Jan 31st, 2025
//    Copyright (C) CheddarCat, WordQuake.
// */

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Examples : MonoBehaviour
// {
    
//     [Space, SerializeField] private string wordToCheck;
//     [Space, SerializeField] private WQTheme themeToCheck;
//     private bool isCheckingWord = false;
    
//     void Start()
//     {
//         wordToCheck = wordToCheck.ToLower().Trim();
//         StartCoroutine(CheckIfWordExists());
//     }

//     [ContextMenu("Quick Test!")]
//     void QuickTest(){
        
//         Start();
//     }

//     IEnumerator CheckIfWordExists()
//     {
//         isCheckingWord = true; //check begins

//         wordToCheck = wordToCheck.ToLower().Trim();

//         WordValidatorAPI.Instance.CheckIfWordExists(wordToCheck, 
        
//         (wordExists) =>
//         {
//             if (wordExists)
//             {
//                 Debug.Log($"<color=orange>'{wordToCheck}' exists in the English Dictionary!</color>");
//             }
//             else
//             {
//                 Debug.Log($"<color=#FB5353FF>'{wordToCheck}' is NOT a real word.</color>");
//             }
//         },
        
//         (wordIsPlural) => 
//         {
//             if (wordIsPlural)
//             {
//                 Debug.Log($"<color=#FB5353FF>'{wordToCheck}' is a plural word!</color>");
//             }

//             else
//             {
//                 Debug.Log($"<color=#61FF7EFF>'{wordToCheck}' is not a plural word!</color>");
//             }

//             isCheckingWord = false; // check ends here 👇
//         });
        
//         // waiting until the check is completed
//         yield return new WaitUntil(() => !isCheckingWord);

//         //? check ends below this line too 👇
//         Debug.Log($"<b>Done with API check!</b>");

//         CheckIfSpecialWordExistsInTheme();
//         GetSpecialWordCategory();

//     }

//     void CheckIfSpecialWordExistsInTheme(){

//         bool existsInTheme = WordValidatorAPI.Instance.CheckIfWordExistsInTheme(wordToCheck, themeToCheck);
//         Debug.Log($"<color=#FF69B4>Does '{wordToCheck}' exist in '{themeToCheck.ThemeName}'? {existsInTheme}</color>");
//     }
// }
