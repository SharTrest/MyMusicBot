using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexMusicApi.Api;
using static System.Net.WebRequestMethods;

namespace MusicBot.models
{
    public class TrackModel
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Url { get; set; }
        public string Album { get; set; }
        public Uri Uri { get; set; }
        public int TrackID { get; set; }
        public int AlbumID { get; set; }
        public JToken JToken { get; set; }

        public TrackModel(JToken i)
        {
            TrackID = Convert.ToInt32(i["track"]["id"].ToString());
            Artist = i["track"]["artists"][0]["name"].ToString();
            Album = i["track"]["albums"][0]["title"].ToString();
            AlbumID = Convert.ToInt32(i["track"]["albums"][0]["id"].ToString());
            Title = i["track"]["title"].ToString();
            Url = $"https://music.yandex.ru/album/{AlbumID}/track/{TrackID}";
            JToken = i;
        }

    }
}
