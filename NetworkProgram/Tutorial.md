# Tutorial - Using SplashKit networking to build a game server and typing test game
## Background
This tutorial leverages SplashKit's networking capabilities to illustrate how both sides of the HTTP client-server relationship work and provides an example of a standardised way to interface with RESTful APIs to extend the capabilities of a program. 
## Objectives
This tutorial will demonstrate:
- how to implement a basic REST API server program, which supports HTTP GET and POST requests
- how to implement an abstract API Connector and derived client to interface with the server
- how to leveage the services provided by the server to produce a typing test game

The following diagram depicts the programs and classes implemented in this tutorial. 

<img width="1246" alt="Screenshot 2025-01-30 at 3 10 15 PM" src="https://github.com/user-attachments/assets/142c4c11-a963-4540-80f8-0cb0af8bd7d4" />

## Tutorial - PART 1
This section of the tutorial will focus on implementation of our game server, which will run independently of (and handle requests from) the typing test program. Given the context of our client application, the game server will support the following methods:
- a `words` endpoint to return a list of words to be used in the game
- a `leaderboard` endpoint to simulate the posting of game results to a fictional leaderboard
- a `shutdown` endpoint to allow for the server to be terminated on game shutdown

### 1. Create a new `GameServer` project
The first step is to create a GameServer directory in your workspace and initialise a new project by navigating to the directory via the terminal and executing the `skm dotnet new console` and `skm dotnet restore` commands.

### 2. Create a field to accommodate the word list
The `words` endpoint will return a list of words to be used in the typing test game. For simplicity, the server will maintain a static list of arbitrary words within its program class. 

```C#
public class Program
{
  // initialise WordList with whatever words you like (minimum 25 suggested)...
  private static readonly List<string> WordList = [ "Add", "Random", "Words" ];
}
```

### 3. Log startup message to console
It's useful to see that the server has started up successfully so a meaningful startup message can be logged to the console via the program's `Main` method.

```C#
public static void Main()
{
  Console.WriteLine("Starting Mock API Server at http://localhost:8080...");
}
```

When running, the server will only be accessible from the local machine, hence the `http://localhost` URL. Further context for the port reference (8080) is provided in step 4 below.

### 4. Initialise the web server and HTTP request objects
SplashKit provides native classes for HTTP networking, which abstract the core implementation logic and allow us to focus on the key operations to be supported by our game server. `WebServer` and `HttpRequest` objects can be declared below the startup message added in step 3.

```C#
public static void Main()
{
  // declare and initalise a SplashKit web server...
  // ------------------------------------------------------------------------
  // NOTE: the 8080 parameter reflects the port HTTP requests will be routed to.
  // The WebServer class defaults to 8080 where no port is provided, however,
  // it is explicitly passed here to enhance readability and understanding...
  // ------------------------------------------------------------------------
  WebServer server = new WebServer(8080);

  // add an HttpRequest variable to function as a placeholder for incoming requests...
  HttpRequest request; 
}
```

### 5. Handle incoming requests
Once the server has started up, it will listen for HTTP requests on port 8080. When a request is received, the server must explicitly access it before it can be routed and fulfilled.

```C#
public static void Main()
{
  // previous code here...

  // direct the server object to retrieve the next queued request...
  request = server.NextWebRequest;
}
```

Once we have the request object, we can then determine how it should be routed based on its type (GET or POST in our context) and path. The specific routes (paths) to be supported will be highlighted in subsequent steps; however, to ensure our server continues to service incoming requests until explicitly terminated, we will enclose our routing logic inside a `while` loop, which continues to execute until a request to `shutdown` is received.

```C#
public static void Main()
{
  // previous code here...
  
  while (!request.IsGetRequestFor("/api/shutdown")) {
    // routing to occur here...
  
    // direct the server object to retrieve next request before loop restarts...
    // ------------------------------------------------------------------------
    // NOTE: failure to do this will result in errors arising from the program
    // attempting to process the same request on each loop execution.
    // ------------------------------------------------------------------------
    request = server.NextWebRequest
  }
  
  // if we make it here, it means a 'shutdown' request was received.
  // Log the request and direct the server to stop running...
  Console.WriteLine("Shutting down Mock API Server...");
  server.Stop();

} // end of Main()...
```

