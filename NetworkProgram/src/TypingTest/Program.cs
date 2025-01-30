using System.Net;
using SplashKitSDK;

namespace TypingTest;

public static class Program
{
    private static Font _customFont;
    
    static void Main()
    {
        // download the font at runtime...
        string fontPath = DownloadFont("https://fonts.gstatic.com/s/opensans/v40/memSYaGs126MiZpBA-UvWbX2vVnXBbObj2OVZyOOSr4dVJWUgsjZ0C4n.ttf", "OpenSans-Regular.ttf");

        // load the downloaded font...
        _customFont = SplashKit.LoadFont("Roboto", fontPath);

        // initialise the game runner and window...
        GameRunner runner = new GameRunner(_customFont);
        SplashKit.OpenWindow("Typing Speed Trainer", 800, 600);

        // execute game loop until exit condition met...
        while (!SplashKit.WindowCloseRequested("Typing Speed Trainer"))
        {
            SplashKit.ProcessEvents();
            SplashKit.ClearScreen(Color.White);

            // show landing page on startup...
            if (!runner.GameStarted)
            {
                SplashKit.DrawText("Press Enter to Start!", Color.Black, 320, 290);
                
                // render game result when returning to landing page from active game...
                if (runner.CorrectWords > 0)
                    SplashKit.DrawText($"Score: {runner.CheckScore()} wpm @ {Math.Round(runner.CheckAccuracy(),2)}% accuracy", Color.Black, _customFont, 18, 10, 10);

                // start new game on return (enter)...
                if (SplashKit.KeyTyped(KeyCode.ReturnKey))
                {
                    runner.StartGame();
                }
            }
            else // game is running...
            {
                runner.DisplayGame();
                runner.HandleTypingInput();
                runner.CheckEndGameConditions();
            }
            SplashKit.RefreshScreen(60);
        }

        // exit condition met. Shutdown mock server prior to terminating... 
        var client = new WordApiClient();
        client.Shutdown();
    }
    
    /**
     * <summary>Downloads a font file from a specified location (URL) and places in Resources dir</summary>
     * <returns>a string corresponding with the filepath to the font</returns>
     */
    static string DownloadFont(string url, string fileName)
    {
        string fontFolder = Path.Combine(Environment.CurrentDirectory, "Resources", "Fonts");
        Directory.CreateDirectory(fontFolder);
        string fontPath = Path.Combine(fontFolder, fileName);

        if (!File.Exists(fontPath))
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, fontPath);
            }
        }
        return fontPath;
    }
}
