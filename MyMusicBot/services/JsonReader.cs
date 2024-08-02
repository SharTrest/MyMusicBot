using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.services
{
    public class JsonReader
    {
        public string Token { get; set; }
        public string Prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("C:\\Users\\snetk\\source\\repos\\DiscordBot\\DiscordBot\\config\\BotCfg.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.Token = data.Token;
                this.Prefix = data.Prefix;
            }
        }
        
    }
    internal sealed class JSONStructure
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