### 6. Implement 'words' endpoint
Now our server program is setup to handle incoming HTTP requests, we can implement the endpoints needed by our typing test application. As highlighted in step 1, the first of these endpoints is `words`. When called, this endpoint will simply return a list of words to be used in the game. As this is a simple request for information (data), it can be handled via the HTTP GET request type.

SplashKit's `HttpRequest` class includes built-in methods, which can be leveraged to check the request type and path (endpoint) of an incoming request (see below). This check is to occur inside the `while` loop defined in the previous step. SplashKit's built in `Json` classes and methods can be leveraged to provide a structured response.

```C#
while (!request.IsGetRequestFor("/api/shutdown")) {

  // use SplashKit's built-in HttpRequest method to check whether the request
  // is of type GET and targeted at the 'words' endpoint...
  if (request.IsGetRequestFor("/api/words")) // fully qualified path: http://localhost:8080/api/words...
  {
    // declare and initialise a Json response object...
    Json response = SplashKit.CreateJson();

    // use SplashKit's built-in utility method to add a 'words' array to the response object
    // and populate from our static WordList...
    SplashKit.JsonSetArray(response, "words", WordList);

    // tell SplashKit to send the response to the requesting client...
    SplashKit.SendResponse(request, response);
  }
}
```

The server will now route GET requests made on the 'words' endpoint and respond accordingly with a Json object comprising the list of supported words. This can be tested by running the program and navigating to http://localhost:8080/api/words in your browser.

### 7. Implement 'leaderboard' endpoint
The second endpoint needed by the typing test application is `leaderboard`. Unlike `words`, this endpoint expects game result data to be posted from the typing test application and retained by the server. The HTTP POST request type is, therefore, appropriate.

Given data storage is outside the scope of this tutorial, we will simulate the operation by logging the posted result to the server console.

```C#
while (!request.IsGetRequestFor("/api/shutdown")) {

  if (request.IsGetRequestFor("/api/words")) {
    // see step 6...
  }
  else if (request.IsPostRequestFor("/api/leaderboard")) // fully qualified path: http://localhost:8080/api/leaderboard...
  {
    // simulate leaderboard update by logging the posted result to the server console...
    Console.WriteLine(request.Body);

    // tell SplashKit to send a response indicating success to the requesting client...
    SplashKit.SendResponse(request, "OK");
  }
}
```

We now have a functional server for our typing test game!

## Tutorial - PART 2
This section of the tutorial will focus on implementation of the Typing Test game, including a client to interface with our game server from Part 1. The basic game sequence will be as follows:
- on startup, poll the game server to get the words list
- while the typing test is active, render a random word from the list to screen for the user to enter. Match user entry to rendered word to determine whether correct or incorrect
- once the game timer runs out, calculate the result (words per minute and accuracy) and post to the mock server (leaderboard)

### 1. Create a new `TypingTest` project
The first step is to create a TypingTest directory in your workspace and initialise a new project by navigating to the directory via the terminal and executing the `skm dotnet new console` and `skm dotnet restore` commands.

### 2. Create an abstract API connector class
As our game server supports standard HTTP requests, it is possible to define an abstract connector to provide standard fields and methods that can be reused by any client.

Start by creating a new abstract class in the project's root directory.

```C#
public abstract class ApiConnector
{
  // class definition here...
}
```

With the class established, it can now be populated with the fields and methods necessary to support interactions with our server. As with the server code discussed in Part 1, the internal complexity of the operations are abstracted by SplashKit's native HTTP classes and utility methods.

