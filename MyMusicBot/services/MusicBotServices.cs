using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using MusicBot.models;
using YandexMusicApi.Api;
using YandexMusicApi.ComponentModels;
using YandexMusicApi.Network;

namespace MusicBot.services
{
    public static class MusicBotServices
    {
        public static Task CheckConnection(InteractionContext context, out DSharpPlus.Entities.DiscordChannel _userVC, out DSharpPlus.Lavalink.LavalinkExtension _lavalinkInstance)
        {
            if (context.Member.VoiceState == null || context.Member.VoiceState.Channel == null)
            {
                throw new Exception($"<@{context.User.Id}> \n Вы должны быть в голосовом канале.");
            }

            var userVC = context.Member.VoiceState.Channel;
            var lavalinkInstance = context.Client.GetLavalink();

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                throw new Exception("Не удалось подключиться к Lavalink");
            }

            if (userVC.Type != DSharpPlus.ChannelType.Voice)
            {
                throw new Exception("Зайдите в нужный канал");
            }

            _userVC = userVC;
            _lavalinkInstance = lavalinkInstance;

            return Task.CompletedTask;
        }

        public async static Task<TrackModel> SearchTrack(string query)
        {
            TrackModel trackModel = null;
            var _apiParams = new ApiParams();
            _apiParams.TokenYandexMusic = "y0_AgAAAAAW1KybAAG8XgAAAAEMEFQ5AACn9cDlpzJPoJ_mcwHgv7buRrkWgg";
            var _track = new YandexMusicApi.Api.Track(_apiParams);
            var account = new YandexMusicApi.Api.Account(_apiParams);
            Search searchApi = new Search(_apiParams);

            var searchResultJson = await searchApi.RetrieveSearch(query, typeSearch: SearchEnums.TypeSearch.Tracks); // Search for your search query with album filter
            var searchResult = searchResultJson["result"]["result"];

            foreach (var i in searchResult["results"])
            {
                trackModel = new TrackModel(i);
                break;
            }

            var trackInfo = await _track.GetDownloadInfo(trackModel.TrackID);
            var downloadInfo = await _track.GetDownloadInfo(trackModel.TrackID);
            var downloadInfoJson = downloadInfo["result"];

            var codec = downloadInfoJson["result"][0]["codec"].ToString();
            var downloadInfoUrl = downloadInfoJson["result"][0]["downloadInfoUrl"].ToString();
            trackModel.Uri = new Uri (await _track.GetDirectLink(downloadInfoUrl));

            return trackModel;
        }
            
    }
}
