using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MusicBot.Services;
using MyMusicBot.Commands;
using MyMusicBot.Services;

public class Program
{
    public static DiscordClient Client { get; set; }
    private static CommandsNextExtension Commands { get; set; }

    private static Task Main()
            => new DiscordService().InitializeAsync();

}