```C#
public abstract class ApiConnector
{
  // as discussed in Part 1, our server will route HTTP requests to port 8080.
  // As this value will never change, declare and initialise a class constant...
  private const ushort Port = 8080;

  // declare an abstract property to store the server URL...
  // the protected scope ensures access is restricted to derived classes only...
  protected abstract string BaseUrl { get; }

  // create a protected method to facilitate submission of GET requests...
  //    the method should return an HttpResponse object and take a string parameter for the endpoint...
  protected HttpResponse Get(string endpoint)
  {
    // network interactions can trigger exceptions so enclose the method logic in a try..catch block...
    try {
      // declare a local variable to store the fully qualified request path...
      string path = $"{BaseUrl}{endpoint}";

      // log an entry to the console for easy tracability...
      Log($"Sending GET request to {path}");

      // tell SplashKit to submit the GET request and return the result...
      return SplashKit.HttpGet(path, Port);
    }
    catch (Exception e)
    {
      // where an error is encountered, log a message and return null...
      Log($"Error during GET request: {e.Message}");
      return null;
    }
  }

  // create a protected method to facilitate submission of POST requests...
  //    the method should return an HttpResponse object and take two string parameters for the
  //    endpoint and messageBody (data to be posted)...
  protected HttpResponse Post(string endpoint, string messageBody)
  {
    try
    {
      // declare a local variable to store the fully qualified request path...
      string path = $"{BaseUrl}{endpoint}";

      // log an entry to the console for easy tracability...
      Log($"Sending POST request to {path}");

      // tell SplashKit to submit the POST request and return the result...
      return SplashKit.HttpPost(path, Port, messageBody);
    }
    catch (Exception e)
    {
      // where an error is encountered, log a message and return null...
      Log($"Error during POST request: {e.Message}");
      return null;
    }
  }

  // create a utility method (invoked by other class methods) to write timestamped messages to the console...
  //    the method does not need to return anything but takes a string parameter for the message...
  protected void Log(string message)
  {
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {message}");
  }
}
```

### 3. Create an API client from the base connector class
The abstract class created in step 2 can now be used to establish a derived class to function as a program-specific client. Start by creating a new class in the project's root directory and extending from the `ApiConnector` class.

```C#
public class WordApiClient : ApiConnector
{
  // class definition here...
}
```

With the class established, it can now be populated with the fields and methods necessary to handle the specific needs of the typing test game. SplashKit's HTTP classes and methods are again leveraged as required.

```C#
public class WordApiClient : ApiConnector
{
  // declare a constant to store the game server URL...
  private const string Url = "http://localhost:8080/api/";

  // declare constants to store the methods (endpoints) supported by the game server...
  private const string WordEndpoint = "words";
  private const string LeaderboardEndpoint = "leaderboard";
  private const string ShutdownEndpoint = "shutdown";

  // declare a static list of words to be used by the game in the event the server cannot be reached...
  // initialise with whatever words you like (minimum 25 suggested)...
  private readonly List<string> BackupWords = ["Add", "Random", "Words" ];

  // override the base class property with the stored server URL...
  protected override string BaseUrl => Url;

  // create a method to poll the game server for a list of words and handle the response
  //    the method should return a simple list of words...
  public List<string> GetWords()
  {
    // declare an HttpResponse variable and invoke the base Get method to retrieve the word list...
    HttpResponse response = base.Get(WordEndpoint);

    // enclose subsequent logic in a try..catch block to ensure proper handling of exceptions...
    try
    {
        // tell SplashKit to convert the response to a string and then a Json object...
        string strResponse = SplashKit.HttpResponseToString(response);
        Json jsonResponse = SplashKit.JsonFromString(strResponse);
        SplashKit.FreeResponse(response); // tell SplashKit to release the response resource...

        // use SplashKit's built-in utility to convert response from JSON array to List...
        List<string> words = new List<string>();
        SplashKit.JsonReadArray(jsonResponse, "words", ref words);

        // return the list of words from the server...
        return words;
    }
    catch (Exception e)
    {
        // where an error is encountered, log a message and return backup list...
        base.Log($"Error parsing JSON response: {e.Message}");
        return BackupWords;
    }
  }

  // create a method to post a game result to the server and handle the response
  //    the method does not need to return anything but takes a string parameter for the result (request body)...
  public void PostToLeaderboard(string result)
  {
    // declare an HttpResponse variable and invoke the base Post method to send the result data...
    HttpResponse response = base.Post(LeaderboardEndpoint, result);

    // check for a valid response and generate a message accordingly...
    string confirmation = (response != null)
        ? "Result successfully posted to leaderboard"
        : "Error posting to leaderboard";

    // invoke the base class Log method to print the message...
    base.Log(confirmation);
  }

  // create a method to initiate shutdown of the game server...
  public void Shutdown()
  {
    // declare an HttpResponse variable and invoke the base Get method to initiate shutdown...
    HttpResponse response = base.Get(ShutdownEndpoint);

    // check for a valid response and generate a message accordingly...
    string confirmation = (response != null)
        ? "Server shutdown successful"
        : "Error shutting down server";

    // invoke the base class Log method to print the message...
    base.Log(confirmation);
  }
}
```

