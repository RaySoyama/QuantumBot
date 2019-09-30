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


        public static ServerConfigs serverConfigs = new ServerConfigs();

        public static List<UserProfile> UserData = new List<UserProfile>();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;


        public static int  latecy = 69;

       

        public static string logFileSavePath = "DiscordChatData.txt";
        public static string configFileSavePath = "DiscordServerConfig.json";
        public static string userFileSavePath = "DiscordUserData.json";

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        private async Task MainAsync()
        {
            //Initializes Chat Logs
            GetFilePath(logFileSavePath, ref logFileSavePath);
            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);

            //Initialize Dictionaries
            LoadServerDataFromFile();

            //User Data shit test
            LoadUserDataFromFile();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info
            });


            _client.Ready += _client_Ready;
            _client.Log += _client_Log;

            await _client.LoginAsync(TokenType.Bot, serverConfigs.TOKEN);
            await _client.StartAsync();

            //If user joins
            _client.UserJoined += AnnounceJoinedUser;

            _client.MessageReceived += _client_MessageReceived;
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
                var Ray = _client.GetUser(serverConfigs.PointersAnonUserID["Ray Soyama"]);
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

                Console.WriteLine(chatLog);
                File.AppendAllText(logFileSavePath, chatLog);
            }
                     


            if (context.Message == null || context.Message.Content.ToString() == "" || context.User.IsBot == true) //Checks if the msg is from a user, or bot
            {
                return;
            }


            int argPos = 0;

            if (!(message.HasCharPrefix(serverConfigs.prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) //Checks for Prefix or @Quantum Bot
            {
                return;
            }

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            //if bot command is good
            if (result.IsSuccess == true)
            {
                //Forward Msg to bot history
                await context.Guild.GetTextChannel(Program.serverConfigs.PointersAnonChatID["Bot History"]).SendMessageAsync($"Command Invoked:\n" +
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
            await _client.SetGameAsync($"{serverConfigs.prefix}Help");
        }


        //SERVER SIDE ACTIONS
        
        //Called when new member joins server
        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {

            await user.SendMessageAsync($"Welcome {user.Mention} to Pointers Anonomous! The unoffical AIE discord server!\n" +
                                        $"I am the helper bot created by <@!173226502710755328> to maintain the server\n" +
                                        $"To gain access to all of the servers channels, read the rules at <#{serverConfigs.PointersAnonChatID["The Law"]}>\n" +
                                        $"and introduce yourself at <#{serverConfigs.PointersAnonChatID["Introductions"]}>, and tell us your\n" +
                                        $"      Full Name:\n" +
                                        $"      Graduating Year:\n" +
                                        $"      Enrolled Course:\n\n" +
                                        $"If you have any questions, feel free to DM one of the Admins\n\n" +
                                        $"If you are not a AIE student, please tell us who you're assosiated with, so we can get a role set up for you~"
                                        );
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
            serverConfigs = JsonConvert.DeserializeObject<ServerConfigs>(contents);
            return;
        }

        public static void SaveServerDataToFile()
        {
            string contents = JsonConvert.SerializeObject(serverConfigs, Formatting.Indented);
            File.WriteAllText(configFileSavePath, contents);
            
            return;
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
