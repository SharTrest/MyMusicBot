using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyMusicBot.Handlers;
using MyMusicBot.Services;
using Victoria;


namespace MyMusicBot
{
    #region
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commandService;
        private InteractionService _commands;
        public IConfiguration Configuration { get; }


        public Bot()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                UseInteractionSnowflakeDate = true,
                GatewayIntents = GatewayIntents.All | GatewayIntents.MessageContent,
                LogLevel = LogSeverity.Debug
            });

            _commandService = new CommandService(new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = true,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                IgnoreExtraArgs = true
            });


            var collection = new ServiceCollection();
            collection.Clear();
            collection.AddLavaNode(
                x =>
                {
                    x.IsSecure = true;
                    x.Hostname = "lavalinkv4.serenetia.com";
                    x.Port = 443;
                    x.Authorization = "lavalinkv4";
                }
            );
            collection.AddSingleton<ConfigManager>();
            collection.AddSingleton<SearchService>();
            collection.AddSingleton(_client);
            collection.AddSingleton(_commandService);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<AudioService>();
            collection.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
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
            var commands = ServiceManager.Provider.GetRequiredService<InteractionService>();
            _commands = commands;
            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, ConfigManager.Config.Token);
            await _client.StartAsync();

            await ServiceManager.Provider.GetRequiredService<CommandHandler>().InitializeAsync();

            await Task.Delay(-1);
        }

        private async Task ReadyAsync()
        {
            await _commands.RegisterCommandsGloballyAsync(true);
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
        }
        static bool IsDebug()
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }
    #endregion


}
