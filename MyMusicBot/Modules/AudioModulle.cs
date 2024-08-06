using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Css;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MyMusicBot.Handlers;
using MyMusicBot.Services;
using Victoria;
using Victoria.Rest;
using Victoria.Rest.Search;
using YandexMusicApi.Api;

namespace MyMusicBot.Modules
{

    public sealed class AudioModule(
        LavaNode<LavaPlayer<LavaTrack>, LavaTrack> lavaNode,
        AudioService audioService, InteractionService Commands,
        CommandHandler _handler, SearchService searchService)
        : InteractionModuleBase<SocketInteractionContext>
    {

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        [SlashCommand("join", "Вы призываете бота в канал!")]
        public async Task JoinAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await RespondAsync("Вы должны находиться в голосовом канале!");
                return;
            }

            try
            {
                await lavaNode.JoinAsync(voiceState.VoiceChannel);
                await RespondAsync($"Я тут {voiceState.VoiceChannel.Name}!");

                audioService.TextChannels.TryAdd(Context.Guild.Id, Context.Channel.Id);
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.ToString());
            }
        }

        [SlashCommand("leave", "Я ливаю, всем бб")]
        public async Task LeaveAsync()
        {
            var voiceChannel = (Context.User as IVoiceState).VoiceChannel;
            if (voiceChannel == null)
            {
                await RespondAsync("Я и так свободен.");
                return;
            }

            try
            {
                await lavaNode.LeaveAsync(voiceChannel);
                await RespondAsync($"Я СВОБОДЕЕЕЕН!");
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.Message);
            }
        }

        [SlashCommand("play", "Проигрывает Ваш трек")]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            await DeferAsync();

              if (string.IsNullOrWhiteSpace(searchQuery))
            {
                await FollowupAsync("Please provide search terms.");
                return;
            }

            var player = await lavaNode.TryGetPlayerAsync(Context.Guild.Id);
            if (player == null || player.VoiceState == null)
            {
                var voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    var emsg = await EmbedHandler.CreateErrorEmbed("/play", "Вы должны быть в голосовом канале!");
                    await FollowupAsync(embed: emsg);
                    return;
                }

                try
                {
                    player = await lavaNode.JoinAsync(voiceState.VoiceChannel);
                    audioService.TextChannels.TryAdd(Context.Guild.Id, Context.Channel.Id);
                }
                catch (Exception exception)
                {
                    await FollowupAsync(exception.Message);
                }
            }

            var findTrack = await SearchService.SearchTrack(searchQuery);
            var searchResponse = await lavaNode.LoadTrackAsync(findTrack.Uri);

            if (searchResponse.Type is SearchType.Empty or SearchType.Error)
            {
                var emsg = await EmbedHandler.CreateErrorEmbed("/play", $"К сожалению, `{searchQuery}` не найдено.");
                await FollowupAsync(embed: emsg);
                return;
            }

            var track = searchResponse.Tracks.FirstOrDefault();

            audioService.AddTrackInQueue(player.GuildId, findTrack.Title);

            if (player.Track == null)
            {
                var firstmsg = await EmbedHandler.CreateBasicEmbed("Сейчас играет!",
                        $"Название: {findTrack.Title}\n"
                        + $"Исполнитель: {findTrack.Artist}\n"
                        + $"Длительность: {track.Duration:mm\\:ss}\n"
                        + $"Ссылка на трек: {findTrack.Url}\n",
                        Color.Teal
                        );
                await player.PlayAsync(lavaNode, track);
                await FollowupAsync(embed: firstmsg);
                return;
            }
            player.GetQueue().Enqueue(track);

            var msg = await EmbedHandler.CreateBasicEmbed("Добавлен трек!",
                            $"Название: {findTrack.Title}\n"
                            + $"Исполнитель: {findTrack.Artist}\n"
                            + $"Длительность: {track.Duration:mm\\:ss}\n"
                            + $"Ссылка на трек: {findTrack.Url}\n",
                            Color.Teal);

            await FollowupAsync(embed: msg);

        }

        [SlashCommand("pause", "Останавливает трек"), RequirePlayer]
        public async Task PauseAsync()
        {
            var player = await lavaNode.TryGetPlayerAsync(Context.Guild.Id);
            if (player == null || player.IsPaused && player.Track != null)
            {
                await RespondAsync("Нечего останавливать!");
                return;
            }

            try
            {
                await player.PauseAsync(lavaNode);
                await RespondAsync($"Paused: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.Message);
            }
        }

        [SlashCommand("resume", "Продолжаем дискотеку"), RequirePlayer]
        public async Task ResumeAsync()
        {
            var player = await lavaNode.TryGetPlayerAsync(Context.Guild.Id);
            if (player == null || !player.IsPaused && player.Track == null)
            {
                await RespondAsync("Нечего проигрывать!");
                return;
            }

            try
            {
                await player.ResumeAsync(lavaNode, player.Track);
                await RespondAsync($"Resumed: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.Message);
            }
        }

        [SlashCommand("stop", "Останавливает дискотеку"), RequirePlayer]
        public async Task StopAsync()
        {
            var player = await lavaNode.TryGetPlayerAsync(Context.Guild.Id);
            if (!player.State.IsConnected || player.Track == null)
            {
                await RespondAsync(embed: await EmbedHandler.CreateErrorEmbed("/stop", "Нечего останавливать"));
                return;
            }

            try
            {
                await player.StopAsync(lavaNode, player.Track);
                await RespondAsync(embed: await EmbedHandler.CreateBasicEmbed("Трек остановлен", "", Color.Red));
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.Message);
            }
        }

        [SlashCommand("skip", "Пропуск трека"), RequirePlayer]
        public async Task SkipAsync()
        {
            var player = await lavaNode.TryGetPlayerAsync(Context.Guild.Id);

            if (!player.State.IsConnected)
            {
                await RespondAsync("Нечего скипать.");
                return;
            }

            {
                try
                {
                    var queue = player.GetQueue();
                    var cur = player.GetQueue().First();
                    var title = await audioService.RemoveTrackFromQueue(player.GuildId);
                    player.PlayAsync(lavaNode, cur, false);
                    player.GetQueue().TryDequeue(out cur);
                    await RespondAsync($"Skipped: {title}\n");
                }
                catch (Exception exception)
                {
                    player.GetQueue().Clear();
                    await RespondAsync("Очередь пустая");
                }
            }
        }
    }
}