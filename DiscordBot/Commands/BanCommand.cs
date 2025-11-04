using Discord;
using Discord.Interactions;

public class BanCommand : InteractionModuleBase<SocketInteractionContext>
{
  [SlashCommand("ban", "ban user")]
  public async Task Ban(
      [Summary("user", "target user to ban")] IUser user,
      [Summary("reason", "reason for ban")] string reason
      )
  {
    var bot = Context.Guild.CurrentUser;
    var bans = Context.Guild.GetBansAsync();
    if (!bot.GuildPermissions.BanMembers)
    {
      await RespondAsync("I dont have a right to ban the member");
      return;
    }
    await Context.Guild.RemoveBanAsync(user);
    await RespondAsync("I have removed the ban");
  }

}
