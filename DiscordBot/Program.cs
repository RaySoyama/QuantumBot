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
    
        //List of Chat References
        public static Dictionary<string, ulong> PointersAnonChatID = new Dictionary<string, ulong>();
        //List of Role References
        public static Dictionary<string, ulong> PointersAnonRoleID = new Dictionary<string, ulong>();
        //List of User References
        public static Dictionary<string, ulong> PointersAnonUserID = new Dictionary<string, ulong>();

        public static Dictionary<WEBSITES, WebsiteProfile> WebsiteData = new Dictionary<WEBSITES, WebsiteProfile>();



        public static List<UserProfile> UserData = new List<UserProfile>();

        private const string TOKEN = "NTQwNjc3MDgwMjk2MTk0MDc5.DzUbVQ.EBSdDBSLjVN_L3Ho_aES9MNG-Fo";
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;


        public static char prefix = '>';
        public static int  latecy = 69;


        public static string logFileSavePath = "DiscordChatData.txt";
        public static string configFileSavePath = "DiscordChatConfig.txt";
        public static string userFileSavePath = "DiscordUserData.json";

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();


        private async Task MainAsync()
        {
            //Initializes Chat Logs
            GetFilePath(logFileSavePath, ref logFileSavePath);
            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);

            //Initialize Dictionaries
            InitializeChatID();
            InitializeRoleID();
            InitializeUserID();

            InitializeWebsiteData();

            //User Data shit test
            LoadUserDataFromFile();

            //UserProfile Ray = new UserProfile
            //{
            //    userID = 173226502710755328,
            //    userNickname = "Ray S",
            //    GradYear = 2020,
            //    isStudent = true,
            //    isTeacher = false,
            //    isGuest = false,
            //    UserWebsiteIndex = new Dictionary<WEBSITES, string>
            //    {
            //        {WEBSITES.Creddle, "https://resume.creddle.io/resume/ijwq0koycmm"},
            //        {WEBSITES.LinkedIn, "https://www.linkedin.com/in/raysoyama/"},
            //        {WEBSITES.GitHub, "https://github.com/RaySoyama"},
            //        {WEBSITES.ArtStation, null},
            //        {WEBSITES.Personal, null},
            //        {WEBSITES.Instagram, "https://www.instagram.com/raysoyama/"},
            //        {WEBSITES.Twitter, "https://twitter.com/RaySoyama"}
            //    },
            //};
            //UserData.Clear();
            //UserData.Add(Ray);            
            
            //SaveUserDataToFile();


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

            await _client.LoginAsync(TokenType.Bot, TOKEN);
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
                var Ray = _client.GetUser(PointersAnonUserID["Ray Soyama"]);
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

            if (!(message.HasCharPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) //Checks for Prefix or @Quantum Bot
            {
                return;
            }

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.IsSuccess == false) //If the command failed, run this
            {
                return;
                /*
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
                */
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


        //SERVER SIDE ACTIONS
        
        //Called when new member joins server
        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {

            await user.SendMessageAsync($"Welcome {user.Mention} to Pointers Anonomous! The unoffical AIE discord server!\n" +
                                        $"I am the helper bot created by <@!173226502710755328> to maintain the server\n" +
                                        $"To gain access to all of the servers channels, read the rules at <#{PointersAnonChatID["The Law"]}>\n" +
                                        $"and introduce yourself at <#{PointersAnonChatID["Introductions"]}>, and tell us your\n" +
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



        //LOCAL ACTIONS
        //Create References
        private void InitializeChatID()
        {
            PointersAnonChatID.Add("Introductions", 487667585295319040);
            PointersAnonChatID.Add("The Law", 487666653690200064);
            PointersAnonChatID.Add("Town Hall", 566017637721571342);
            PointersAnonChatID.Add("Bot Commands", 489949750762668035);
            PointersAnonChatID.Add("Personal Links", 487883949033652246);
        }

        private void InitializeRoleID()
        {
            PointersAnonRoleID.Add("Admin", 487403594300129291);
            PointersAnonRoleID.Add("Certified", 487403476373078028);
            PointersAnonRoleID.Add("Guest", 553001579146379281);
            PointersAnonRoleID.Add("Teacher", 507411381461712896);
            PointersAnonRoleID.Add("Class Of 2020", 603018181262704640);
            PointersAnonRoleID.Add("Class Of 2021", 603018331775303720);
        }

        private void InitializeUserID()
        {
            PointersAnonUserID.Add("Ray Soyama", 173226502710755328);
            PointersAnonUserID.Add("Terry Nguyen", 99563003434782720);
        }


        private void InitializeWebsiteData()
        {
            WebsiteData.Add(WEBSITES.Creddle, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.Creddle,
                WebsiteChatID = 626664819210059805,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626204428260737057/sUvz1kky_400x400.png",
                WebsiteColor = new Color(39, 130, 130)
            });
            
            WebsiteData.Add(WEBSITES.LinkedIn, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.LinkedIn,
                WebsiteChatID = 626664824180310026,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626311018498228235/LI-In-Bug.png",
                WebsiteColor = new Color(0, 119, 181)
            });
            
            WebsiteData.Add(WEBSITES.GitHub, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.GitHub,
                WebsiteChatID = 626664829020667905,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626311584175620109/GitHub-Mark-120px-plus.png",
                WebsiteColor = new Color(27, 29, 35)
            });
            
            WebsiteData.Add(WEBSITES.ArtStation, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.ArtStation,
                WebsiteChatID = 626664834360147969,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626313302397419530/logo-artstation-plain.png",
                WebsiteColor = new Color(19, 175, 240)
            });

            WebsiteData.Add(WEBSITES.Personal, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.Personal,
                WebsiteChatID = 626664839338786816,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626313819408433170/map_023-globe-location-earth-website-512.png",
                WebsiteColor = new Color(0, 0, 0)
            });

            WebsiteData.Add(WEBSITES.Twitter, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.Twitter,
                WebsiteChatID = 626664845047103489,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626314390513254400/Twitter_Social_Icon_Square_Color.png",
                WebsiteColor = new Color(29, 161, 242)
            });
            
            WebsiteData.Add(WEBSITES.Instagram, new WebsiteProfile
            {
                WebsiteEnum = WEBSITES.Instagram,
                WebsiteChatID = 626664849744855080,
                WebsiteIconURL = "https://cdn.discordapp.com/attachments/489949750762668035/626314091430150154/Instagram_AppIcon_Aug2017.png",
                WebsiteColor = new Color(131, 58, 180)
            });

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

        #region depricated
        //public static void GetUserDataFromFile(string path, ref List<Human> _ListOfHumans)
        //{
        //    _ListOfHumans = new List<Human>();
        //    Human tempHuman = new Human();

        //    if (System.IO.File.Exists(path) == false) //If File Doesn't Exsist
        //    {
        //        System.IO.File.CreateText(path).Close();
        //        _ListOfHumans = new List<Human>();
        //    }

        //    string[] userSaveFileData = System.IO.File.ReadAllLines(path);


        //    for (int i = 0; i < userSaveFileData.Length; i++)
        //    {
        //        if (userSaveFileData[i].Equals("<NAME>"))
        //        {
        //            tempHuman = new Human();
        //            i++;
        //            tempHuman.dicordUserName = userSaveFileData[i];
        //            i++;
        //            tempHuman.discordID = userSaveFileData[i];
        //        }
        //        else if (userSaveFileData[i].Equals("<LINK>"))
        //        {
        //            i++;
        //            tempHuman.HumanSiteData.Add(userSaveFileData[i], userSaveFileData[i + 1]);
        //        }
        //        else if (userSaveFileData[i].Equals("<END>"))
        //        {
        //            _ListOfHumans.Add(tempHuman);
        //        }
        //    }
        //}

        //public static int UpdateUserDataList(string userID, string userName, string key, string URL)
        //{
        //    //Seperate Adding to Method, from writting to file

        //    for (int i = 0; i < ListOfHumans.Count; i++)
        //    {
        //        if (ListOfHumans[i].discordID == userID)
        //        {
        //            ListOfHumans[i].dicordUserName = userName;

        //            string throwAwayString;

        //            if (ListOfHumans[i].HumanSiteData.TryGetValue(key, out throwAwayString) == true) // Chekcks if the link already exsist, not case sensitive
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                ListOfHumans[i].HumanSiteData.Add(key, URL);
        //                UpdateUserDataFile();
        //                return 0;
        //            }
        //        }
        //    }

        //    //Adding new Human
        //    Human newHuman = new Human
        //    {
        //        discordID = userID,
        //        dicordUserName = userName
        //    };
        //    newHuman.HumanSiteData.Add(key, URL);

        //    ListOfHumans.Add(newHuman);
        //    UpdateUserDataFile();
        //    return 0;
        //}

        //public static void UpdateUserDataFile()
        //{
        //    StreamWriter streamWriter = File.CreateText(userFileSavePath);

        //    for (int i = 0; i < ListOfHumans.Count; i++)
        //    {
        //        streamWriter.WriteLine("<NAME>");
        //        streamWriter.WriteLine(ListOfHumans[i].dicordUserName);
        //        streamWriter.WriteLine(ListOfHumans[i].discordID);

        //        foreach (KeyValuePair<string, string> entry in ListOfHumans[i].HumanSiteData)
        //        {
        //            streamWriter.WriteLine("<LINK>");
        //            streamWriter.WriteLine(entry.Key);
        //            streamWriter.WriteLine(entry.Value);
        //        }
        //        streamWriter.WriteLine("<END>");
        //    }
        //    streamWriter.Close();
        //}
        #endregion
    }
}
