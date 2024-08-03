using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MusicBot.models;
using MyMusicBot.Handlers;
using Victoria;
using Victoria.Enums;
using YandexMusicApi.Api;
using YandexMusicApi.ComponentModels;
using YandexMusicApi.Network;

namespace MyMusicBot.Services
{
    public sealed class AudioManager
    {
    
        public static async Task<Embed> JoinAsync(IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            var provider = ServiceManager.Provider;
            var _lavaNode = provider.GetRequiredService<LavaNode<LavaPlayer<LavaTrack>, LavaTrack>>();

            if (!_lavaNode.IsConnected)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Join", "I'm already connected to a voice channel!");
            }

            if (voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Join", "You must be connected to a voice channel!");
            }

            try
            {
                var connection = await _lavaNode.JoinAsync(voiceState.VoiceChannel);
                return await EmbedHandler.CreateBasicEmbed("Music, Join", $"Joined {voiceState.VoiceChannel.Name}. {textChannel.Name}", Color.Green);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Join", ex.Message);
            }

        }



    }
}
