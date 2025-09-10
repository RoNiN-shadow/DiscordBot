using System.Text;
using Discord.Interactions;
using System.Text.Json.Nodes;

public class RonaldoAI : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("chat", "Chat with AI")]
    public async Task Chat([Summary("text", "Say anything")] string message)
    {
        //let the bot think a bit longer
        await DeferAsync();
        //asynchronsly iterate over IEnumarable splited text
        await foreach (var answer in GetAnswerAsync(message))
        {
            await FollowupAsync(answer);
        }
    }

    private static async IAsyncEnumerable<string> GetAnswerAsync(string message)
    {
        string? apiKey = Environment.GetEnvironmentVariable("GEMENI_API");
        string modelName = "gemini-2.5-flash-lite";
        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent";

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
                            ["text"] = message
                        }
                    }
                }
            }
        };

        StringContent content = new(json.ToString(), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await http.PostAsync(endpoint, content);

        string? resultString = await response.Content.ReadAsStringAsync() ?? "";

        JsonNode? doc = JsonNode.Parse(resultString)
                ?? throw new InvalidOperationException("didn't parse JSON");

        string? resultText = doc["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
        Console.WriteLine(resultText);

        for (int i = 0; i < resultText?.Length; i += 2000)
        {
            int end = Math.Min(2000, resultText.Length - i);
            yield return resultText.Substring(i, end);
        }
        //more FP approch but I prefer for-loop in this case because it is more clear
        //IEnumerable<string> resultEnum = Enumerable
        //    .Range(0, (resultText.Length + 2000 - 1) / 2000)
        //    .Select(i => resultText.Substring(i * 2000, Math.Min(2000, resultText.Length - i)));



    }
}

