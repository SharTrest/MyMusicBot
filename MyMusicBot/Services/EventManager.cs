using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace MyMusicBot.Services
{
    public class EventManager
    {
        private static DiscordSocketClient _client = ServiceManager.GetService<DiscordSocketClient>();
        private static CommandService _commandService = ServiceManager.GetService<CommandService>();

        public static Task LoadCommands()
        {

            _client.Log += message =>
            {
                Console.WriteLine($"{DateTime.Now}\t {message.Source}\t {message.Message}");
                return Task.CompletedTask;
            };
            _commandService.Log += message =>
            {
                Console.WriteLine($"{DateTime.Now}\t {message.Source}\t {message.Message}");
                return Task.CompletedTask;
            };

            _client.Ready += OnReady;

            _client.MessageReceived += GetOnMessageReceived;
            return Task.CompletedTask;
        }

        private static async Task GetOnMessageReceived(SocketMessage arg)
        {

            var message = arg as SocketUserMessage;

            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot || message.Channel is IDMChannel) return;

            var argPos = 0;
            if (!(message.HasStringPrefix(ConfigManager.Config.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var result = await _commandService.ExecuteAsync(context, argPos, ServiceManager.Provider);

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand) return;
            }
        }

        private static async Task OnReady()
        {
            try
            {
                var provider = ServiceManager.Provider;
                var _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode<LavaPlayer<LavaTrack>, LavaTrack>>();
                await _lavaNode.ConnectAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.WriteLine($"{DateTime.Now}\t (READY)\tBot is ready");
            await _client.SetStatusAsync(Discord.UserStatus.Online);
            await _client.SetGameAsync($"Prefix: {ConfigManager.Config.Prefix}", null, Discord.ActivityType.Listening);

        }
       
    }
}
