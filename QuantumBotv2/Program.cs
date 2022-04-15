using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using QuantumBotv2.DataClass;
using QuantumBotv2.Commands;
using System.Collections.Generic;

namespace QuantumBotv2
{
    class Program
    {
        /*
        TODO
        *Logging - User Messages
        *Logging - Server Logs, Seperate
        Logging - Split Via Month? Week? Day?

        *UserProfiles
            *- GameCodes
                *- Add Game Codes
                *- Remove Game Codes
                *- View Game Codes

        Telemetry
            Bot Uptime?
            User Messages Per Chat
            User Command Usage
        OnSlashCommands
            - Log Slash Commands
        OnButtonClicked
            - Log Button Clicks, Role Updates


        When User Joins
            Send Msg to "I am Logs"
            Create Profile

        When Username changed?
            Update Profile?

        *When User Leaves
            *Log out data

        Reaction Added Set Up
        Reaction Removed Set Up
        
        -Message Receieved
            - Update Latency Logging/Ping
            *- When someone DM's the Bot, relay to Ray. Add edge case If Ray msm Bot
        
        Custom Keywords
            - Happy Birthday
            - Good Bot

        Ability to sync files through a command
        Ability to Spit out files through a command
        Ability to Update files through a command

        Custom Data Class's
            ReactEmote
                ServerRoleSetUpMsgID
            Monster Hunter
                Monster Hunter Monster Fail Count
                    Generic
                    Add/Subtract
                MonsterHunterNicknames
            Blackout Queue
            ChannelRoles (for react to get roles)
                add ephemeral response support

            
            (Depricated) WebsiteProfile
            (Depricated) Lunchbox
            (Depricated) BullietinEvent
            (Depricated) BulletinBoard
            (Depricated) Accountabilitbuddy

        */


        private static DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        private async Task MainAsync()
        {
            //Init Data
            DataClassManager dataClassManager = new DataClassManager();

            /*
            TelemetryLog test = new TelemetryLog();
            DataClassManager.Instance.SaveData(test);
            return;
            */

            DataClassManager.Instance.LoadAllData();

            //Init Command Logic Singletons
            SlashCommandLogic slashCommandLogic = new SlashCommandLogic();
            ButtonCommandLogic buttonCommandLogic = new ButtonCommandLogic();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true,
                UseInteractionSnowflakeDate = false,
                ConnectionTimeout = 60000
            });

            commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Critical
            });

            client.Log += OnClientLog;

            await client.LoginAsync(TokenType.Bot, DataClassManager.Instance.serverConfigs.Token);

            //attach event listeners
            await client.StartAsync();
            client.Ready += OnClientIsReady;
            client.MessageReceived += OnMessageReceived;
            client.SlashCommandExecuted += OnSlashCommandCalled;
            client.UserJoined += OnUserJoined;
            client.UserLeft += OnUserLeft;

            client.ButtonExecuted += OnButtonClicked;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            services = new ServiceCollection().BuildServiceProvider();

            await Task.Delay(-1);
        }

        private Task OnClientLog(LogMessage logMessage)
        {
            //Stores in RAM, less read/write
            /*
            ClientLog.ClientLogData newClientLogData = new ClientLog.ClientLogData(logMessage);
            DataClassManager.Instance.clientLog.allClientLogs.Add(newClientLogData);
            DataClassManager.Instance.SaveData(DataClassManager.Instance.clientLog);
            Console.WriteLine(DataClassManager.Instance.clientLog.ClientLogDataAsString(newClientLogData));
            */

            //Less RAM Usage
            //Read Data from File, Save, Delete Cache?
            DataClassManager.Instance.clientLog = DataClassManager.Instance.LoadData(DataClassManager.Instance.clientLog);

            ClientLog.ClientLogData newClientLogData = new ClientLog.ClientLogData(logMessage);
            DataClassManager.Instance.clientLog.allClientLogs.Add(newClientLogData);

            DataClassManager.Instance.SaveData(DataClassManager.Instance.clientLog);
            Console.WriteLine(DataClassManager.Instance.clientLog.ClientLogDataAsString(newClientLogData));

            DataClassManager.Instance.clientLog = new ClientLog();

            return Task.CompletedTask;
        }
        private async Task OnClientIsReady()
        {
            await client.SetGameAsync($"{DataClassManager.Instance.serverConfigs.prefix}Help");
            await LoadSlashCommands();
        }
        private async Task LoadSlashCommands()
        {
            var guild = client.GetGuild(DataClassManager.Instance.serverConfigs.serverID);

            /*
            //Deleting Global Commands
            var globalCommands = await client.GetGlobalApplicationCommandsAsync();
            foreach (var globCmd in globalCommands)
            {
                await globCmd.DeleteAsync();
            }
            */

            //when rebooting the bot, delete and remake all slash commands
            await guild.DeleteApplicationCommandsAsync();

            try
            {
                // With global commands we don't need the guild.
                SlashCommands slashCommands = DataClassManager.Instance.slashCommands;
                foreach (SlashCommands.SlashCommandData cmdData in slashCommands.allSlashCommands)
                {
                    await guild.CreateApplicationCommandAsync(cmdData.slashCommandBuilder.Build());
                }
            }
            catch (Exception exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.ToString(), Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        private async Task OnMessageReceived(SocketMessage message)
        {
            SocketUserMessage userMessage = message as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(client, userMessage);

            if (context.User.IsBot == true)
            {
                return;
            }

            if (context.IsPrivate == true) //If they send a DM to Quantum bot
            {
                SocketGuild guild = client.GetGuild(DataClassManager.Instance.serverConfigs.serverID);
                SocketGuildUser UserRay = guild.GetUser(DataClassManager.Instance.serverConfigs.userID["Ray Soyama"]);
                MessageLog.MessageData newMessage = new MessageLog.MessageData(message);
                await UserRay.SendMessageAsync(DataClassManager.Instance.messageLog.MessageDataAsString(newMessage));
            }
            else
            {
                //Stores in RAM, less read/write
                /*
                MessageLog.MessageData newMessage = new MessageLog.MessageData(message);
                DataClassManager.Instance.messageLog.allMessageLogs.Add(newMessage);
                DataClassManager.Instance.SaveData(DataClassManager.Instance.messageLog);
                Console.WriteLine(DataClassManager.Instance.messageLog.MessageDataAsString(newMessage));
                */


                //Less RAM Usage
                //Read Data from File, Save, Delete Cache?
                DataClassManager.Instance.messageLog = DataClassManager.Instance.LoadData(DataClassManager.Instance.messageLog);

                MessageLog.MessageData newMessage = new MessageLog.MessageData(message);
                DataClassManager.Instance.messageLog.allMessageLogs.Add(newMessage);

                DataClassManager.Instance.SaveData(DataClassManager.Instance.messageLog);

                Console.WriteLine(DataClassManager.Instance.messageLog.MessageDataAsString(newMessage));

                DataClassManager.Instance.messageLog = new MessageLog();
            }


            //Commands
            int argPos = 0;

            //if valid command prompt
            if (userMessage.HasCharPrefix(DataClassManager.Instance.serverConfigs.prefix, ref argPos) || userMessage.HasMentionPrefix(client.CurrentUser, ref argPos)) //Checks for Prefix or @Quantum Bot
            {
                var result = await commands.ExecuteAsync(context, argPos, services);

                //if bot command is good
                if (result.IsSuccess == true)
                {
                    //TODO: Add logging when command gets called. Add admin only filter
                    await AdminCommandLogic.OnCommandInvoked(context);
                }
                else if (result.IsSuccess == false) //If the command failed, run this
                {

                }
            }

        }
        private async Task OnSlashCommandCalled(SocketSlashCommand command)
        {
            SlashCommands slashCommands = DataClassManager.Instance.slashCommands;

            foreach (SlashCommands.SlashCommandData cmdData in slashCommands.allSlashCommands)
            {
                if (cmdData.slashCommandBuilder.Name == command.Data.Name)
                {
                    //reflect and call function
                    try
                    {
                        Type logicClass = SlashCommandLogic.Instance.GetType();
                        MethodInfo logic = logicClass.GetMethod(cmdData.commandMethodName);

                        object[] parameters = new object[] { command };
                        logic.Invoke(SlashCommandLogic.Instance, parameters);

                        await SlashCommandLogic.Instance.OnSlashCommandInvoked(command, false);
                    }
                    catch (Exception)
                    {
                        //TODO: log errors
                    }

                    return;
                }
            }

            //no logic
            await SlashCommandLogic.Instance.OnSlashCommandInvoked(command, true);
            await command.RespondAsync("No Logic For Slash Command Found", ephemeral: true);
        }
        private async Task OnUserJoined(SocketGuildUser guildUser)
        {
            var embedBuiler = new EmbedBuilder()
                .WithThumbnailUrl(guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithAuthor(guildUser.ToString())
                .WithTitle("New User Joined!")
                .WithDescription($"<@{guildUser.Id}>\n" + $"ID: {guildUser.Id}\n" + "Intro message sent")
                .WithCurrentTimestamp();

            await guildUser.Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["I Am Logs"]).SendMessageAsync(embed: embedBuiler.Build());
            DataClassManager.Instance.userProfile.GetUserData(guildUser);

            await SendIntroductionMessage(guildUser);
        }
        private async Task OnUserLeft(SocketGuild guild, SocketUser user)
        {
            var embedBuiler = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithAuthor(user.ToString())
                .WithTitle("User Left")
                .WithDescription($"ID: {user.Id}")
                .WithCurrentTimestamp();

            // Now, Let's respond with the embed.
            await guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["I Am Logs"]).SendMessageAsync(embed: embedBuiler.Build());
        }
        private async Task OnButtonClicked(SocketMessageComponent component)
        {
            string methodName = null;
            if (DataClassManager.Instance.slashCommands.allButtonCommands.TryGetValue(component.Data.CustomId, out methodName))
            {
                //reflect and call function
                try
                {
                    Type logicClass = ButtonCommandLogic.Instance.GetType();
                    MethodInfo logic = logicClass.GetMethod(methodName);

                    object[] parameters = new object[] { component };
                    logic.Invoke(ButtonCommandLogic.Instance, parameters);

                    await ButtonCommandLogic.Instance.OnButtonCommandInvoked(component, false);
                }
                catch (Exception)
                {
                    //TODO: log errors
                    throw new NotImplementedException();
                }
            }
            else
            {
                await component.RespondAsync("No Logic For Button Press Found", ephemeral: true);
                await ButtonCommandLogic.Instance.OnButtonCommandInvoked(component, true);
            }
        }

        public static async Task SendIntroductionMessage(SocketGuildUser user)
        {
            //check if bot
            if (user.IsBot == true)
            {
                return;
            }

            string Msg = $"Welcome, {user.Mention} to Pointers Anonymous!\n" +
                            $"I am the helper bot created by <@!173226502710755328> to maintain the server~\n" +
                            $"A few things before we get you started,\n" +
                            $"";

            var builder = new EmbedBuilder()
                            .WithColor(Color.Blue)
                            .WithTitle($"Welcome {user.Nickname} to Pointers Anonymous!")
                            .WithDescription($"I am the helper bot created by <@!173226502710755328> to maintain the server~\n" +
                                            $":sparkles: A few things before we get you started:sparkles: \n\n" +
                                            $"Read the rules at <#{DataClassManager.Instance.serverConfigs.channelID["The Law"]}>")
                            .AddField("If you're a guest and would like to get access to the Game Development Channels", $"Please give us your name in <#{DataClassManager.Instance.serverConfigs.channelID["Introductions"]}>")
                            //.AddField("If you're an AIE Student", $"Type the following in <#{DataClassManager.Instance.serverConfigs.channelID["Introductions"]}>\n    Full Name:\n    Alias (Optional):\n    Graduating year:")
                            .AddField($"If you're just here to chill and play games", $"Welcome!~ You should play some <#{DataClassManager.Instance.serverConfigs.channelID["Monster Hunter"]}> with us :)");


            var embed = builder.Build();
            await user.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }
        private void ADMIN_ManuallyAddSlashCommand()
        {
            SlashCommandBuilder newSCB = new SlashCommandBuilder()
                .WithName("create-button-message")
                .WithDescription("Creates a message with buttons you can click");

            /*             SlashCommandOptionBuilder newSCOB = new SlashCommandOptionBuilder()
                                .WithName("platform-name")
                                .WithDescription("Name of the Platform")
                                .WithRequired(true)
                                .WithType(ApplicationCommandOptionType.Integer);

                        foreach (int gamePlatform in Enum.GetValues(typeof(UserProfile.UserData.GamePlatforms)))
                        {
                            newSCOB.AddChoice($"{Enum.GetName(typeof(UserProfile.UserData.GamePlatforms), gamePlatform)}", gamePlatform);
                        } 
            */

            //add options
            //newSCB.AddOption(newSCOB);
            //newSCB.AddOption("user", ApplicationCommandOptionType.User, "The user whoms't you want to see the game codes of", isRequired: true);

            SlashCommands.SlashCommandData newSCD = new SlashCommands.SlashCommandData(newSCB, "CreateButtonMessageCommand");

            //check if slashcommand with the same name exists
            List<SlashCommands.SlashCommandData> allSlashCommands = DataClassManager.Instance.slashCommands.allSlashCommands;

            foreach (SlashCommands.SlashCommandData scd in allSlashCommands) //prevent dupes? (doesn't work with overrides, so def fix later)
            {
                if (scd.slashCommandBuilder.Name == newSCD.slashCommandBuilder.Name)
                {
                    //ERROR
                    throw new NotImplementedException();
                }
            }
            DataClassManager.Instance.slashCommands.allSlashCommands.Add(newSCD);
            DataClassManager.Instance.SaveData(DataClassManager.Instance.slashCommands);
        }
    }
}
