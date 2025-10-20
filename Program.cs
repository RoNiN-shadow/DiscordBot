using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Interactions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;



namespace DiscordBot
{
    class Program
    {
        private readonly DiscordSocketClient _client; 
        private readonly InteractionService _interactionService;

        private readonly IServiceProvider _services;
        

        public Program()
        {
            _client = new DiscordSocketClient();
            _interactionService = new InteractionService(_client.Rest);
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_interactionService)
                .BuildServiceProvider();

        }

        static async Task Main(string[] args) => await new Program().MainAsync();

        public async Task MainAsync()
        {
            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.InteractionCreated += HandleInteraction;
            _client.SlashCommandExecuted += SlashCommandHandler;

            string? botToken = Environment.GetEnvironmentVariable("DISOCRD_API_KEY");
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private  async Task ClientReady()
        {
            foreach(SocketGuild guild in _client.Guilds)
            {
                Console.WriteLine("****************");

                Console.WriteLine($"Name: {guild.Name}");
                Console.WriteLine($"ID: {guild.Id}");
                Console.WriteLine($"Owner: {guild.OwnerId}");
                Console.WriteLine($"Icon URL: {guild.IconUrl}");

                Console.WriteLine("****************");

                
                await _interactionService.AddModulesAsync(typeof(Program).Assembly, _services);
                await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
                SlashCommandBuilder suiCommand = new SlashCommandBuilder()
                    .WithName("sui")
                    .WithDescription("suuuuiii");

                try
                {
                    await guild.CreateApplicationCommandAsync(suiCommand.Build());
                }
                catch (HttpException exception)
                {
                    string json = JsonSerializer.Serialize(exception.Errors, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    Console.WriteLine(json);
                }

                if (guild != null){

                    var textChannel = guild.TextChannels
                       .FirstOrDefault(c =>
                            c.GetUser(_client.CurrentUser.Id)?.GetPermissions(c).SendMessages == true);
 
                    if (textChannel != null){

                        await textChannel.SendMessageAsync("The bot is alive");

                    }
                    else{
                        Console.WriteLine("Can't access text channels for this bot");
                    }
                }
            }

        }
        private  async Task SlashCommandHandler(SocketSlashCommand command) 
        {
            switch (command.Data.Name)
            {
                case "sui":
                    await command.RespondAsync($"SUIIIII");
                    break;
            }
        }
        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }
    }

}
