using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using YandexMusicApi.Api;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyMusicBot.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordClient _client;
        private readonly SlashCommandsExtension _commands;
        private readonly IServiceProvider _services;
        private readonly DiscordConfiguration _logger;

        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<SlashCommandsExtension>();
            _client = services.GetRequiredService<DiscordClient>();
            _logger = services.GetRequiredService<DiscordConfiguration>();
            _services = services;

            HookEvents();
        }

        private void HookEvents()
        {
            _commands.SlashCommandExecuted += SlashCommandExecutedAsync;
        }

        private async Task SlashCommandExecutedAsync(SlashCommandsExtension sender, SlashCommandExecutedEventArgs args)
        {
            await Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var slash = _client.UseSlashCommands();

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace.StartsWith("MyMusicBot.Commands")) //Limit our namespace to Project.Commands
                .Where(s => s.Name.Contains("Commands")); //Limit the types to those named with the word Command

            foreach (var t in types)
            {
                slash.RegisterCommands(t);
            }
        }



    }
}
