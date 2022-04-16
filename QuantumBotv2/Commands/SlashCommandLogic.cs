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
                                    $"monsterhunter-nickname-add - Give a monster a nickname\n" +
                                    $"monsterhunter-nickname-remove - Remove a nickname from a monster\n" +
                                    $"monsterhunter-nickname-view - View monster nicknames")
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
            string inputGameCode = command.Data.Options.ElementAt(1).Value.ToString();
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


        public async Task AddMonsterHunterNickname(SocketSlashCommand command)
        {
            List<MonsterHunterNicknames.MonsterNicknames> allMonsterHunterNicknames = DataClassManager.Instance.monsterHunterNicknames.allMonsterHunterNicknames;

            string inputMonsterName = command.Data.Options.ElementAt(0).Value.ToString();
            string inputMonsterNickname = command.Data.Options.ElementAt(1).Value.ToString();

            /*
             * -> Find Monster with the same Name
             *      -> Does the nickname already exist?
             *          -> Add Nickname, print
             *          !-> Alert, print 
             * !-> Doesn't exist, print
            */

            //Try and find a matching Monster Name
            foreach (MonsterHunterNicknames.MonsterNicknames monsterNicknames in allMonsterHunterNicknames)
            {
                if (monsterNicknames.monsterName.ToLower() == inputMonsterName.ToLower() || monsterNicknames.monsterName.ToLower() == inputMonsterName.ToLower().Replace(' ', '-'))
                {
                    //we found our monster

                    //If the nickname already exists, we can't add it
                    if (monsterNicknames.nicknameData.ContainsKey(inputMonsterNickname) == true)
                    {
                        //ALERT THE USER THE NICKNAME EXISTS
                        await command.RespondAsync($"The nickname \"{inputMonsterNickname}\" for \"{inputMonsterName}\" already exists!", ephemeral: true);
                        return;
                    }
                    else
                    {
                        monsterNicknames.nicknameData.Add(inputMonsterNickname, command.User.Id);
                        DataClassManager.Instance.SaveData(DataClassManager.Instance.monsterHunterNicknames);

                        await command.RespondAsync(embed: CreateMonsterNicknameEmbed(monsterNicknames, command));
                        return;
                    }
                }
            }

            //No Monster with that name found, alert
            await command.RespondAsync($"Couldn't find a monster with the name \"{inputMonsterName}\" \nCheck the supported monster list with the \"monsterhunter-nickname-view\" command", ephemeral: true);
        }

        //NEEDS TESTING
        public async Task RemoveMonsterHunterNickname(SocketSlashCommand command)
        {
            List<MonsterHunterNicknames.MonsterNicknames> allMonsterHunterNicknames = DataClassManager.Instance.monsterHunterNicknames.allMonsterHunterNicknames;

            string inputMonsterName = command.Data.Options.ElementAt(0).Value.ToString();
            string inputMonsterNickname = command.Data.Options.ElementAt(1).Value.ToString();

            /*
             * -> Find Monster with the same Name
             *      -> Does the nickname exist?
             *          -> Check if original author
             *              -> Remove Nickname, print
                            !-> Alert
             *          !-> Alert, print 
             * !-> Doesn't exist, print
            */

            //Try and find a matching Monster Name
            foreach (MonsterHunterNicknames.MonsterNicknames monsterNicknames in allMonsterHunterNicknames)
            {
                if (monsterNicknames.monsterName.ToLower() == inputMonsterName.ToLower() || monsterNicknames.monsterName.ToLower() == inputMonsterName.ToLower().Replace(' ', '-'))
                {
                    //we found our monster

                    //If the nickname already exists, remove it
                    if (monsterNicknames.nicknameData.ContainsKey(inputMonsterNickname) == true)
                    {
                        //Check if the person trying to remove it is also the original author
                        if (monsterNicknames.nicknameData[inputMonsterName] == command.User.Id)
                        {
                            monsterNicknames.nicknameData.Remove(inputMonsterName);
                            DataClassManager.Instance.SaveData(DataClassManager.Instance.monsterHunterNicknames);

                            await command.RespondAsync($"The nickname \"{inputMonsterNickname}\" for \"{inputMonsterName}\" removed!", ephemeral: true);
                        }
                        else
                        {
                            await command.RespondAsync($"Can't remove the nickname \"{inputMonsterNickname}\" for \"{inputMonsterName}\"\nOnly the author of the nickname can delete that nickname", ephemeral: true);
                        }
                        return;
                    }
                    else //Nickname doesn't exist
                    {
                        await command.RespondAsync($"The nickname \"{inputMonsterNickname}\" for \"{inputMonsterName}\" doesn't exist!", ephemeral: true);
                        return;
                    }
                }
            }

            //No Monster with that name found, alert
            await command.RespondAsync($"Couldn't find a monster with the name \"{inputMonsterName}\" \nCheck the supported monster list with the \"monsterhunter-nickname-view\" command", ephemeral: true);
        }

        /*         
        public async Task ViewMonsterHunterNickname(SocketSlashCommand command)
                {
                    //show all or just the monsters

                } 
        */

        public async Task PurgeMessagesFromChannel(SocketSlashCommand command)
        {
            if (await SlashCommandUserHasRoles(new string[] { "Admin" }, command) == false)
            {
                return;
            }

            int purgeAmount = Convert.ToInt32((Int64)command.Data.Options.First().Value);

            if (purgeAmount > 100)
            {
                purgeAmount = 100;
            }
            else if (purgeAmount < 0)
            {
                purgeAmount = 0;
            }

            var messages = await command.Channel.GetMessagesAsync(purgeAmount).FlattenAsync();
            await (command.Channel as ITextChannel).DeleteMessagesAsync(messages);

            await (command.User as SocketGuildUser).Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Admin"]).SendMessageAsync($"User: <@{command.User.Id}> purged {purgeAmount} messages in the <#{command.Channel.Id}>");

            await command.RespondAsync($"Succesfully purged {purgeAmount} messages from <#{command.Channel.Id}>", ephemeral: true);
        }
        public async Task QuitBot(SocketSlashCommand command)
        {
            if (await SlashCommandUserHasRoles(new string[] { "Admin" }, command) == false)
            {
                return;
            }

            await command.RespondAsync("I'll be back - Gandhi\nhttps://media.giphy.com/media/gFwZfXIqD0eNW/giphy.gif");
            await (command.User as SocketGuildUser).Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Admin"]).SendMessageAsync($"<@{command.User.Id}> just pulled the plug. Good bye <:SadCat:656612740718133289>");

            await Task.Delay(5000);

            System.Environment.Exit(1);
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

        private Discord.Embed CreateMonsterNicknameEmbed(MonsterHunterNicknames.MonsterNicknames monsterData, SocketSlashCommand command)
        {
            EmbedBuilder builder = new EmbedBuilder();
            try
            {
                builder = new EmbedBuilder()
                                        .WithTitle("Monster Hunter Nicknames!~")
                                        .WithDescription($"Nicknames for {monsterData.monsterName}")
                                        .WithThumbnailUrl(monsterData.monsterIconURL);
            }
            catch (ArgumentException) //incase a URL is dead
            {
                builder = new EmbedBuilder()
                                        .WithTitle("Monster Hunter Nicknames!~")
                                        .WithDescription($"Nicknames for {monsterData.monsterName}\nURL is dead, please alert an Admin");
            }

            Dictionary<ulong, string> fieldContents = new Dictionary<ulong, string>();

            foreach (KeyValuePair<string, ulong> nicknamesKVP in monsterData.nicknameData)
            {
                if (fieldContents.ContainsKey(nicknamesKVP.Value))
                {
                    fieldContents[nicknamesKVP.Value] += $"\n{nicknamesKVP.Key}";
                }
                else
                {
                    fieldContents.Add(nicknamesKVP.Value, nicknamesKVP.Key);
                }
            }

            if (fieldContents.Count() == 0)
            {
                builder.AddField($"No nicknames found <:SadCat:656612740718133289>", $"Use the \"monsterhunter-nicknames-add\" slash command!");
            }

            foreach (KeyValuePair<ulong, string> creatorKVP in fieldContents)
            {
                SocketGuildUser guildUser = ((SocketGuildChannel)command.Channel).Guild.GetUser(creatorKVP.Key);

                if (guildUser != null)
                {
                    if (guildUser.Nickname == "" || guildUser.Nickname == null)
                    {
                        builder.AddField($"By {guildUser.Username}#{guildUser.Discriminator}", creatorKVP.Value);
                    }
                    else
                    {
                        builder.AddField($"By {guildUser.Nickname}", creatorKVP.Value);
                    }
                }
                else
                {
                    builder.AddField($"By UserNotFound", creatorKVP.Value);
                }
            }

            return builder.Build();
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
