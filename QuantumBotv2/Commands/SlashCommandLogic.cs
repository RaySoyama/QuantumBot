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

        public async Task Ping(SocketSlashCommand command)
        {
            await command.RespondAsync($"{Program.clientPing} MS", ephemeral: true);
            return;
        }

        public async Task SendBotHelpMessage(SocketSlashCommand command)
        {
            SocketGuildUser guildUser = (SocketGuildUser)command.User;

            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot - Commands")
                          .WithDescription($"Ping one of the moderators, or <@{DataClassManager.Instance.serverConfigs.userID["Ray Soyama"]}> if you have any questions!\n" +
                                            //$"Current Prefix is \"{DataClassManager.Instance.serverConfigs.prefix}\"\n" +
                                            $"Almost all of the commands have been translated to SlashCommands! Try them out by typing /")
                          .AddField("General",
                                    $"ping - returns current bot latency\n" +
                                    $"help - DM's a the message you are reading right now")
                            .AddField("Game Codes",
                                     $"game-code-add - Add a game code\n" +
                                     $"game-code-remove - Remove a game code\n" +
                                     $"game-code-view - View a users game codes\n")
                            .AddField("Monster Hunter World + Rise",
                                    $"add-monster-nickname - Give a monster a nickname\n" +
                                    $"remove-monster-nickname - Remove a nickname from a monster\n" +
                                    $"view-monster-nickname - View all nicknames given to a monster \n" +
                                    $"view-monsters - View all Monsters\n")
                            .AddField("Admin",
                                     $"admin-send-intro-message - Sends the user the introduction msg\n" +
                                     $"admin-purge - Mass deletes messages in a channel\n" +
                                     $"admin-quit - Bot commits Seppuku")
                          .WithColor(new Color(60, 179, 113));

            await guildUser.SendMessageAsync("", embed: builder.Build());
            await command.RespondAsync($"A DM with commands sent!", ephemeral: true);
        }


        public async Task AddGameCodeCommand(SocketSlashCommand command)
        {
            UserProfile.UserData userData = DataClassManager.Instance.userProfile.GetUserData((SocketGuildUser)command.User);

            //lierally disgusting
            int inputGamePlatform = Convert.ToInt32((Int64)command.Data.Options.ElementAt(0).Value);
            string inputGameCode = (string)command.Data.Options.ElementAt(1).Value;
            UserProfile.UserData.GamePlatforms inputGamePlatformEnum = (UserProfile.UserData.GamePlatforms)inputGamePlatform;

            SocketGuildUser guildUser = (SocketGuildUser)command.User;

            if (userData.UserGameCodeIndex.ContainsKey(inputGamePlatformEnum)) //Stomp over existing value
            {
                EmbedBuilder embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Game Codes")
                .AddField($"{inputGamePlatformEnum.ToString()}", $"~~{userData.UserGameCodeIndex[inputGamePlatformEnum]}~~ {inputGameCode}")
                .WithCurrentTimestamp();

                userData.UserGameCodeIndex[inputGamePlatformEnum] = inputGameCode;

                await command.RespondAsync(embed: embedBuiler.Build());
            }
            else
            {
                userData.UserGameCodeIndex.Add(inputGamePlatformEnum, inputGameCode);

                EmbedBuilder embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Game Codes")
                .AddField($"{inputGamePlatformEnum.ToString()}", $":star2: NEW! :star2: {inputGameCode}")
                .WithCurrentTimestamp();

                userData.UserGameCodeIndex.Add(inputGamePlatformEnum, inputGameCode);

                await command.RespondAsync(embed: embedBuiler.Build());
            }

            DataClassManager.Instance.SaveData(DataClassManager.Instance.userProfile);
        }

        public async Task RemoveGameCodeCommand(SocketSlashCommand command)
        {
            UserProfile.UserData userData = DataClassManager.Instance.userProfile.GetUserData((SocketGuildUser)command.User);

            //lierally disgusting
            int inputGamePlatform = Convert.ToInt32((Int64)command.Data.Options.ElementAt(0).Value);
            UserProfile.UserData.GamePlatforms inputGamePlatformEnum = (UserProfile.UserData.GamePlatforms)inputGamePlatform;

            SocketGuildUser guildUser = (SocketGuildUser)command.User;

            if (userData.UserGameCodeIndex.ContainsKey(inputGamePlatformEnum)) //key exists
            {
                EmbedBuilder embedBuiler = new EmbedBuilder()
                                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                                .WithTitle("Game Codes")
                                .AddField($"{inputGamePlatformEnum.ToString()}", $"~~{userData.UserGameCodeIndex[inputGamePlatformEnum]}~~ Removed!")
                                .WithCurrentTimestamp();

                await command.RespondAsync(embed: embedBuiler.Build());

                userData.UserGameCodeIndex.Remove(inputGamePlatformEnum);
                DataClassManager.Instance.SaveData(DataClassManager.Instance.userProfile);
            }
            else
            {
                await command.RespondAsync($"No Code found for {inputGamePlatformEnum.ToString()}", ephemeral: true);
            }
        }

        public async Task ViewGameCodeCommand(SocketSlashCommand command)
        {
            SocketGuildUser guildUser = (SocketGuildUser)command.Data.Options.First().Value;
            UserProfile.UserData userData = DataClassManager.Instance.userProfile.GetUserData(guildUser);

            if (userData.UserGameCodeIndex.Count == 0) //No Game Codes
            {
                await command.RespondAsync($"No Codes found for User <@{guildUser.Id}>", ephemeral: true);
            }
            else
            {
                EmbedBuilder embedBuiler = new EmbedBuilder()
                                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                                .WithTitle("Game Codes")
                                .WithCurrentTimestamp();


                foreach (KeyValuePair<UserProfile.UserData.GamePlatforms, string> kvp in userData.UserGameCodeIndex)
                {
                    embedBuiler.AddField($"{kvp.Key.ToString()}", $"{kvp.Value}");
                }

                await command.RespondAsync(embed: embedBuiler.Build());
            }
        }





        public async Task UpdateUserProfilesCommand(SocketSlashCommand command)
        {
            if (await SlashCommandUserHasRoles(new string[] { "Admin" }, command) == false)
            {
                return;
            }

            var allUsers = await ((IGuild)((SocketGuildUser)command.User).Guild).GetUsersAsync();

            foreach (IGuildUser user in allUsers)
            {
                DataClassManager.Instance.userProfile.GetUserData((SocketGuildUser)user);
            }

            await command.RespondAsync("Succesfully Updated All User Profiles ", ephemeral: true);
        }

        public async Task SendServerIntroMessage(SocketSlashCommand command)
        {
            if (await SlashCommandUserHasRoles(new string[] { "Admin" }, command) == false)
            {
                return;
            }

            SocketGuildUser guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            await Program.SendIntroductionMessage(guildUser);
            await command.RespondAsync($"Server Intro Message Sent to <@{guildUser.Id}>", ephemeral: true);
        }






        private async Task<bool> SlashCommandUserHasRoles(string[] roles, SocketSlashCommand command)
        {
            SocketGuildUser guildUser = (SocketGuildUser)command.User;

            foreach (string role in roles)
            {
                //get role from database
                var guildRole = guildUser.Guild.GetRole(DataClassManager.Instance.serverConfigs.roleID[role]);

                if (guildUser.Roles.Contains(guildRole))
                {
                    return true;
                }
            }

            await command.RespondAsync($"You do not have have the required roles to use this command. ({string.Join(", ", roles)}). \nMessage a Admin if you think this is wrong", ephemeral: true);
            return false;
        }

        /// <summary>
        /// Call when you run a SlashCommand to add logging in Bot-History and logs
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task OnSlashCommandInvoked(SocketSlashCommand command, bool isFailed)
        {
            SocketGuildUser guildUser = (SocketGuildUser)command.User;

            var embedBuiler = new EmbedBuilder()
                .WithThumbnailUrl(guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithAuthor(guildUser.ToString())
                .WithCurrentTimestamp();

            if (isFailed)
            {
                embedBuiler.WithTitle($":warning: Failed Slash Command Invoked! :warning:");
            }
            else
            {
                embedBuiler.WithTitle($"Slash Command Invoked!");
            }

            string commandString = $"{command.Data.Name} ";

            foreach (var arg in command.Data.Options)
            {
                commandString += $"\"{arg.Value.ToString()}\" ";
            }

            commandString += $"\nChannel: <#{command.Channel.Id}>";
            embedBuiler.WithDescription(commandString);

            await guildUser.Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Bot History"]).SendMessageAsync(embed: embedBuiler.Build());


            //Not RAM friendly
            DataClassManager.Instance.telemetryLog.allCommandTelemetryLogs.Add(new TelemetryLog.CommandTelemetryData(command));
            DataClassManager.Instance.SaveData(DataClassManager.Instance.telemetryLog);
        }
    }
}
