using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuantumBotv2.Commands
{
    public class SlashCommandLogic
    {
        private static SlashCommandLogic instance = null;
        public static SlashCommandLogic Instance
        {
            get
            {
                return instance;
            }
        }

        public SlashCommandLogic()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public async Task SlashPingCommand(SocketSlashCommand command)
        {
            SocketGuildUser guildUser = (SocketGuildUser)command.Data.Options.First().Value;
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build());
        }

    }
}
