using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusicBot.Models
{
    public class BotModel
    {
        public string DiscordToken { get; set; }
        public string YaToken { get; set; }
        public string DefaultPrefix { get; set; }
        public string GameStatus { get; set; }
        public List<ulong> BlackListedChannels {  get; set; } 
    }
}
