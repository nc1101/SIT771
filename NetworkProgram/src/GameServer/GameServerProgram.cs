using SplashKitSDK;

namespace MockServer
{
    public class GamerServerProgram
    {
        public static void Main()
        {
            Console.WriteLine("Starting Mock API Server at http://localhost:8080...");

            // Create a SplashKit HTTP server...
            WebServer server = new WebServer(8080); // defaults to port 8080...
            HttpRequest request;
            
            request = server.NextWebRequest;
            Console.WriteLine($"{request.Method}: {request.URI}");

            // continue serving requests until shutdown explicitly called...
            while (!request.IsGetRequestFor("/api/shutdown"))
            {
                if (request.IsGetRequestFor("/api/words"))
                {
                    Console.WriteLine("Retrieving words...");
                    
                    // prepare JSON object for sending...
                    Json response = SplashKit.CreateJson();
                    SplashKit.JsonSetArray(response, "words", WordList);
                    SplashKit.SendResponse(request, response); // return word list as JSON...
                }
                else if (request.IsPostRequestFor("/api/leaderboard"))
                {
                    Console.WriteLine("Posting to leaderboard...");
                    
                    Console.WriteLine(request.Body);  // simulate leaderboard update by posting to console...
                    SplashKit.SendResponse(request, "OK"); // confirm request succeeded...
                }
                // request handled or invalid. Move to next...
                request = server.NextWebRequest;
            }
            Console.WriteLine("Shutting down Mock API Server...");
            server.Stop();
        }
        
        private static readonly List<string> WordList = [
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
}
