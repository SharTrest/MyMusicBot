using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria.WebSocket.EventArgs;
using Victoria;
using System.Text.Json;
using Victoria.Enums;
using MusicBot.models;

namespace MyMusicBot.Services
{
    public sealed class AudioService
    {
        private readonly LavaNode<LavaPlayer<LavaTrack>, LavaTrack> _lavaNode;
        private readonly DiscordSocketClient _socketClient;
        private readonly ILogger _logger;
        public readonly HashSet<ulong> VoteQueue;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;
        public readonly ConcurrentDictionary<ulong, ulong> TextChannels;
        public ConcurrentDictionary <ulong, List<string>> _tracks;


        public AudioService(
            LavaNode<LavaPlayer<LavaTrack>, LavaTrack> lavaNode,
            DiscordSocketClient socketClient,
            ILogger<AudioService> logger)
        {
            
            _lavaNode = lavaNode;
            _socketClient = socketClient;
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();
            _logger = logger;
            TextChannels = new ConcurrentDictionary<ulong, ulong>();
            VoteQueue = [];
            _lavaNode.OnWebSocketClosed += OnWebSocketClosedAsync;
            _lavaNode.OnStats += OnStatsAsync;
            _lavaNode.OnPlayerUpdate += OnPlayerUpdateAsync;
            _lavaNode.OnTrackEnd += OnTrackEndAsync;
            _lavaNode.OnTrackStart += OnTrackStartAsync;
            _tracks = new ConcurrentDictionary<ulong, List<string>>();
        }

        public async Task AddTrackInQueue(ulong playerGuildId, string trackName)
        {
   
            _tracks.TryGetValue(playerGuildId, out var list);
            if (list == null)
                list = new List<string>();
            list.Add(trackName);
            _tracks.AddOrUpdate(playerGuildId, list, (key, oldList) => list);
        }

        public async Task<string> RemoveTrackFromQueue(ulong playerGuildId)
        {
            _tracks.TryGetValue(playerGuildId, out var list);
            var trackName = list.FirstOrDefault();
            list.RemoveAt(0);
            _tracks.AddOrUpdate(playerGuildId, list, (key, oldList) => list);
            return trackName;
        }

        private async Task OnTrackEndAsync(TrackEndEventArg arg)
        {
            if (arg.Reason != TrackEndReason.Finished)
            {
                return;
            }

            var player = await _lavaNode.TryGetPlayerAsync(arg.GuildId);
          
            if (!player.GetQueue().TryDequeue(out var queueable))
            {
                await SendAndLogMessageAsync(arg.GuildId,
                        "Очередь пустая, добавьте треков!");
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await SendAndLogMessageAsync(arg.GuildId,
                         "Там не трек!");
                return;
            }
            RemoveTrackFromQueue(arg.GuildId);
            await player.PlayAsync(_lavaNode, track);
            await SendAndLogMessageAsync(arg.GuildId,
                         "Сейчас играет что-то!");
        }

        private Task OnTrackStartAsync(TrackStartEventArg arg)
        {
            var track = _tracks.TryGetValue(arg.GuildId, out var list);
            _logger.LogInformation("Guild latency: {}", track);
            return SendAndLogMessageAsync(arg.GuildId,
                $"Сейчас играет: {list.FirstOrDefault()}");
        }


        private Task OnPlayerUpdateAsync(PlayerUpdateEventArg arg)
        {
            _logger.LogInformation("Guild latency: {}", arg.Ping);
            return Task.CompletedTask;
        }

        private Task OnStatsAsync(StatsEventArg arg)
        {
            _logger.LogInformation("{}", JsonSerializer.Serialize(arg));
            return Task.CompletedTask;
        }

        private Task OnWebSocketClosedAsync(WebSocketClosedEventArg arg)
        {
            _logger.LogCritical("{}", JsonSerializer.Serialize(arg));
            return Task.CompletedTask;
        }

        private Task SendAndLogMessageAsync(ulong guildId,
                                            string message)
        {
            _logger.LogInformation(message);
            if (!TextChannels.TryGetValue(guildId, out var textChannelId))
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;

            //return (_socketClient
            //        .GetGuild(guildId)
            //        .GetChannel(textChannelId) as ITextChannel)
            //    .SendMessageAsync(message);
        }
    }
}
