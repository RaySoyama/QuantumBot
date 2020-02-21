using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

//dotnet publish -c Build{value} -r win10-x64

namespace DiscordBot
{
    public class Program
    {
        /*
         * LIST OF DATA
         */

        public enum WEBSITES
        {
            Creddle,
            LinkedIn,
            GitHub,
            ArtStation,
            Personal,
            Twitter,
            Instagram,
        }

        public static ServerConfigs ServerConfigData = new ServerConfigs();

        public static List<UserProfile> UserData = new List<UserProfile>();

        public static BulletinBoard BulletinBoardData = new BulletinBoard();

        public static Dictionary<string, ChannelRoles> ChannelRolesData = new Dictionary<string, ChannelRoles>();

        private static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static int latency = 69;

        public static string logFileSavePath = "DiscordChatData.txt";

        public static string configFileSavePath = "DiscordServerConfig.json";
        public static string userFileSavePath = "DiscordUserData.json";
        public static string bulletinBoardSavePath = "BulletinBoardData.json";
        public static string channelRolesSavePath = "ChannelRolesData.json";

        #region Bot Core
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            Console.WriteLine(System.IO.Directory.GetParent(System.IO.Path.GetFullPath(configFileSavePath)).ToString());

            //Initializes Logs
            GetFilePath(logFileSavePath, ref logFileSavePath);

            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);
            GetFilePath(bulletinBoardSavePath, ref bulletinBoardSavePath);
            GetFilePath(channelRolesSavePath, ref channelRolesSavePath);


            //Initialize Dictionaries
            LoadServerDataFromFile();

            //User Data shit test
            LoadUserDataFromFile();

            //BulletinBoard
            LoadBulletinBoardFromFile();

