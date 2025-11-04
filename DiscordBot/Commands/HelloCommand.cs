using Discord;
using Discord.Interactions;

public class HelloCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("hello", "says hello to the chat")]
    [CommandContextType(InteractionContextType.PrivateChannel)]
    public async Task Hello(){

      await RespondAsync($"Hello, {Context.User.GlobalName}");
    }
}
