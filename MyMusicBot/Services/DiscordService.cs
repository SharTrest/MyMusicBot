using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using MusicBot.Services;
using MyMusicBot.Handlers;
using System.Reflection;

namespace MyMusicBot.Services
{
    public class DiscordService
    {
        private DiscordClient _client;
        private CommandsNextExtension _commands;
        private DiscordConfiguration _discordConfiguration;
        private CommandsNextConfiguration _commandsNextConfiguration;
        private SlashCommandsExtension _slashCommands;
        private ConnectionEndpoint _connectionEndpoint;
        private LavalinkConfiguration _lavalinkConfiguration;
        private LavalinkExtension _lavalink;

        public DiscordService()
        {
        }

        public async Task InitializeAsync()
        {
            ConfigureService(out _discordConfiguration, out _commandsNextConfiguration, out ConnectionEndpoint _connectionEndpoint, out LavalinkConfiguration _lavalinkConfiguration);
            _client = new DiscordClient(_discordConfiguration);
            _commands = _client.UseCommandsNext(_commandsNextConfiguration);
            _slashCommands = _client.UseSlashCommands();
            await RegisterSlashCommands();
            _lavalink = _client.UseLavalink();

            SubscribeDiscordEvents();

            await _client.ConnectAsync();

            try
            {
                await _lavalink.ConnectAsync(_lavalinkConfiguration);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }

            await Task.Delay(-1);
        }

        private void ConfigureService(out DiscordConfiguration _discordConfiguration, out CommandsNextConfiguration _commandsNextConfiguration, out ConnectionEndpoint _connectionEndpoint, out LavalinkConfiguration _lavalinkConfiguration)
        {
            var config = new JsonReader();
            Task.Run(async () =>
            {
                await config.ReadJSON();
            }).Wait();



            _discordConfiguration = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt"
            };
            _commandsNextConfiguration = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { config.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,

            };
            _connectionEndpoint = new ConnectionEndpoint
            {
                Hostname = "lava-v3.ajieblogs.eu.org",
                Port = 443,
                Secured = true,
            };

            _lavalinkConfiguration = new LavalinkConfiguration
            {
                Password = "https://dsc.gg/ajidevserver",
                RestEndpoint = _connectionEndpoint,
                SocketEndpoint = _connectionEndpoint,
            };

        }

        private async Task RegisterSlashCommands()
        {
            var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == "MyMusicBot.Commands") //Limit our namespace to Project.Commands
            .Where(s => s.Name.Contains("Command")); //Limit the types to those named with the word Command

            foreach (var t in types)
            {
                _slashCommands.RegisterCommands(t);
            }
            await Task.CompletedTask;
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
        }

        private async Task ReadyAsync(DiscordClient sender, ReadyEventArgs args)
        {
        }
    }
}