### 4. Implement the typing test game logic
Now we have classes defined to handle the required server interactions, we can implement the game logic. As described in the introduction to this section, the game loop:
- renders a random word from the supplied list to the screen
- monitors for user input and compares the word entered to the one prompted, maintaining a count of correctly v. incorrectly entered words
- stops after a defined period and calculates a score, which is then posted to the server (leaderboard).

Start by creating a new class in the project's root directory.

```C#
public class GameRunner
{
  // class definition here...
}
```

With the class established, it can now be populated with the fields, properties, and methods necessary to support the game loop described.

```C#
public class GameRunner
{
  // declare a constant to store the game timeout indicator (in seconds)...
  private const int Timeout = 30;

  // declare the fields needed by the game...
  private List<string> _words;
  private Random _randomGen;
  private string _currentWord;
  private string _typedWord;
  private int _incorrectWords;
  private DateTime _startTime;

  // some information will need to be shared with the Program class
  // declare as properties...
  public bool GameStarted { get; private set; }
  public int CorrectWords { get; private set; }

  // create a default constructor...
  public GameRunner()
  {
    // declare and initialise the API client and retrieve the word list from the server...
    var client = new WordApiClient();
    _words = client.GetWords();

    // log an entry to the console for tracability...
    Console.WriteLine($"Word list successfully initialised: {_words.Count}");
    
    // initialise global game fields...    
    _randomGen = new Random();
    _currentWord = "";
  }

  // create a method to initiate the game loop...
  public void StartGame()
  {
    // initialise fields that need to be reset for each round...
    GameStarted = true;
    _startTime = DateTime.Now;
    CorrectWords = 0;
    _incorrectWords = 0;
    _typedWord = "";

    // invoke a method (implemented below) to retrieve a random word from the list...
    GenerateNewWord();
  }

  // create a method to retrieve a random word from the list...
  private void GenerateNewWord()
  {
    // assign the word to the _currentWord variable, which is rendered to the screen by DisplayGame() (implemented below)...
    _currentWord = _words[_randomGen.Next(_words.Count)];
  }

  // create a method to render the game artefacts to the screen...
  public void DisplayGame()
  {
    // tell SplashKit to render a game timer and word counter...
    SplashKit.DrawText("Time: " + TimeElapsed() + "s", Color.Black, "arial", 18, 10, 10);
    SplashKit.DrawText("Correct Words: " + CorrectWords, Color.Black, "arial", 18, 10, 40);
    SplashKit.DrawText("Incorrect Words: " + _incorrectWords, Color.Black, "arial", 18, 10, 70);

    // tell SplashKit to render the game prompts and user inputs...
    SplashKit.DrawText("Type the Word:", Color.Black, "arial", 24, 300, 200);
    SplashKit.DrawText(_currentWord, Color.Blue, "arial", 24, 300, 240); // the propmt...
    SplashKit.DrawText(_typedWord, Color.Green, "arial", 24, 300, 280); // the user's input...
  }

  // create a method to handle user inputs during the game...
  public void HandleTypingInput()
  {
    // iterate over the set of available keys to capture and handle target inputs...
    foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
    {
      if (SplashKit.KeyTyped(key))
      {
        // check whether backspace pressed and handle where one or more characters have been entered...
        if (key == KeyCode.BackspaceKey && _typedWord.Length > 0)
        {
          // remove last character in current text entry string...
          _typedWord = _typedWord.Substring(0, _typedWord.Length - 1);
        }
        else
        {
          string keyName = SplashKit.KeyName(key);

          // handle entry of letters and numbers...
          if (keyName.Length == 1 && char.IsLetterOrDigit(keyName[0]))
          {
              // add entered character to current text entry string...
              _typedWord += char.ToLower(keyName[0]);
          }
          else if (key == KeyCode.SpaceKey) // handle spacebar input...
          {
              _typedWord += ' ';
          }
        }
      }
    }

    // check whether return (enter) key pressed and if so, invoke a method (implemented below) to compare
    // the typed word to the active prompt...
    if (SplashKit.KeyTyped(KeyCode.ReturnKey))
    {
      CheckWord();
    }
  }

  // create a method to check whether the user-entered word matches the word prompt...
  private void CheckWord()
  {
    // check whether the words match (ignoring case) and if so, incrememnt CorrectWords count...
    if (_typedWord.Equals(_currentWord, StringComparison.OrdinalIgnoreCase))
        CorrectWords++;
    else // words don't match so increment _incorrectWords count...
        _incorrectWords++;

    // reset the _typedWord variable and get a new random word from the list...
    _typedWord = "";
    GenerateNewWord();
  }

  // create a method to check whether the game has reached its timeout threshold...
  public void CheckEndGameConditions()
  {
    // check whether the time since start matches and timeout period...
    if (TimeElapsed() == Timeout)
    {
      // stop the game loop...
      GameStarted = false;

      // get the result (using methods implemented below) and log to the console...
      string gameResult = $"Correct words: {CorrectWords}\nIncorrect words: {_incorrectWords}\nWPM: {CheckScore()}\nAccuracy: {Math.Round(CheckAccuracy(), 2)}";
      Console.WriteLine($"Game Over! {gameResult}");

      // initiate posting of the result to the game server (method implementation below)...
      PostToLeaderboard(gameResult);
    }
  }

  // create a method to derive the game score...
  public int CheckScore()
  {
    // the score is based on the words typed per minute...
    return (CorrectWords + _incorrectWords) * (60 / Timeout);
  }

  // create a method to check the user's accuracy...
  public double CheckAccuracy()
  {
    // accuracy is based on the proportion of correct to incorrect words entered...
    int totalWords = CorrectWords + _incorrectWords;
    return totalWords > 0 ? (double)CorrectWords / totalWords * 100 : 0;
  }

  // create a method to determine how long the game loop has been running (in seconds)...
  private int TimeElapsed()
  {
    return (int)(DateTime.Now - _startTime).TotalSeconds;
  }

  // create a method to initiate posting of a game result to the leaderboard (via WordApiClient)...
  private void PostToLeaderboard(string gameResult)
  {
    // declare and initialise a client and post the game result...
    var client = new WordApiClient();
    client.PostToLeaderboard(gameResult);
  }
}
```

