using Newtonsoft.Json;

namespace MyMusicBot.Services
{
    public static class ConfigManager
    {
        private static string ConfigFolder = "Config";
        private static string ConfigFile = "BotCfg.json";
        private static string ConfigPath = ConfigFolder + "/" + ConfigFile;
        public static BotConfig Config { get; private set; }

        static ConfigManager()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            if (!File.Exists(ConfigPath))
            {
                Config = new BotConfig();
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                Console.WriteLine("Файл с конфигурацией бота создан");
            }
            else
            {
                var json = File.ReadAllText(ConfigPath);
                Config = JsonConvert.DeserializeObject<BotConfig>(json);
                Console.WriteLine(Config.Token);
            }
        }
    }

    public struct BotConfig
    {
        [JsonProperty("Token")]
        public string Token { get; private set; }
        [JsonProperty("Prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("YaToken")]
        public string YaToken { get; private set; }
    }
}
