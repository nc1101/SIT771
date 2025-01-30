using SplashKitSDK;

namespace TypingTest;

public class GameRunner
{
    private const int Timeout = 30;
    
    private Font _customFont;
    private List<string> _words;
    private Random _randomGen;
    private string _currentWord;
    private string _typedWord;
    private int _incorrectWords;
    private DateTime _startTime;

    public bool GameStarted { get; private set; }
    public int CorrectWords { get; private set; }


    public GameRunner(Font customFont)
    {
        // retrieve word list from mock server (words API)...
        var client = new WordApiClient();
        _words = client.GetWords();
        Console.WriteLine($"Words list successfully initialised: {_words.Count}");
        
        _customFont = customFont;
        _randomGen = new Random();
        _currentWord = "";
    }
    
    /**
     * <summary>Initiates the game loop</summary>
     */
    public void StartGame()
    {
        GameStarted = true;
        _startTime = DateTime.Now;
        CorrectWords = 0;
        _incorrectWords = 0;
        _typedWord = "";
        GenerateNewWord();
    }
    
    /**
     * <summary>Retrieves a random word from the word list and assigns to variable rendered to screen</summary>
     */
    private void GenerateNewWord()
    {
        _currentWord = _words[_randomGen.Next(_words.Count)];
    }

    /**
     * <summary>Renders the game artefacts to the screen</summary>
     */
    public void DisplayGame()
    {
        // render a timer a word counter...
        SplashKit.DrawText("Time: " + TimeElapsed() + "s", Color.Black, _customFont, 18, 10, 10);
        SplashKit.DrawText("Correct Words: " + CorrectWords, Color.Black, _customFont, 18, 10, 40);
        SplashKit.DrawText("Incorrect Words: " + _incorrectWords, Color.Black, _customFont, 18, 10, 70);
        
        SplashKit.DrawText("Type the Word:", Color.Black, _customFont, 24, 300, 200);
        SplashKit.DrawText(_currentWord, Color.Blue, _customFont, 24, 300, 240); // the word to be typed...
        SplashKit.DrawText(_typedWord, Color.Green, _customFont, 24, 300, 280); // the user's input...
    }

    /**
     * <summary>Handles user input events during game execution</summary>
     */
    public void HandleTypingInput()
    {
        // determine the key pressed...
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (SplashKit.KeyTyped(key))
            {
                if (key == KeyCode.BackspaceKey && _typedWord.Length > 0)
                {
                    // handle backspace input...
                    _typedWord = _typedWord.Substring(0, _typedWord.Length - 1);
                }
                else
                {
                    string keyName = SplashKit.KeyName(key);
                    
                    // handle alphanumeric input...
                    if (keyName.Length == 1 && char.IsLetterOrDigit(keyName[0]))
                    {
                        _typedWord += char.ToLower(keyName[0]);
                    }
                    else if (key == KeyCode.SpaceKey) // handle spacebar input...
                    {
                        _typedWord += ' ';
                    }
                }
            }
        }

        // on return (enter) key input, check typed word against prompted word to determine whether correct/incorrect...
        if (SplashKit.KeyTyped(KeyCode.ReturnKey))
        {
            CheckWord();
        }
    }

    /**
     * <summary>Checks whether the word entered matches the word prompt and increments appropriate count</summary>
     */
    private void CheckWord()
    {
        if (_typedWord.Equals(_currentWord, StringComparison.OrdinalIgnoreCase))
            CorrectWords++;
        else
            _incorrectWords++;
        
        _typedWord = "";
        GenerateNewWord();
    }

    /**
     * <summary>Checks whether the game has reached its timeout threshold and stops the game loop</summary>
     */
    public void CheckEndGameConditions()
    {
        if (TimeElapsed() == Timeout)
        {
            GameStarted = false;
            string gameResult = $"Correct words: {CorrectWords}\nIncorrect words: {_incorrectWords}\nWPM: {CheckScore()}\nAccuracy: {Math.Round(CheckAccuracy(), 2)}";
            Console.WriteLine($"Game Over! {gameResult}");
            
            PostToLeaderboard(gameResult);
        }
    }

    /**
     * <summary>Determines a 'words per minute' score from the words entered</summary>
     * <returns>the calculated WPM score</returns>
     */
    public int CheckScore()
    {
        return (CorrectWords + _incorrectWords) * (60 / Timeout);
    }

    /**
     * <summary>Determines an accuracy score based on the proportion of correct words</summary>
     * <returns>the calculated accuracy score</returns>
     */
    public double CheckAccuracy()
    {
        int totalWords = CorrectWords + _incorrectWords;
        return totalWords > 0 ? (double)CorrectWords / totalWords * 100 : 0;
    }

    /**
     * <summary>Determines how much time has passed since game loop start</summary>
     * <returns>the time elapsed (in seconds)</returns>
     */
    private int TimeElapsed()
    {
        return (int)(DateTime.Now - _startTime).TotalSeconds;
    }

    /**
     * <summary>Posts the game result to the mock server (leaderboard API)</summary>
     */
    private void PostToLeaderboard(string gameResult)
    {
        var client = new WordApiClient();
        client.PostToLeaderboard(gameResult);
    }
}