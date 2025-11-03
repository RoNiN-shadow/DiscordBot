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

            string? botToken = Environment.GetEnvironmentVariable("DISCORD_API_KEY");
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
                
                await _interactionService.AddModulesAsync(typeof(Program).Assembly, _services);
                await _interactionService.RegisterCommandsToGuildAsync(guild.Id);

            }

        }
        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }
    }

}
