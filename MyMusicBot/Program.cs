using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MusicBot.Commands;
using MusicBot.services;
using System.Formats.Asn1;

public class Program
{
    public static DiscordClient Client { get; set; }
    private static CommandsNextExtension Commands { get; set; }

    static async Task Main(string[] args)
    {
        
        var config = new JsonReader();
        await config.ReadJSON();

        var discordConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = config.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        Client = new DiscordClient(discordConfig);


        Client.Ready += Client_Ready;

        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = new string[] { config.Prefix },
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = true,
        };

        Commands = Client.UseCommandsNext(commandsConfig);


        var endpoint = new ConnectionEndpoint
        {
            Hostname = "lava-v3.ajieblogs.eu.org",
            Port = 443,
            Secured = true,
        };

        var lavalinkCfg = new LavalinkConfiguration
        {
            Password = "https://dsc.gg/ajidevserver",
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint,
        };

        var slashCommandsConfig = Client.UseSlashCommands();
        slashCommandsConfig.RegisterCommands<MusicCommands>();

        var lavalink = Client.UseLavalink();

        await Client.ConnectAsync();
        await lavalink.ConnectAsync(lavalinkCfg);
        await Task.Delay(-1);
    }



    private static async Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        await Task.CompletedTask;
    }
}
