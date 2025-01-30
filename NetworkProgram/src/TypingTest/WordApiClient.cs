using SplashKitSDK;

namespace TypingTest;

public class WordApiClient : ApiConnector
{
    private const string Url = "http://localhost:8080/api/";
    private const string WordEndpoint = "words";
    private const string LeaderboardEndpoint = "leaderboard";
    private const string ShutdownEndpoint = "shutdown";

    protected override string BaseUrl => Url;
    
    /**
     * <summary>Retrieves word list from the game server</summary>
     * <returns>the list of retrieved words where successful, else a backup list</returns>
     */
    public List<string> GetWords()
    {
        // invoking base method to get resource...
        HttpResponse response = base.Get(WordEndpoint);

        try
        {
            // get response object and convert to JSON...
            string strResponse = SplashKit.HttpResponseToString(response);
            Json jsonResponse = SplashKit.JsonFromString(strResponse);
            SplashKit.FreeResponse(response);

            // use SplashKit's built-in utility to convert response from JSON array to List...
            List<string> words = new List<string>();
            SplashKit.JsonReadArray(jsonResponse, "words", ref words);

            return words;
        }
        catch (Exception e)
        {
            // where an error is encountered, log a message to the console and return backup list...
            base.Log($"Error parsing JSON response: {e.Message}");
            return BackupWords;
        }
    }

    /**
     * <summary>Posts a result to the game server</summary>
     */
    public void PostToLeaderboard(string result)
    {
        // invoking base method to post message...
        HttpResponse response = base.Post(LeaderboardEndpoint, result);

        string confirmation = (response != null)
            ? "Result successfully posted to leaderboard"
            : "Error posting to leaderboard";
        Console.WriteLine(confirmation);
    }

    /**
     * <summary>Initiates shutdown of the game server</summary>
     */
    public void Shutdown()
    {
        // invoking base method to fulfil request...
        HttpResponse response = base.Get(ShutdownEndpoint);
        
        string confirmation = (response != null)
            ? "Server shutdown successful"
            : "Error shutting down server";
        Console.WriteLine(confirmation);
    }
    
    // static word list used where game server unresponsive...
    private static readonly List<string> BackupWords = [
        "myrmecology",
        "beater",
        "unclouded",
        "delusional",
        "overbid",
        "nomadic",
        "nones",
        "carrousel",
        "outlets",
        "templates",
        "ember",
        "novelisations",
        "glossiness",
        "controversial",
        "monocyte",
        "impugner",
        "embroiled",
        "initialism",
        "tabbies",
        "gelato",
        "physiologist",
        "delayed",
        "scriptures",
        "dribbled",
        "provisional",
        "germ",
        "hairstylists",
        "spottily",
        "elated",
        "mapping",
        "tiebreakers",
        "easters",
        "coffees",
        "conformable",
        "central",
        "capitalism",
        "germinal",
        "kilosiemens",
        "ultra",
        "humanistic",
        "formatters",
        "fortune",
        "conversions",
        "angularities",
        "aneurysm",
        "quantize",
        "contribute",
        "cephalics",
        "teeing",
        "denudes"
    ];
}