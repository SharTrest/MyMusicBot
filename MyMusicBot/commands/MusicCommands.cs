using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

using MusicBot.Services;
using MyMusicBot.Handlers;
using System.Drawing;


namespace MyMusicBot.Commands
{
    public class MusicCommands : ApplicationCommandModule
    {
        [SlashCommand("play", "Введите исполнителя и название песни")]
        public async Task PlayMusic(InteractionContext context, [Option("track", "Исполнитель и название песни")] string query)
        {
            DSharpPlus.Entities.DiscordChannel userVC;
            DSharpPlus.Lavalink.LavalinkExtension lavalinkInstance;

            try
            {
                await MusicBotServices.CheckConnection(context, out userVC, out lavalinkInstance);
            }
            catch (Exception ex)
            {
                await context.Channel.SendMessageAsync(ex.Message);
                return;
            }
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(context.Member.VoiceState.Guild);
            if (conn == null)
            {
                await context.Channel.SendMessageAsync("Ошибка!");
            }

            var track = await MusicBotServices.SearchTrack(query);

            var searchQuery = await node.Rest.GetTracksAsync(track.Uri);

            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed) 
            {
                await context.Channel.SendMessageAsync(embed:await EmbedHandler.CreateErrorEmbed("проигрывании трека 😔", "повторите попытку позже"));
                return;
            }

            var color = DiscordColor.Lilac;
            var title = "Cейчас играет ебейший трек!";
            var description = $"Сейчас играет {track.Title}\n"
                               + $"Исполнитель: {track.Artist}\n"
                               + $"Альбом: {track.Album}\n"
                               + $"Ссылочка на трек: {track.Url}";

            var nowPlaying = await EmbedHandler.CreateBasicEmbed(title, description, color);
            

            await context.CreateResponseAsync($"<@{context.User.Id}>", embed: nowPlaying);
            var musicTrack = searchQuery.Tracks.First();
           
            await conn.PlayAsync(musicTrack);
        }

        [SlashCommand("stop", "Останавливает проигрывание трека")]
        public async Task StopMusic(InteractionContext context)
        {
            DSharpPlus.Entities.DiscordChannel userVC;
            DSharpPlus.Lavalink.LavalinkExtension lavalinkInstance;
            try
            {
                MusicBotServices.CheckConnection(context, out userVC, out lavalinkInstance);
            }
            catch (Exception ex)
            {
                context.Channel.SendMessageAsync(ex.Message);
                return;
            }
        }
    }
}
