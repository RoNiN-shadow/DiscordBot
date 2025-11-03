using System.Text;
using Discord.Interactions;
using System.Text.Json.Nodes;

public class ChatAI : InteractionModuleBase<SocketInteractionContext> {

    private readonly static string _modelName = "gemeni-2.5-flash-lite"; 
    private readonly static string _endpoint=$"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent";
    private static string? _message;

    [SlashCommand("chat", "Chat with AI")]
    public async Task Chat([Summary("text", "Say anything")] string message)
    {
        _message = message;
        //let the bot think a bit longer
        await DeferAsync();
        //asynchronsly iterate over IEnumarable splited text
        await foreach (var answer in SplitAnswer())
        {
            await FollowupAsync(answer);
        }
    }

    private static async Task<JsonNode> MakeRequest()
    {

        string? apiKey = Environment.GetEnvironmentVariable("GEMENI_API");

        using HttpClient http = new();
        http.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

        JsonObject json = new()
        {
            ["contents"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["parts"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["text"] = _message
                        }
                    }
                }
            }
        };
        StringContent content = new(json.ToString(), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await http.PostAsync(_endpoint, content);

        string? resultString = await response.Content.ReadAsStringAsync() ?? "";

        JsonNode? doc = JsonNode.Parse(resultString)
                ?? throw new InvalidOperationException("didn't parse JSON");
        return doc;
    }
    private static async IAsyncEnumerable<string> SplitAnswer()
    {

        JsonNode doc = await MakeRequest();

        string? resultText = doc["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
        Console.WriteLine(resultText);

        for (int i = 0; i < resultText?.Length; i += 2000)
        {
            int end = Math.Min(2000, resultText.Length - i);
            yield return resultText.Substring(i, end);
        }
    }
}

