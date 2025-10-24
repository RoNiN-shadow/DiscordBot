using Discord;
using Discord.Interactions;
using System.Text.Json.Nodes;


public class RonaldoCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("siuuu", "Does the super SUI")]
    public async Task SuperSui()
    {
        Emoji goatEmoji = new Emoji("\uD83D\uDC10");
        string gifUrl = await GetRonaldoGif();
        await RespondAsync($"{gifUrl}\nSIUUUUUUU {goatEmoji}");
    }
    private static async Task<string> GetRonaldoGif()
    {
        string? apiKey = Environment.GetEnvironmentVariable("GIPHY_API_KEY");
        string endpoint = $"https://api.giphy.com/v1/gifs/random?api_key={apiKey}&tag=ronaldo&rating=g";

        try
        {
            using var http = new HttpClient();
            string response = await http.GetStringAsync(endpoint);

            JsonNode? json = JsonNode.Parse(response)
                    ?? throw new InvalidOperationException("can't parse JSON");
            string? gifUrl = (json["data"]?["images"]?["original"]?["url"]?.ToString())
                    ?? throw new Exception("Giphy response did not work");
            return gifUrl;

        }
        catch(HttpRequestException httpEx)
        {
            Console.WriteLine($"Http request error - {httpEx.Message}");
            return $"Http request error - {httpEx.Message}";
        }
    }
}

