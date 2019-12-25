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


        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
         

        public static int  latecy = 69;

       

        public static string logFileSavePath = "DiscordChatData.txt";


        public static string configFileSavePath = "DiscordServerConfig.json";
        public static string userFileSavePath = "DiscordUserData.json";
        public static string bulletinBoardSavePath = "BulletinBoardData.json";



        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            //Initializes Logs
            GetFilePath(logFileSavePath, ref logFileSavePath);

            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);
            GetFilePath(bulletinBoardSavePath, ref bulletinBoardSavePath);

            
            //Initialize Dictionaries
            LoadServerDataFromFile();

            //User Data shit test
            LoadUserDataFromFile();

            //BulletinBoard
            LoadBulletinBoardFromFile();

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
            _client.UserLeft += AnnouceLeftUser;
            _client.MessageReceived += _client_MessageReceived;
            _client.ReactionAdded += MessageReactionAdded;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _services = new ServiceCollection().BuildServiceProvider();

            await Task.Delay(-1);

        }
        
        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            latecy = _client.Latency;

                    
            if (context.IsPrivate == true && context.User.IsBot == false) //If they send a DM to Quantum bot
            {
                var Ray = _client.GetUser(ServerConfigData.PointersAnonUserID["Ray Soyama"]);
                await Ray.SendMessageAsync($"DM to Quantum Bot\n" +
                                         $"Time: {arg.Timestamp}\n" +
                                         $"Channel: {arg.Channel}\n" +
                                         $"Discord ID: {arg.Author.Id}\n" +
                                         $"Message: {arg.ToString()}\n");
            }
            else if(context.User.IsBot == false)
            {

                string chatLog = $"\n\n" +
                                 $"Time: {arg.Timestamp}\n" +
                                 $"Channel: {arg.Channel}\n" +
                                 $"Discord ID: {arg.Author.Id}\n" +
                                 $"Username: {((IGuildUser)arg.Author).Nickname}\n" +
                                 $"Message: {arg}\n";

                foreach (var FileLink in arg.Attachments)
                { 
                    chatLog += $"File: {FileLink.Url}\n";
                }
                                
                Console.WriteLine(chatLog);
                File.AppendAllText(logFileSavePath, chatLog);
            }
                     


            if (context.Message == null || context.Message.Content.ToString() == "" || context.User.IsBot == true) //Checks if the msg is from a user, or bot
            {
                return;
            }


            //custom inline msg replies
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




            int argPos = 0;

            if (!(message.HasCharPrefix(ServerConfigData.prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) //Checks for Prefix or @Quantum Bot
            {
                return;
            }

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            //if bot command is good
            if (result.IsSuccess == true)
            {
                //Forward Msg to bot history
                await context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync($"Command Invoked:\n" +
                                                                                                                                         $"Message - \"{context.Message.ToString()}\"\n" +
                                                                                                                                         $"User - <@{context.Message.Author.Id}>\n" +
                                                                                                                                         $"Channel - <#{context.Channel.Id}>\n" +
                                                                                                                                         $"Time - {DateTime.Now}");
            }
            else if (result.IsSuccess == false) //If the command failed, run this
            {
                return;
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


        //SERVER SIDE ACTIONS
        
        //Called when new member joins server
        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {
            await user.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["I Am Logs"]).SendMessageAsync($"User <@!{user.Id}> has joined the server. Intro msg has been sent");
            await SendIntroductionMessage(user);
        }
        
        public async Task AnnouceLeftUser(SocketGuildUser user) //Annouces the left user
        {
            await user.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["I Am Logs"]).SendMessageAsync($"User <@!{user.Id}> has left the Server");
        }

        //General Use Introduction
        public static async Task SendIntroductionMessage(SocketGuildUser user)
        {
            await user.SendMessageAsync($"Welcome {user.Mention} to Pointers Anonymous, the unofficial AIE Discord server!\n" +
                                $"I am the helper bot created by <@!173226502710755328> to maintain the server\n\n" +
                                $"Read the rules at <#{ServerConfigData.PointersAnonChatID["The Law"]}> and to gain access to some of the server's channels, \n" +
                                $"Introduce yourself at <#{ServerConfigData.PointersAnonChatID["Introductions"]}>! (It's okay if you don't)\n" +
                                $"      Prefered Name:\n" +
                                $"      Occupation:\n" +
                                $"      Favorite food:\n\n" +
                                $"If you are a AIE Student, please state your\n" +
                                $"      Full Name:\n" +
                                $"      Alias (Optional):\n" +
                                $"      Graduating Year:\n" +
                                $"      Enrolled Course:\n\n" +
                                $"If you have any questions, feel free to DM one of the Admins"
                                );
        }

        public static async Task MessageReactionAdded(Cacheable<IUserMessage, ulong> OldMsg, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            await SoftUpdate(OldMsg.Id,NewMsg,react);
        }




        //User Data Handling
        public void LoadUserDataFromFile()
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
        public void LoadServerDataFromFile()
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

        public void LoadBulletinBoardFromFile()
        {
            string contents = File.ReadAllText(bulletinBoardSavePath);
            BulletinBoardData = JsonConvert.DeserializeObject<BulletinBoard>(contents);

            if(BulletinBoardData.Lunchboxes == null)
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

        private static async Task SoftUpdate(ulong MessageID, ISocketMessageChannel NewMsg, SocketReaction react)
        {
            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (MessageID == bulletinEvent.MsgID && react.UserId != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"] && react.Emote.Equals(BulletinBoardData.BulletinAttendingEmote))
                {
                    if (bulletinEvent.AttendingUsers.Contains(react.UserId) == false)
                    {
                        bulletinEvent.AttendingUsers.Add(react.UserId);
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
                    await msg.RemoveAllReactionsAsync();
                    await msg.AddReactionAsync(BulletinBoardData.BulletinAttendingEmote);
                }
            }
        }



        //Gets the save files
        public static void GetFilePath(string textFileName, ref string path)
        {
            path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(textFileName)).ToString();

            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();

            Directory.CreateDirectory(path + "\\Poiners Anonymous Bot Files\\");

            path += "\\Poiners Anonymous Bot Files\\" + textFileName;

            //if no file exsist, create
            if (File.Exists(path) == false)
            {
                var myfile =  File.Create(path);
                myfile.Close();
            }
        }      
    }
}