            //Load Channel Roles System
            LoadChannelRolesFromFile();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info
            });


            _client.Ready += _client_Ready;
            _client.Log += _client_Log;

            await _client.LoginAsync(TokenType.Bot, ServerConfigData.TOKEN);
            await _client.StartAsync();

            //If user joins
            _client.UserJoined += AnnounceJoinedUser;
            _client.UserLeft += AnnounceLeftUser;
            _client.MessageReceived += _client_MessageReceived;
            _client.ReactionAdded += MessageReactionAdded;
            _client.ReactionRemoved += MessageReactionRemoved;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _services = new ServiceCollection().BuildServiceProvider();

            await Task.Delay(-1);

        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            latency = _client.Latency;


            if (context.IsPrivate == true && context.User.IsBot == false) //If they send a DM to Quantum bot
            {
                var Ray = _client.GetUser(ServerConfigData.PointersAnonUserID["Ray Soyama"]);
                await Ray.SendMessageAsync($"DM to Quantum Bot\n" +
                                         $"Time: {arg.Timestamp}\n" +
                                         $"Channel: {arg.Channel}\n" +
                                         $"Discord ID: {arg.Author.Id}\n" +
                                         $"Message: {arg.ToString()}\n");
            }
            else if (context.User.IsBot == false)
            {
                string chatLog = $"\n\n" +
                                 $"Time: {arg.Timestamp}\n" +
                                 $"Channel: {arg.Channel}\n" +
                                 $"Msg ID: {arg.Id}\n" +
                                 $"User ID: {arg.Author.Id}\n" +
                                 $"Username: {((IGuildUser)arg.Author).Nickname}\n" +
                                 $"Message: {arg}\n";

                foreach (var FileLink in arg.Attachments)
                {
                    chatLog += $"File: {FileLink.Url}\n";
                }

                Console.WriteLine(chatLog);
                File.AppendAllText(logFileSavePath, chatLog);
            }


            //Music bot cleansing
            int argPos = 0;
            if(context.Channel.Id.Equals(ServerConfigData.PointersAnonChatID["Music"]))
            {   
                if(context.Message.HasCharPrefix('!', ref argPos))
                {
                    //this can cause issues, so care
                    Task.Run(() => WaitThenDeleteMessage(context.Message));
                    return;
                }
                else if(context.User.Id == ServerConfigData.PointersAnonUserID["Rythm Bot"]) 
                {
                    Task.Run(() => WaitThenDeleteMessage(context.Message));
                    return;
                }
            }



            if (context.Message == null || context.Message.Content.ToString() == "" || context.User.IsBot == true) //Checks if the msg is from a user, or bot
            {
                return;
            }


            #region Custom Word Checks
            if (context.Message.ToString().ToLower().Contains("good bot"))
            {
                await context.Channel.SendMessageAsync("Thank you! You're a good human <a:partyparrot:647210646177447936>");
            }
            else if (context.Message.ToString().ToLower().Contains("happy birthday"))
            {
                var msg = await context.Channel.SendMessageAsync("<a:rave:647212416618332161>~Happy Birthday!~<a:rave:647212416618332161>");
                //var emoji = new Emoji("\uD83D\uDC4C");
                //await msg.AddReactionAsync(emoji);
            }

            #endregion



            if (!(message.HasCharPrefix(ServerConfigData.prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) //Checks for Prefix or @Quantum Bot
            {
                return;
            }

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            //if bot command is good
            if (result.IsSuccess == true)
            {

                //If Admin, don't ping
                var AuthRole = context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Admin"]);

                SocketGuildUser user = context.User as SocketGuildUser;
                if (user.Roles.Contains(AuthRole))
                {
                    //Forward Msg to bot history
                    await context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"Command Invoked:\n" +
                                                                                                                                             $"Message - \"{context.Message.ToString()}\"\n" +
                                                                                                                                             $"User - Admin: {user.Nickname} \n" +
                                                                                                                                             $"Channel - <#{context.Channel.Id}>\n" +
                                                                                                                                             $"Time - {DateTime.Now}");
                }
                else
                {
                    //Forward Msg to bot history
                    await context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"Command Invoked:\n" +
                                                                                                                                             $"Message - \"{context.Message.ToString()}\"\n" +
                                                                                                                                             $"User - <@{context.Message.Author.Id}>\n" +
                                                                                                                                             $"Channel - <#{context.Channel.Id}>\n" +
                                                                                                                                             $"Time - {DateTime.Now}");
                }
            }
            else if (result.IsSuccess == false) //If the command failed, run this
            {
                //await context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"Command Invoked and Failed <@{ServerConfigData.PointersAnonUserID["Ray Soyama"]}>:\n" +
                //                                                                                                                          $"Message - \"{context.Message.ToString()}\"\n" +
                //                                                                                                                          $"User - <@{context.Message.Author.Id}>\n" +
                //                                                                                                                          $"Channel - <#{context.Channel.Id}>\n" +
                //                                                                                                                          $"Time - {DateTime.Now}");
            }
        }

        private Task _client_Log(LogMessage arg)
        {

            Console.WriteLine($"{DateTime.Now} {arg.Message}");

            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    File.AppendAllText(logFileSavePath, $"\n{DateTime.Now} {arg.Message}");
                    break;
                case LogSeverity.Debug:
                    File.AppendAllText(logFileSavePath, $"\n{DateTime.Now} {arg.Message}");
                    break;
                case LogSeverity.Error:
                    File.AppendAllText(logFileSavePath, $"\n{DateTime.Now} {arg.Message}");
                    break;
                case LogSeverity.Info:
                    break;
                case LogSeverity.Verbose:
                    break;
                case LogSeverity.Warning:
                    File.AppendAllText(logFileSavePath, $"\n{DateTime.Now} {arg.Message}");
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task _client_Ready()
        {
            await _client.SetGameAsync($"{ServerConfigData.prefix}Help");
        }

        #endregion

        #region Server events

        //Called when new member joins server
        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {
            await user.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["I Am Logs"]).SendMessageAsync($"User <@!{user.Id}> has joined the server. Intro msg has been sent");
            await SendIntroductionMessage(user);
        }

        public async Task AnnounceLeftUser(SocketGuildUser user) //Announces the left user
        {
            await user.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["I Am Logs"]).SendMessageAsync($"User <@!{user.Id}> has left the Server");
        }

        private async Task WaitThenDeleteMessage(IUserMessage msg)
        {
            await Task.Delay(10000);
            await msg.DeleteAsync();
        }
  
        //General Use Introduction
        public static async Task SendIntroductionMessage(SocketGuildUser user)
        {
            //check if bot
            if(user.IsBot == true)
            {
                return;
            }

            string Msg = $"Welcome, {user.Mention} to Pointers Anonymous, the unofficial AIE Discord server!\n" +
                            $"I am the helper bot created by <@!173226502710755328> to maintain the server~\n" +
                            $"A few things before we get you started,\n" +
                            $"";

            var builder = new EmbedBuilder()
                            .WithColor(Color.Blue)
                            .WithTitle($"Welcome {user.Nickname} to Pointers Anonymous, the unofficial AIE Discord server!")
                            .WithDescription($"I am the helper bot created by <@!173226502710755328> to maintain the server~\n" +
                                            $":sparkles: A few things before we get you started:sparkles: \n\n" +
                                            $"Read the rules at <#{ServerConfigData.PointersAnonChatID["The Law"]}>")
                            .AddField("If you're a guest and would like to get access to the Game Development Channels", $"Please give us your name in <#{ServerConfigData.PointersAnonChatID["Introductions"]}>")
                            .AddField("If you're an AIE Student", $"Type the following in <#{ServerConfigData.PointersAnonChatID["Introductions"]}>\n    Full Name:\n    Alias (Optional):\n    Graduating year:")
                            .AddField($"If you're just here to chill and play games", $"Welcome!~ You should play some <#{Program.ServerConfigData.PointersAnonChatID["Monster Hunter"]}> with us :)");


            var embed = builder.Build();
            await user.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }

        public static async Task MessageReactionAdded(Cacheable<IUserMessage, ulong> OldMsg, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            await OnReactionAdded(OldMsg.Id, NewMsg, react);
        }

        public static async Task MessageReactionRemoved(Cacheable<IUserMessage, ulong> OldMsg, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            await OnReactionRemoved(OldMsg.Id, NewMsg, react);
        }
        
        #endregion

        #region Bot File Parcing
        public static void UpdateAllDataFromFiles()
        {

            //Initialize Dictionaries
            LoadServerDataFromFile();

            //User Data shit test
            LoadUserDataFromFile();

            //BulletinBoard
            LoadBulletinBoardFromFile();

            //Load Channel Roles System
            LoadChannelRolesFromFile();
        }

        //User Data Handling
        private static void LoadUserDataFromFile()
        {
            string contents = File.ReadAllText(userFileSavePath);
            UserData = JsonConvert.DeserializeObject<List<UserProfile>>(contents);

            if (UserData == null)
            {
                UserData = new List<UserProfile>();
            }

            return;
        }

        public static void SaveUserDataToFile()
        {
            string contents = JsonConvert.SerializeObject(UserData, Formatting.Indented);
            File.WriteAllText(userFileSavePath, contents);
            return;
        }


        //Server Data Handling
        private static void LoadServerDataFromFile()
        {
            string contents = File.ReadAllText(configFileSavePath);
            ServerConfigData = JsonConvert.DeserializeObject<ServerConfigs>(contents);
            return;
        }

        public static void SaveServerDataToFile()
        {
            string contents = JsonConvert.SerializeObject(ServerConfigData, Formatting.Indented);
            File.WriteAllText(configFileSavePath, contents);

            return;
        }


        //Bulletin Board Data Handling

        private static void LoadBulletinBoardFromFile()
        {
            string contents = File.ReadAllText(bulletinBoardSavePath);
            BulletinBoardData = JsonConvert.DeserializeObject<BulletinBoard>(contents);

            if (BulletinBoardData.Lunchboxes == null)
            {
                BulletinBoardData.Lunchboxes = new List<Lunchbox>();
            }
            return;
        }

        public static void SaveBulletinBoardDataToFile()
        {
            string contents = JsonConvert.SerializeObject(BulletinBoardData, Formatting.Indented);
            File.WriteAllText(bulletinBoardSavePath, contents);

            return;
        }

        private static async Task OnReactionAdded(ulong MessageID, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            //Assign Roles based off of reaction to the emote
            if (MessageID == ServerConfigData.ServerRoleSetUpMsgID && react.UserId != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"])
            {
                foreach (KeyValuePair<string, ChannelRoles> EmoteData in Program.ChannelRolesData)
                {
                    if (react.Emote.Equals(EmoteData.Value.ChannelReactEmote))
                    {
                        //Checks if certified student
                        var AllRoles = (react.User.Value as SocketGuildUser).Roles;
                        var Guilds = (NewMsg as SocketGuildChannel).Guild;
                        if (AllRoles.Contains(Guilds.GetRole(ServerConfigData.PointersAnonRoleID["Student"])) || AllRoles.Contains(Guilds.GetRole(ServerConfigData.PointersAnonRoleID["Guest"])) || AllRoles.Contains(Guilds.GetRole(ServerConfigData.PointersAnonRoleID["Teacher"])))
                        {
                            //If you get a null ref exception. It's because the role ID is bad
                            await (react.User.Value as SocketGuildUser).AddRoleAsync(Guilds.GetRole(EmoteData.Value.RoleID));

                            await (NewMsg as SocketGuildChannel).Guild.GetTextChannel(ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"User <@{react.UserId}> gained role {EmoteData.Key}");
                            return;
                        }
                        else
                        {
                            await (await NewMsg.GetMessageAsync(MessageID) as IUserMessage).RemoveReactionAsync(react.Emote, react.User.Value);
                            await (NewMsg as SocketGuildChannel).Guild.GetTextChannel(ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"User <@{react.UserId}> tried to gained role {EmoteData.Key}, but does not have the proper perms");
                        }
                    }
                }

                return;
            }


            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (MessageID == bulletinEvent.MsgID && react.UserId != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"] && react.Emote.Equals(BulletinBoardData.BulletinAttendingEmote))
                {
                    if (bulletinEvent.AttendingUsers.Contains(react.UserId) == false)
                    {
                        bulletinEvent.AttendingUsers.Add(react.UserId);
                        await (NewMsg as SocketGuildChannel).Guild.GetTextChannel(ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"User <@{react.UserId}> is going to the Event \"{bulletinEvent.Title}\"");
                    }

                    var builder = new EmbedBuilder()
                        .WithTitle(bulletinEvent.Title)
                        .WithUrl($"{bulletinEvent.EventURL}")
                        .WithColor(new Color(0, 0, 255))
                        .WithDescription($"{bulletinEvent.Description}")
                        .WithThumbnailUrl($"{bulletinEvent.IconURL}")
                        .AddField($"Time", bulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                        .AddField($"Location", $"{bulletinEvent.Location}", true)
                        .AddField($"Cost", $"{bulletinEvent.Cost}", true)
                        .AddField($"Capacity", $"{bulletinEvent.Capacity}", true)
                        .AddField($"Attending", $"{bulletinEvent.AttendingUsers.Count}", true)
                        .WithFooter($"By {(await NewMsg.GetUserAsync(bulletinEvent.author) as SocketGuildUser).Nickname}", $"{bulletinEvent.authorIconURL}")
                        .WithTimestamp(bulletinEvent.embedCreated);

                    var embed = builder.Build();

                    var msg = await NewMsg.GetMessageAsync(MessageID) as IUserMessage;

                    await msg.ModifyAsync(x => x.Embed = embed);

                    //add atending emote
                    SaveBulletinBoardDataToFile();
                    await msg.AddReactionAsync(BulletinBoardData.BulletinAttendingEmote);

                    return;
                }
            }
        }
        private static async Task OnReactionRemoved(ulong MessageID, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            if (MessageID == ServerConfigData.ServerRoleSetUpMsgID && react.UserId != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"])
            {
                foreach (KeyValuePair<string, ChannelRoles> EmoteData in Program.ChannelRolesData)
                {
                    if (react.Emote.Equals(EmoteData.Value.ChannelReactEmote))
                    {
                        await (react.User.Value as SocketGuildUser).RemoveRoleAsync((NewMsg as SocketGuildChannel).Guild.GetRole(EmoteData.Value.RoleID));

                        //NO LOGIC CHECK FOR WHO GETS WHAT ROLES
                        await (NewMsg as SocketGuildChannel).Guild.GetTextChannel(ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"User <@{react.UserId}> removed role {EmoteData.Key}");
                        return;
                    }
                }

                return;
            }


            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (MessageID == bulletinEvent.MsgID && react.UserId != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"] && react.Emote.Equals(BulletinBoardData.BulletinAttendingEmote))
                {
                    if (bulletinEvent.AttendingUsers.Contains(react.UserId) == true)
                    {
                        bulletinEvent.AttendingUsers.Remove(react.UserId);
                        await (NewMsg as SocketGuildChannel).Guild.GetTextChannel(ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"User <@{react.UserId}> is no longer going to the Event \"{bulletinEvent.Title}\"");
                    }

                    var builder = new EmbedBuilder()
                        .WithTitle(bulletinEvent.Title)
                        .WithUrl($"{bulletinEvent.EventURL}")
                        .WithColor(new Color(0, 0, 255))
                        .WithDescription($"{bulletinEvent.Description}")
                        .WithThumbnailUrl($"{bulletinEvent.IconURL}")
                        .AddField($"Time", bulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                        .AddField($"Location", $"{bulletinEvent.Location}", true)
                        .AddField($"Cost", $"{bulletinEvent.Cost}", true)
                        .AddField($"Capacity", $"{bulletinEvent.Capacity}", true)
                        .AddField($"Attending", $"{bulletinEvent.AttendingUsers.Count}", true)
                        .WithFooter($"By {(await NewMsg.GetUserAsync(bulletinEvent.author) as SocketGuildUser).Nickname}", $"{bulletinEvent.authorIconURL}")
                        .WithTimestamp(bulletinEvent.embedCreated);

                    var embed = builder.Build();

                    var msg = await NewMsg.GetMessageAsync(MessageID) as IUserMessage;

                    await msg.ModifyAsync(x => x.Embed = embed);

                    //add atending emote
                    SaveBulletinBoardDataToFile();
                    await msg.AddReactionAsync(BulletinBoardData.BulletinAttendingEmote);

                    return;
                }
            }
        }


        //ChannelRoles Data Handleing
        private static void LoadChannelRolesFromFile()
        {
            string contents = File.ReadAllText(channelRolesSavePath);
            ChannelRolesData = JsonConvert.DeserializeObject<Dictionary<string, ChannelRoles>>(contents);

            foreach (KeyValuePair<string, ChannelRoles> data in Program.ChannelRolesData)
            {
                data.Value.ChannelReactEmote = Discord.Emote.Parse($"{data.Value.ChannelReactEmoteID}");
            }

            return;
        }

        public static void SaveChannelRolesToFile()
        {
            string contents = JsonConvert.SerializeObject(ChannelRolesData, Formatting.Indented);
            File.WriteAllText(channelRolesSavePath, contents);

            return;
        }

        //Gets the save files
        private static void GetFilePath(string textFileName, ref string path)
        {
            path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(textFileName)).ToString();

            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();

            Directory.CreateDirectory(path + "\\PointersAnonymousBotFiles\\");

            path += "\\PointersAnonymousBotFiles\\" + textFileName;

            //if no file exist, create
            if (File.Exists(path) == false)
            {
                var myFile = File.Create(path);
                myFile.Close();
            }
        }
        #endregion
    }
}