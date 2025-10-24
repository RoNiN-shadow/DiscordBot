using Discord;
using Discord.Interactions;

public class DevTools : InteractionModuleBase<SocketInteractionContext>
{
    private readonly InteractionService _interactionService;
    public DevTools(InteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    [SlashCommand("ping", "ping/pong")]
    public async Task Ping()
    {
        await RespondAsync("Pong!");
    }
    [SlashCommand("repeat", "repeats your message")]
    public async Task Repeat([Summary("text", "Say anything")] string message)
    {
        await RespondAsync(message);
    }
    [UserCommand("info")]
    public async Task UserInfoAsync(IUser user)
    {
        await RespondAsync($"User id is - {user.Id}");
    }
    [SlashCommand("list", "list all avaliable commands")]
    public async Task ListAllCommands()
    {
        var commands = _interactionService.SlashCommands.Select(c => $"{c.Name} - {c.Description}");

        await RespondAsync(string.Join("\n", commands));
    }

}
