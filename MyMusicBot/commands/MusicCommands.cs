using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using MusicBot.services;


namespace MusicBot.Commands
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
            var nowPlaying = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Lilac,
                Title = "Cейчас играет ебейший трек!",
                Description = $"Сейчас играет {track.Title}\n"
                   + $"Исполнитель: {track.Artist}\n"
                   + $"Альбом: {track.Album}\n"
                   + $"Ссылочка на трек: {track.Url}",
            };

            await context.CreateResponseAsync($"<@{context.User.Id}>", embed: nowPlaying);

            Thread.Sleep(1000);

            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed) 
            {
                Console.WriteLine(track.JToken);
                await context.Channel.SendMessageAsync($"Трек не найден :( {track.Uri}");
            }
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
