using Newtonsoft.Json;

namespace MusicBot.Services
{
    public class JsonReader
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string YaToken { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("C:\\Users\\snetk\\source\\repos\\MyMusicBot\\MyMusicBot\\Config\\BotCfg.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.Token = data.Token;
                this.Prefix = data.Prefix;
                this.YaToken = data.YaToken;
            }
        }
        
    }
    internal sealed class JSONStructure
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string YaToken {  get; set; }
    }
}
