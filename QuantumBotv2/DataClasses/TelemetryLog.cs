using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;
using Discord.Commands;

namespace QuantumBotv2.DataClass
{
    public class TelemetryLog : DataClass
    {
        public override string FileName()
        {
            return "TelemetryLog.json";
        }

        public class CommandTelemetryData
        {
            public enum CommandTypes
            {
                prefix = 1,
                slash = 2,
                button = 3,
            }

            public CommandTypes commandType = CommandTypes.prefix;
            public string authorName = "NoAuthorName";
            public string authorNickname = "NoAuthorNickname";
            public ulong authorID = 6969696969696969;
            public DateTimeOffset timestamp = DateTimeOffset.Now;
            public string channelName = "NoChannelName";
            public ulong channelID = 6969696969696969;
            public string commandName = "NoCommandName";
            public List<string> commandArguments = new List<string>();

            public CommandTelemetryData()
            {
            }

            public CommandTelemetryData(SocketSlashCommand command) //slash commands
            {
                SocketGuildUser guildUser = (SocketGuildUser)command.User;
                commandType = CommandTypes.slash;

                authorName = $"{guildUser.Username}#{guildUser.Discriminator}";
                authorNickname = $"{guildUser.Nickname}";
                authorID = guildUser.Id;
                timestamp = command.CreatedAt;
                channelName = command.Channel.Name;
                channelID = command.Channel.Id;

                commandName = command.Data.Name;

                foreach (var arg in command.Data.Options)
                {
                    commandArguments.Add($"{arg.Value.ToString()}");
                }
            }
            public CommandTelemetryData(SocketMessageComponent component) //Button commands
            {
                SocketGuildUser guildUser = (SocketGuildUser)component.User;

                commandType = CommandTypes.button;

                authorName = $"{guildUser.Username}#{guildUser.Discriminator}";
                authorNickname = $"{guildUser.Nickname}";
                authorID = guildUser.Id;
                timestamp = component.CreatedAt;
                channelName = component.Channel.Name;
                channelID = component.Channel.Id;

                commandName = component.Data.CustomId;
            }
            public CommandTelemetryData(SocketCommandContext context) //prefix commands
            {
                SocketGuildUser guildUser = (SocketGuildUser)context.User;

                commandType = CommandTypes.prefix;

                authorName = $"{guildUser.Username}#{guildUser.Discriminator}";
                authorNickname = $"{guildUser.Nickname}";
                authorID = guildUser.Id;
                timestamp = context.Message.CreatedAt;
                channelName = context.Channel.Name;
                channelID = context.Channel.Id;

                //TODO: implement a parser to seperate args?
                commandName = context.Message.Content;
                //throw new NotImplementedException();
            }
        }
        public List<CommandTelemetryData> allCommandTelemetryLogs = new List<CommandTelemetryData>();

        public int NumberOfCommandInvokesFromUser(ulong userId)
        {
            int invkCount = 0;
            foreach (CommandTelemetryData commandTelemetryData in allCommandTelemetryLogs)
            {
                if (commandTelemetryData.authorID == userId)
                {
                    invkCount++;
                }
            }
            return invkCount;
        }
    }
}
