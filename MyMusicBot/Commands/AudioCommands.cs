using Discord;
using Discord.Commands;
using MyMusicBot.Services;

namespace MyMusicBot.Commands
{
    public class AudioCommands : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        public async Task JoinAndPlay()
            => await ReplyAsync(embed: await AudioManager.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));
    }
}
