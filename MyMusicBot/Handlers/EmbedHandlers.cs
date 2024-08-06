using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusicBot.Handlers
{
    public static class EmbedHandler
    {
        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp().Build();
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed =  new EmbedBuilder()
                .WithTitle($"ОШИБКА: {source}")
                .WithDescription($"ОПИСАНИЕ ОШИБКИ: \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp().Build();
            return embed;
        }
    }
}
