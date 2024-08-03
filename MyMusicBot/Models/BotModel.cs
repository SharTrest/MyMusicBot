namespace MyMusicBot.Models
{
    public class BotModel
    {
        public string? DiscordToken { get; set; }
        public string? YaToken { get; set; }
        public string? DefaultPrefix { get; set; }
        public string? GameStatus { get; set; }
        public List<ulong>? BlacklistedChannels {  get; set; } 
    }
}
