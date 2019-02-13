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

namespace DiscordBot
{
    public class Program
    {
        private const string TOKEN = "NTQwNjc3MDgwMjk2MTk0MDc5.DzUbVQ.EBSdDBSLjVN_L3Ho_aES9MNG-Fo";
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public static char prefix = '&';

        public static string logFileSavePath = "DiscordChatData.txt";
        public static string configFileSavePath = "DiscordChatConfig.txt";
        public static string userFileSavePath = "DiscordUserData.txt";

        public static List<Human> ListOfHumans = new List<Human>();

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();


        private async Task MainAsync()
        {

            GetFilePath(logFileSavePath, ref logFileSavePath);
            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);

            //Read User Data
            GetUserDataFromFile(userFileSavePath, ref ListOfHumans);

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });


            _client.Ready += _client_Ready;
            _client.Log += _client_Log;

            await _client.LoginAsync(TokenType.Bot, TOKEN);
            await _client.StartAsync();


            _client.MessageReceived += _client_MessageReceived;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _services = new ServiceCollection().BuildServiceProvider();

            await Task.Delay(-1);

        }

        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            string chatLog = $"\n\n" +
                             $"Time: {arg.Timestamp}\n" +
                             $"Channel: {arg.Channel}\n" +
                             $"Username: {arg.Author}\n" +
                             $"Message: {arg}\n";

            Console.WriteLine(chatLog);
            File.AppendAllText(logFileSavePath, chatLog);

            if (context.Message == null || context.Message.Content == "" || context.User.IsBot == true) //Checks if the msg is from a user, or bot
            {
                return;
            }

            int argPos = 0;

            if (!(message.HasCharPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) //Checks for Prefix or @Quantum Bot
            {
                return;
            }

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.IsSuccess == false) //If the command failed, run this
            {
                var builder = new EmbedBuilder()
                          .WithTitle("Command Not Found, or is broken")
                          .WithDescription($"\nMessage: {message}\n")
                          .WithColor(new Color(255, 0, 0))
                          .WithTimestamp(DateTimeOffset.Now)
                          .WithAuthor(author =>
                          {
                              author
                              .WithName("Summoned by " + message.Author.ToString())
                              .WithIconUrl(message.Author.GetAvatarUrl());
                          });
                var embed = builder.Build();
                await message.Channel.SendMessageAsync("", embed: embed);
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
            await _client.SetGameAsync($"{prefix}Help");
        }

        public static void GetFilePath(string textFileName, ref string path)
        {
            path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(textFileName)).ToString();

            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();
            //path = System.IO.Directory.GetParent(path).ToString();

            Directory.CreateDirectory(path + "\\DiscordBotFiles\\");

            path += "\\DiscordBotFiles\\" + textFileName;
        }

        public static void GetUserDataFromFile(string path, ref List<Human> _ListOfHumans)
        {
            _ListOfHumans = new List<Human>();
            Human tempHuman = new Human();

            if (System.IO.File.Exists(path) == false) //If File Doesn't Exsist
            {
                System.IO.File.CreateText(path).Close();
                _ListOfHumans = new List<Human>();
            }

            string[] userSaveFileData = System.IO.File.ReadAllLines(path);


            for (int i = 0; i < userSaveFileData.Length; i++)
            {
                if (userSaveFileData[i].Equals("<NAME>"))
                {
                    tempHuman = new Human();
                    i++;
                    tempHuman.dicordUserName = userSaveFileData[i];
                    i++;
                    tempHuman.discordID = userSaveFileData[i];
                }
                else if (userSaveFileData[i].Equals("<LINK>"))
                {
                    i++;
                    tempHuman.HumanSiteData.Add(userSaveFileData[i], userSaveFileData[i + 1]);
                }
                else if (userSaveFileData[i].Equals("<END>"))
                {
                    _ListOfHumans.Add(tempHuman);
                }
            }
        }

        public static int UpdateUserDataList(string userID, string userName, string key, string URL)
        {
            //Seperate Adding to Method, from writting to file

            for (int i = 0; i < ListOfHumans.Count; i++)
            {
                if (ListOfHumans[i].discordID == userID)
                {
                    ListOfHumans[i].dicordUserName = userName;

                    string throwAwayString;

                    if (ListOfHumans[i].HumanSiteData.TryGetValue(key, out throwAwayString) == true) // Chekcks if the link already exsist, not case sensitive
                    {
                        return 1;
                    }
                    else
                    {
                        ListOfHumans[i].HumanSiteData.Add(key, URL);
                        UpdateUserDataFile();
                        return 0;
                    }
                }
            }

            //Adding new Human
            Human newHuman = new Human
            {
                discordID = userID,
                dicordUserName = userName
            };
            newHuman.HumanSiteData.Add(key, URL);

            ListOfHumans.Add(newHuman);
            UpdateUserDataFile();
            return 0;
        }

        public static void UpdateUserDataFile()
        {
            StreamWriter streamWriter = File.CreateText(userFileSavePath);

            for (int i = 0; i < ListOfHumans.Count; i++)
            {
                streamWriter.WriteLine("<NAME>");
                streamWriter.WriteLine(ListOfHumans[i].dicordUserName);
                streamWriter.WriteLine(ListOfHumans[i].discordID);

                foreach (KeyValuePair<string, string> entry in ListOfHumans[i].HumanSiteData)
                {
                    streamWriter.WriteLine("<LINK>");
                    streamWriter.WriteLine(entry.Key);
                    streamWriter.WriteLine(entry.Value);
                }
                streamWriter.WriteLine("<END>");
            }
            streamWriter.Close();
        }

    }
}
