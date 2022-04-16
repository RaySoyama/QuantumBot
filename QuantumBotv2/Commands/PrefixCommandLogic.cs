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
    public class PrefixCommandLogic : ModuleBase<SocketCommandContext>
    {
        [Command("CreateRoleMessage"), Alias("CreateRoleMessage"), Summary("Creates a Message with buttons")]
        public async Task CreateRoleMessage()
        {
            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

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

        [Command("DM"), Summary("Sends a DM to a user on behalf of Quantum Bot")]
        public async Task SendDM(ulong targetUser, [Remainder] string msg)
        {
            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

            await Context.Guild.GetUser(targetUser).SendMessageAsync(msg);
        }

        [Command("Spam"), Summary("Spams a copy of a message in the channel")]
        public async Task SpamMsg(int count, [Remainder] string msg)
        {
            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

            await Context.Message.ReplyAsync("💀");

            for (int i = 0; i < count; i++)
            {
                await Context.Channel.SendMessageAsync(msg);
            }
        }

        [Command("Quit"), Alias("quit"), Summary("Quits the bot exe")]
        public async Task QuitBot()
        {
            if (await PrefixCommandIsDepricated())
            {
                return;
            }

            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

            await Context.Message.Channel.SendMessageAsync("I'll be back - Gandhi\nhttps://media.giphy.com/media/gFwZfXIqD0eNW/giphy.gif");
            await (Context.User as SocketGuildUser).Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Admin"]).SendMessageAsync($"<@{Context.User.Id}> just pulled the plug. Good bye <:SadCat:656612740718133289>");
            await Task.Delay(5000);

            System.Environment.Exit(1);
        }

        /*         [Command("ADMINTESTING")]
                public async Task ADMINTEST()
                {
                    IUserMessage message = (IUserMessage)await Context.Guild.GetTextChannel(487666653690200064).GetMessageAsync(675211861708374046);

                    var builder = new EmbedBuilder()
                                .WithTitle("Depricated Role Assignment Message")
                                .WithDescription("The reactions are cute so I'm leaving this here :)");
                    await message.ModifyAsync(x => x.Embed = builder.Build());
                } */

        #region Depricated Prefix Commands
        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]
        public async Task Ping()
        {
            if (await PrefixCommandIsDepricated())
            {
                return;
            }

            var botMsg = await Context.Message.ReplyAsync($"{Program.clientPing} MS");

            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await botMsg.DeleteAsync();
            return;
        }

        [Command("Help"), Alias("help"), Summary("DM's list of all commands")]
        public async Task SendBotHelpMessage()
        {
            if (await PrefixCommandIsDepricated())
            {
                return;
            }

            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot - Commands")
                          .WithDescription($"Ping one of the moderators, or <@{DataClassManager.Instance.serverConfigs.userID["Ray Soyama"]}> if you have any questions!\n" +
                                            //$"Current Prefix is \"{DataClassManager.Instance.serverConfigs.prefix}\"\n" +
                                            $"Almost all of the commands have been translated to SlashCommands! Try them out by typing /")
                          .AddField("General",
                                    $"ping - returns current bot latency\n")
                            .AddField("Game Codes",
                                     $"add-game-code - Add a game code\n" +
                                     $"remove-game-code - Remove a game code\n" +
                                     $"view-game-code - View a users game codes\n")
                            .AddField("Monster Hunter World + Rise",
                                    $"add-monster-nickname - Give a monster a nickname\n" +
                                    $"remove-monster-nickname - Remove a nickname from a monster\n" +
                                    $"view-monster-nickname - View all nicknames given to a monster \n" +
                                    $"view-monsters - View all Monsters\n")
                            .AddField("Admin",
                                     $"send-intro-message - Sends the user the introduction msg\n" +
                                     $"purge-messages - Mass deletes messages in a channel\n" +
                                     $"quit-bot - Bot commits Seppuku")
                          .WithColor(new Color(60, 179, 113));

            var embed = builder.Build();
            await Context.User.SendMessageAsync("", embed: embed);
            await Context.Message.DeleteAsync();

            return;
        }

        [Command("SendIntro"), Summary("DM's the Server intro Message to user")]
        public async Task SendServerIntroMessage(params string[] userList)
        {
            if (await PrefixCommandIsDepricated())
            {
                return;
            }

            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

            foreach (var targetUser in Context.Message.MentionedUsers)
            {
                await Program.SendIntroductionMessage((SocketGuildUser)targetUser);
                await Context.Message.ReplyAsync($"Server Intro Message Sent to <@{targetUser.Id}>");
            }
        }

        [Command("Purge", RunMode = RunMode.Async)]
        public async Task PurgeMessagesFromChannel(int purgeAmount)
        {
            if (await PrefixCommandIsDepricated())
            {
                return;
            }

            if (await PrefixCommandUserHasRoles(new string[] { "Admin" }) == false)
            {
                return;
            }

            if (purgeAmount > 100)
            {
                purgeAmount = 100;
            }
            else if (purgeAmount < 0)
            {
                purgeAmount = 0;
            }

            var messages = await Context.Channel.GetMessagesAsync(purgeAmount).FlattenAsync();
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);

            await (Context.User as SocketGuildUser).Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Admin"]).SendMessageAsync($"User: <@{Context.User.Id}> purged {purgeAmount} messages in the <#{Context.Channel.Id}>");

            //await Context.Message.ReplyAsync($"Succesfully purged {purgeAmount} messages from <#{Context.Channel.Id}>");
        }

        #endregion

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

        private async Task<bool> PrefixCommandIsDepricated()
        {
            var botMsg = await Context.Message.ReplyAsync($"This Command is now depricated and is no longer supported. Try the new slash command version!");

            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await botMsg.DeleteAsync();
            return true;
        }
        private async Task<bool> PrefixCommandUserHasRoles(string[] roles)
        {
            SocketGuildUser guildUser = (SocketGuildUser)Context.User;

            foreach (string role in roles)
            {
                //get role from database
                var guildRole = guildUser.Guild.GetRole(DataClassManager.Instance.serverConfigs.roleID[role]);

                if (guildUser.Roles.Contains(guildRole))
                {
                    return true;
                }
            }

            await Context.Message.ReplyAsync($"You do not have have the required roles to use this command. ({string.Join(", ", roles)}). \nMessage a Admin if you think this is wrong");
            return false;
        }
    }
}
