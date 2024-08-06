using MusicBot.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMusicApi.Api;
using YandexMusicApi.ComponentModels;
using YandexMusicApi.Network;

namespace MyMusicBot.Services
{
    public class SearchService
    {
        public SearchService(
            ConfigManager configManager
            ) { }
        public async static Task<TrackModel> SearchTrack(string query)
        {
            TrackModel trackModel = null;
            var _apiParams = new ApiParams();
            _apiParams.TokenYandexMusic = ConfigManager.Config.YaToken;
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
                trackModel.Uri = await _track.GetDirectLink(downloadInfoUrl);

            return trackModel;
        }
    }
}
