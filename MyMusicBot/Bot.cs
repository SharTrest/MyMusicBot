using AngleSharp;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyMusicBot.Services;
using Victoria;


namespace MyMusicBot
{
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        public IConfiguration Configuration { get; }


        public Bot()
        {
            
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                LogLevel = LogSeverity.Debug
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                IgnoreExtraArgs = true
            });

            
            var collection = new ServiceCollection();
            collection.AddLavaNode(
                x =>
                {
                    x.IsSecure = true;
                    x.Hostname = "ssl.lavalink.rocks";
                    x.Port = 443;
                    x.Authorization = "horizxon.tech";
                }
            );
            collection.AddSingleton(_client);
            collection.AddSingleton(_commandService);
            collection.AddLogging(x =>
            {
                x.ClearProviders();
                x.SetMinimumLevel(LogLevel.Trace);
            });


            ServiceManager.SetProvider(collection);


            Console.WriteLine();
        }
            

            public async Task MainAsync()
        {
            if (string.IsNullOrEmpty(ConfigManager.Config.Token)) return;

            await CommandManager.LoadCommandsAsync();
            await EventManager.LoadCommands();
            await _client.LoginAsync(TokenType.Bot, ConfigManager.Config.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

    }
}
