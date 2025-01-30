using SplashKitSDK;

namespace TypingTest;

public abstract class ApiConnector
{
    private const ushort Port = 8080;
    protected abstract string BaseUrl { get; }
    
    /**
     * <summary>Submits an HTTP GET request to a specified endpoint</summary>
     * <returns>an HttpResponse object where successful, else null</returns>
     */
    protected HttpResponse Get(string endpoint)
    {
        try
        {
            // construct request path...
            string path = $"{BaseUrl}{endpoint}";
            Log($"Sending GET request to {path}");
            return SplashKit.HttpGet(path, Port); // submit the GET request and return the result...
        }
        catch (Exception e)
        {
            // where an error is encountered, log a message to the console and return null...
            Log($"Error during GET request: {e.Message}");
            return null;
        }
    }

    /**
     * <summary>Submits an HTTP POST request to a specified endpoint</summary>
     * <returns>an HttpResponse object where successful, else null</returns>
     */
    protected HttpResponse Post(string endpoint, string messageBody)
    {
        try
        {
            // construct request path...
            string uri = $"{BaseUrl}{endpoint}";
            Log($"Sending POST request to {uri}");
            return SplashKit.HttpPost(uri, Port, messageBody);
        }
        catch (Exception e)
        {
            // where an error is encountered, log a message to the console and return null...
            Log($"Error during POST request: {e.Message}");
            return null;
        }
    }

    /**
     * <summary>Logs a timestamped message to the console</summary>
     */
    protected void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] {message}");
    }
}