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

                userData.UserGameCodeIndex[inputGamePlatformEnum] = inputGameCode;

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

        public async Task<bool> SlashCommandUserHasRoles(string[] roles, SocketSlashCommand command)
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

            await command.RespondAsync($"You do not have have the required roles to use this command.", ephemeral: true);
            return false;
        }

    }
}
