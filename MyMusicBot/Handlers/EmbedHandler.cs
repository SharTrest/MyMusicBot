using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMusicBot.Handlers
{
    public static class EmbedHandler
    {
        public static async Task<DiscordEmbedBuilder> CreateBasicEmbed(string title, string description, DiscordColor color)
        {
            var embed = await Task.Run(() =>
            (
                new DiscordEmbedBuilder()
                {
                    Title = title,
                    Description = description,
                    Color = color,
                    Timestamp = DateTime.Now,
                }
            ));
                    return embed;
              
        }

        public  static async Task<DiscordEmbed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new DiscordEmbedBuilder()
            {
                Title = $"Ошибка вознила в {source}",
                Description = $"Описание: {error}",
                Color = DiscordColor.DarkRed,
                Timestamp = DateTime.Now,
            }.Build()
            );
            return embed;
        }
    }
}
