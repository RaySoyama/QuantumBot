using Discord;
using Discord.Commands;
using Discord.WebSocket;
using QuantumBotv2.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuantumBotv2.Commands
{
    public class AdminCommandLogic : ModuleBase<SocketCommandContext>
    {
        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]
        public async Task Ping()
        {
            //var msg = await Context.Message.Channel.SendMessageAsync($"MS {Program.latency}");
            var botMsg = await Context.Message.Channel.SendMessageAsync($"{Context.Client.Latency} MS");

            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await botMsg.DeleteAsync();
            return;
        }

        [Command("CreateRoleMessage"), Alias("CreateRoleMessage"), Summary("Creates a Message with buttons")]
        public async Task CreateRoleMessage()
        {
            var embed = new EmbedBuilder()
                        .WithDescription("React with these emotes to get access to the corresponding channels!~\n(You need to be a Student or a Guest)")
                        .WithAuthor("Pointers Anonymous Role Selection")
                        .WithThumbnailUrl("https://cdn.discordapp.com/icons/487403414741975040/a_c01491057777dfa5b5313a65868fd1a4.webp?size=128");

            var builder = new ComponentBuilder()
                .WithButton("Programming", "programming-button")
                .WithButton("Art", "art-button")
                .WithButton("Design", "design-button")
                .WithButton("TechArt", "techart-button");

            await ReplyAsync(embed: embed.Build(), components: builder.Build());
        }

        public static async Task OnCommandInvoked(SocketCommandContext context)
        {
            SocketGuildUser guildUser = (SocketGuildUser)context.User;

            var embedBuiler = new EmbedBuilder()
                .WithThumbnailUrl(guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Prefix Command Invoked!")
                .WithAuthor(guildUser.ToString())
                .WithDescription($"{context.Message.Content}\nChannel: <#{context.Channel.Id}>")
                .WithCurrentTimestamp();

            await guildUser.Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Bot History"]).SendMessageAsync(embed: embedBuiler.Build());

            //Not RAM friendly
            DataClassManager.Instance.telemetryLog.allCommandTelemetryLogs.Add(new TelemetryLog.CommandTelemetryData(context));
            DataClassManager.Instance.SaveData(DataClassManager.Instance.telemetryLog);
        }
    }
}