### 5. Implement the `Main` method
We now have all the classes we require to run the game. All that's left is to implement `Main()` in the `Program` class.

```C#
public static class Program
{
  static void Main()
  {
    // declare and initialise the game runner...
    GameRunner runner = new GameRunner();

    // open a window in which to render the game artefacts...
    SplashKit.OpenWindow("Typing Test", 800, 600);

    // tell the game loop to execute until the exit condition (user-initiated close) met...
    while (!SplashKit.WindowCloseRequested("Typing Test"))
    {
      // get user events... 
      SplashKit.ProcessEvents();
      SplashKit.ClearScreen(Color.White);

      // show landing screen on launch...
      if (!runner.GameStarted)
      {
        SplashKit.DrawText("Press Enter to Start!", Color.Black, 320, 290);

        // show result on landing screen when returned from game...
        if (runner.CorrectWords > 0)
            SplashKit.DrawText($"Score: {runner.CheckScore()} wpm @ {Math.Round(runner.CheckAccuracy(),2)}% accuracy", Color.Black, _customFont, 18, 10, 10);

        // start a new game on return (enter) key input...
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

    // exit condition met. Initiate shutdown of mock server before terminating...
    var client = new WordApiClient();
    client.Shutdown();
  }
}
```

### 6. Run the programs
To play the game, run the `GameServer` program and wait on the startup message before running the `TypingTest` program. Start the game and monitor both consoles to observe the interactions between client and server.
![Screenshot 2025-01-29 at 8 19 41 PM](https://github.com/user-attachments/assets/92638c86-d4a3-4c50-9953-cfe313eeab5f)

> ### Potential enhancements
> - integrate a custom font to improve readabilty
> - add simple authentication to the game server
> - add configuration options (e.g. timeout duration)
> - add a lightweight database to the game server to support storage of an expanded word list and retention of leaderboard scores
