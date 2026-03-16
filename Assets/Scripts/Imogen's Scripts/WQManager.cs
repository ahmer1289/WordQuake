using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class WQManager : MonoBehaviour
{
    public static WQManager Instance { get; private set; }
    public TMP_Text previousWordUsedText; //text object to show the previous word used on the UI
    public TMP_Text themeText;
    public TMP_Text ThemeReveal;
    private List<string> usedWords = new List<string>(); //list to store all the words used by p1
    public TMP_Text wordHistoryText;

    private string currentWord; //string variable to store the game's current word (this will be reset every turn)
    private string previousWord; //string variable to store the previous word used (this will also reset every turn)
    private string errorMessage;
    public WordValidatorAPI wordValidatorAPI;
    
    private bool isCheckingWord = false;
    private string word;
    private WQTheme currentTheme = null;
    private bool validAPIWord;
    private bool validLetter = true;
    private bool validTheme;
    private bool validUnused;
    private bool validWordBool;
    private string validWord;

    //Managers
    private ThemedWordsLoader themedWordsLoader;
    private WQThemes themesManager;
    private GameManager gameManager;

    private HashSet<string> easterEggWords = new HashSet<string>() //set of easter egg words
    {
        "blockparty", "gravriders", "chessrancher", "ascension"
    };

    void Awake(){
        
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        themesManager = WQThemes.Instance;
        gameManager = GameManager.Instance;
        themedWordsLoader = ThemedWordsLoader.Instance;

        usedWords.Clear(); //clear the usedWords dictionary upon starting

        
        CheckLobbyTheme();
    }

    //INPUT METHOD----------------------------------------
    public void ReadPlayerInput(string input)
    {

        if (string.IsNullOrWhiteSpace(input))
        {
            SetErrorText("Empty or invalid input");
            return;
        }

        currentWord = input; 

        StartCoroutine(WordValidationAPI()); 
    }

    //SET UI TEXT METHOD-----------------------------------
    public void SetFirstWordText(string validWord)
    {
        previousWordUsedText.text = validWord; //sets previous word used text object to display player input
        DisplayPreviousWord();
        Debug.Log("Previous Word is " + previousWord);
        UpdateWordHistoryUI();
    }

    public void SetPreviousWordText(string previousWord)
    {
        previousWordUsedText.text = previousWord; //sets previous word used text object to display player input
        DisplayPreviousWord();
        Debug.Log("Previous Word is " + previousWord);

        UpdateWordHistoryUI();
    }

    //DISPLAY PREVIOUS WORD
    public void DisplayPreviousWord()
    {
        previousWordUsedText.gameObject.SetActive(true);
    }

    //SET THEME TEXT METHOD
    public void SetRandomTheme()
    {
        SetNewTheme(themesManager.GetRandomWQTheme());
    }
    public void CheckLobbyTheme()
    {
        WQTheme currentWQTheme = themesManager.GetCurrentWQTheme();

        if (currentWQTheme != null)
        {
            SetNewTheme(currentWQTheme); 
        }
        else
        {
            SetRandomTheme(); // If no theme is set, pick a random one
        }
    }
    public void SetNewTheme(WQTheme newTheme){

        currentTheme = newTheme;
        ThemeReveal.text = currentTheme.ThemeName;
        themeText.text = currentTheme.ThemeName; 

        gameManager.turnTimer.ResetTimer();
    }


    #region Word Validation (Local and API Validation)

    IEnumerator WordValidationAPI()
    {
        isCheckingWord = true; //check begins
        word = currentWord;

        if (easterEggWords.Contains(word.ToLower())) //skip dictionary validation for easter egg words
        {   
            if(previousWord != null && CompareStrings(word, previousWord)){

                HandleValidShiritoriLogicWord();
            }
            
            else{

                HandleValidWord();
            }   
            
            yield break; // reset and exit coroutine
        }

        //? local check first

        if (CheckIfWordExistsLocally(word))
        {   

            //TODO: Playtesting purposes
            SaveWordsFromPlayTest.Instance.TrackWord(word);

            if (CheckIfWordExistsInThemeLocally(word))
            {
                if (CheckIfWordCanBeUsed(word))
                {

                    if (previousWord != null && CompareStrings(word, previousWord))
                    {
                        HandleValidShiritoriLogicWord();
                        yield break; // reset and exit coroutine
                    }

                    HandleValidWord();
                    yield break;
                }
                else
                {
                    SetErrorText("This word has already been used.");
                    yield break; // reset and exit coroutine
                }
            }
            else
            {
                SetErrorText("This word does not fit the theme.");
                yield break; // reset and exit coroutine
            }
        }

        //? word does not exist locally
        //? API Pathway then

        WordValidatorAPI.Instance.CheckIfWordExists(word, (wordExists) =>
        {
            isCheckingWord = false;  // Check ends here 👇

            if (wordExists)
            {   
                //TODO: Playtesting purposes
                SaveWordsFromPlayTest.Instance.TrackWord(word);
                validAPIWord = true;
                APIValidation();
            }
            else
            {
                SetErrorText("This word is not in our dictionary.");
            }

        }, (wordIsPlural) =>
        {
            if (wordIsPlural)
            {
                validWordBool = false;
                SetErrorText("Plurals can't be used.");
            }

        }, themesManager.IsInCountryTheme());

        // Wait until the check is completed
        while (isCheckingWord)
        {
            Debug.Log("Checking...");
            yield return null;  // Wait for API response
        }

        Debug.Log("Done with check");
    }

    private void HandleValidShiritoriLogicWord()
    {
        WQPointSystem.Instance.AwardShiritoriLogicPoints();

        HandleValidWord();
    }

    private void HandleValidWord()
    {
        validWordBool = true;
        gameManager.HandleWordIsValid(word);
        UpdateWordHistory();
        isCheckingWord = false;
    }

    private void UpdateWordHistory()
    {
        usedWords.Add(currentWord);
        previousWord = currentWord;
        currentWord = null;  // Reset after processing
        SetPreviousWordText(previousWord);
    }

    private void APIValidation()
    {
        if (CheckIfWordExistsInThemeWithAPI() && CheckIfWordCanBeUsed(currentWord))
        {
            if (previousWord != null && CompareStrings(word, previousWord))
            {
                HandleValidShiritoriLogicWord();
                return;
            }

            HandleValidWord();
        }
        else
        {
            DisplayErrorMessage();
        }
    }

    #endregion


    // Utilities
    bool CheckIfWordExistsLocally(string word){

        return themedWordsLoader.CheckIfWordExistsLocally(word);
    }

    bool CheckIfWordExistsInThemeLocally(string word){

        return themedWordsLoader.CheckIfWordExistsInThemeLocally(word, currentTheme);
    }

    bool CompareStrings(string current, string previous) //method compares the two strings to check the last letter of previous is the same as first letter of current
    {
        char lastChar = char.ToLower(previous[previous.Length - 1]); //char variable to store previous word's last letter (puts in lowercase)
        char firstChar = char.ToLower(current[0]); //char variable to store current word's first letter (puts in lowercase)

        //if last letter of previous == first letter of current
        if (lastChar == firstChar)
        {
            Debug.Log("The first letter of the word matched the previous last letter.");
            validLetter = true;
            return true;
        }
        else
        {
            Debug.Log("The first letter of the word did not match the previous last letter.");
            validLetter = false; // Set false, but don't display error here
            return false;
        }

    }

    bool CheckIfWordCanBeUsed(string current) //method checks the word inputted has not already been used
    {
        if (usedWords.Contains(current))
        {
            Debug.Log("The word already exists in the stored words dictionary.");
            validUnused = false; // Set false, but don't display error here
            return false;
        }

        else
        {
            Debug.Log("The word has not yet been used.");
            validUnused = true;
            return true; //if it does not exist in the list, it hasn't been used yet
        }
    }

    bool CheckIfWordExistsInThemeWithAPI()
    {

        if (easterEggWords.Contains(word.ToLower())) //skip theme check for easter egg words
        {
            validTheme = true;
        }

        else
        {
            bool existsInTheme = wordValidatorAPI.CheckIfWordExistsInTheme(word, currentTheme);

            if (!existsInTheme)
            {
                validTheme = false;
                errorMessage = "This word does not fit the theme.";
                SetErrorText(errorMessage);
            }
            else
            {
                validTheme = true;
            }
        }

        return validTheme;
    }

    void SetErrorText(string errorMessage)
    {
        gameManager.HandleWordError(currentWord, errorMessage);
        gameManager.turnTimer.ContinueTimer();
    }

    void DisplayErrorMessage()
    {
        if (!validAPIWord)
        {
            errorMessage = "This word is not in our dictionary.";
        }
        else if (!validLetter)
        {
            errorMessage = "This word does not begin with the correct letter.";
        }
        else if (!validTheme)
        {
            errorMessage = "This word does not fit the theme.";
        }
        else if (!validUnused)
        {
            errorMessage = "This word has already been used.";
        }

        SetErrorText(errorMessage);
    }

    void UpdateWordHistoryUI()
    {
        wordHistoryText.text = ""; // Clear previous text

        foreach (string word in usedWords)
        {
            wordHistoryText.text += word + "\n"; // Append each word on a new line
        }
    }

    
    #region WQManager linked Abilities

    /// <summary>
    /// Suggests a word based on theme. If a previous word exists, it suggests a related word using the CompareStrings(). If not, it suggests any random word in theme.
    /// </summary>
    public string GetSuggestableWord(){
        
        List<string> m_ThemedWords = themedWordsLoader.GetAllWordsInTheme(currentTheme);
        string suggestableWord = null;

        if(previousWord != null){ 
            
            List<string> m_SuggestableWords = new List<string>();

            foreach (var word in m_ThemedWords)
            {
                m_SuggestableWords.Add(word);
                // if(CompareStrings(word, previousWord)){

                //     m_SuggestableWords.Add(word);
                // }
            }
            
            if (m_SuggestableWords.Count > 0){

                suggestableWord = GetRandomStringFromList(m_SuggestableWords);
                Debug.Log($"<color=#5ED4FFFF> Here's a word you can use: '{suggestableWord}' </color>");
            }

            else{

                Debug.Log("<color=#FF5E5EFF> Can't suggest any word, sorry! </color>");
            }
        }

        else{ //? Random word in theme. 

            suggestableWord = GetRandomStringFromList(m_ThemedWords);
            Debug.Log($"<color=#5ED4FFFF> Here's a word you can use: '{suggestableWord}' </color>");
        }

        return suggestableWord;
    }

    /// <summary>
    /// Simple method to return a random word from a list.
    /// </summary>
    string GetRandomStringFromList(List<string> refList){

        return refList[UnityEngine.Random.Range(0, refList.Count)];
    }

    #endregion

}
