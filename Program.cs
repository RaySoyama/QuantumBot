using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace DiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        //My stuffs
        public string logFileSavePath = "DiscordChatData.txt";
        public string configFileSavePath = "DiscordChatConfig.txt";
        public string userFileSavePath = "DiscordUserData.txt";

        List<DiscordHuman> ListOfHuman = new List<DiscordHuman>();

        public bool chatFilterEnabled;
        //

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            //Gets File Locations
            GetFilePath(logFileSavePath, ref logFileSavePath);
            GetFilePath(configFileSavePath, ref configFileSavePath);
            GetFilePath(userFileSavePath, ref userFileSavePath);

            //Read User Data
            GetUserDataFromFile(userFileSavePath, ref ListOfHuman);

            //setup
            _client.Log += Log;
            string token = "NTQwNjc3MDgwMjk2MTk0MDc5.DzUbVQ.EBSdDBSLjVN_L3Ho_aES9MNG-Fo"; //Discord Bot Token
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceived; //Runs if a msg was received

            await Task.Delay(-1);  // Block this task until the program is closed.
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            File.AppendAllText(logFileSavePath, msg.ToString() + "\n");
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            SocketUserMessage msg = message as SocketUserMessage; // cast to a USER msg;

            if (msg == null || msg.Author.ToString() == "Quantum Bot#5354") //if msg is from bot,
            {
                return;
            }

            string chatLog = "Time: " + msg.Timestamp.ToString() + "\nChannel: " + msg.Channel.ToString() + "\nUsername: " + msg.Author.ToString() + "\nMessage: " + msg.ToString() + "\n\n";


            File.AppendAllText(logFileSavePath, chatLog);

            Console.WriteLine(chatLog);
            int argPos = 0;
            char prefix = '*';

            if (msg.HasCharPrefix(prefix, ref argPos))
            {
                string msgContent = msg.ToString().Substring(argPos);

                //QUIT Function
                if (msgContent == "Quit" && msg.Author.ToString() == "Quantum Blue#1234")
                {
                    await message.Channel.SendMessageAsync("I'll be back - Steve Jobs\nhttps://media.giphy.com/media/gFwZfXIqD0eNW/giphy.gif");
                    System.Environment.Exit(1);
                }
                else if (msgContent == "Ping")
                {
                    await message.Channel.SendMessageAsync("Pong!");
                }
                else if (msgContent == "Help" || msgContent == "Commands")
                {
                    await message.Channel.SendMessageAsync(
                        "```\n" +
                        "Currently \' " + prefix + "\' is the prefix for all commands\n" +
                        "\n" +
                        "Help or Commands - Prints what you're reading now\n" +
                        "Ping             - Pong, a good way to make sure I'm alive\n" +
                        "Profile @User    - Returns Info bout User" +
                        "ProfileAdd URL_NAME URL - Adds URL, and Key into the system" +
                        "Example:\n" +
                        "Profile @Quantum Bot\n" +
                        "ProfileAdd Github https://github.com/RaySoyama " +
                        "ProfileEdit Github GitHub https://github.com/RaySoyama" +
                        "\n" +
                        "Mod Commands\n" +
                        "Quit             - Closes Bot\n" +
                        "ToggleChatFilter - Turns Chat Filter On/Off\n" +
                        "\n" +
                        "\n" +
                        "I know the Profile Functions are a pain in the ass\n" +
                        "If you have any recomendations, or need help @Quantum Blue#1234\n" +
                        "```\n"
                        );
                }
                else if (msgContent == "ToggleChatFilter" && msg.Author.ToString() == "Quantum Blue#1234")
                {
                    if (chatFilterEnabled == true)
                    {
                        chatFilterEnabled = false;
                        await message.Channel.SendMessageAsync("Chat filter deactivated");
                    }
                    else
                    {
                        chatFilterEnabled = true;
                        await message.Channel.SendMessageAsync("Chat filter activated");
                    }
                }
                else if (msgContent.Substring(0, 7) == ("Profile"))
                {
                    //Console.WriteLine(msgContent.Substring(11, msgContent.Length - 1 - 11));

                    //await message.Channel.SendMessageAsync(msgContent);

                    if (msgContent.Length <= 10)
                    {
                        await message.Channel.SendMessageAsync("Specify Command");
                        return;
                    }


                    /*
                    ID TAG 
                    @ - Bot
                    @! - Human
                    @& - Role
                    */

                    if (msgContent.Substring(8, 3) == ("<@!"))//Checks if @ is a human
                    {
                        for (int i = 0; i < ListOfHuman.Count; i++) //goes through database, 
                        {
                            if (ListOfHuman[i].discordID == msgContent.Substring(8, msgContent.Length - 8))
                            {
                                string userLinks = "";

                                foreach (KeyValuePair<string, string> entry in ListOfHuman[i].HumanSiteData)
                                {
                                    userLinks += entry.Key + ": <" + ListOfHuman[i].HumanSiteData[entry.Key] + ">\n";
                                }

                                await message.Channel.SendMessageAsync(ListOfHuman[i].discordID + "\n" + userLinks);
                                return;
                            }
                        }//If the human is not in our data base, print out (below)
                        await message.Channel.SendMessageAsync(msgContent.Substring(8, msgContent.Length - 8) + "is not in our database");
                    }
                    else if (msgContent.Substring(8, 3) == ("<@&"))//Checks if @ is a Role
                    {
                        await message.Channel.SendMessageAsync("I can only get data for a specific user, not a Role ");

                    }
                    else if (msgContent.Substring(8, 2) == ("<@"))//Checks if @ is a Role
                    {
                        await message.Channel.SendMessageAsync("WOAH, WHY IS A ROBOT TALKIN TO ME, WHO THE HECK YOU THINK YOU ARE");
                    }
                    else if (msgContent.Substring(7, 3) == "Add")
                    {
                        int keyToURLIndex = msgContent.Substring(11).IndexOf(" ");

                        if (keyToURLIndex == -1)
                        {
                            await message.Channel.SendMessageAsync("Formatting Error, please sperate the name of the URL, and the URL with an space");
                            return;
                        }


                        int updateResult = UpdateUserDataList(
                                                              "<@!" + msg.Author.Id.ToString() + ">",
                                                              msg.Author.ToString(), msgContent.Substring(11, keyToURLIndex),
                                                              msgContent.Substring(11 + keyToURLIndex + 1,
                                                              msgContent.Length - (11 + keyToURLIndex + 1))
                                                              );

                        if (updateResult == 1)
                        {
                            await message.Channel.SendMessageAsync("There is already a Link with the same name as this, if you would like to edit, type ProfileEdit");
                        }
                        else if (updateResult == 0)
                        {
                            await message.Channel.SendMessageAsync("Link Successfully Added");
                        }

                    }

                    else if (msgContent.Substring(7, 4) == "Edit")
                    {
                        await message.Channel.SendMessageAsync("Edit Function not yet added, if you really want to change it, Ping @Quantum Blue#1234");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Command not found");
                    }

                }
            }

            //Language Filter
            if (LanguageFilter(msg.ToString().Substring(argPos)) == true && chatFilterEnabled == true)
            {
                await message.Channel.SendMessageAsync("watch your profanity\n<https://youtu.be/25f2IgIrkD4>");
                return;
            }
        }

        public bool LanguageFilter(string sentence)
        {
            string temp = sentence.ToLower();

            string[] BadWords =
                {
                    "Bulloks",
                    "Fuck",
                    "Faggot",
                    "Retard",
                    "Shit",
                    "Ass",
                    "Nigger",
                    "Faggot",
                    "Ass"
                };

            for (int i = 0; i < BadWords.Length; i++)
            {
                if (temp.Contains(BadWords[i].ToLower()) == true)
                {
                    return true;
                }
            }
            return false;


        }

        public void GetFilePath(string textFileName, ref string path)
        {
            path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(textFileName)).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path += "\\DiscordBotFiles\\" + textFileName;
        }

        public void GetUserDataFromFile(string path, ref List<DiscordHuman> ListOfHuman)
        {
            ListOfHuman = new List<DiscordHuman>();
            DiscordHuman tempHuman = new DiscordHuman();

            if (System.IO.File.Exists(path) == false) //If File Doesn't Exsist
            {
                System.IO.File.CreateText(path).Close();
                ListOfHuman = new List<DiscordHuman>();
            }

            string[] userSaveFileData = System.IO.File.ReadAllLines(path);


            for (int i = 0; i < userSaveFileData.Length; i++)
            {
                if (userSaveFileData[i].Equals("<NAME>"))
                {
                    tempHuman = new DiscordHuman();
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
                    ListOfHuman.Add(tempHuman);
                }
            }
        }

        public int UpdateUserDataList(string userID, string userName, string key, string URL)
        {
            //Seperate Adding to Method, from writting to file

            for (int i = 0; i < ListOfHuman.Count; i++)
            {
                if (ListOfHuman[i].discordID == userID)
                {
                    ListOfHuman[i].dicordUserName = userName;

                    string throwAwayString;

                    if (ListOfHuman[i].HumanSiteData.TryGetValue(key, out throwAwayString) == true) // Chekcks if the link already exsist
                    {
                        return 1;
                    }
                    else
                    {
                        ListOfHuman[i].HumanSiteData.Add(key, URL);
                        UpdateUserDataFile();
                        return 0;
                    }
                }
            }

            //Adding new Human
            DiscordHuman newHuman = new DiscordHuman
            {
                discordID = userID,
                dicordUserName = userName
            };
            newHuman.HumanSiteData.Add(key, URL);

            ListOfHuman.Add(newHuman);
            UpdateUserDataFile();
            return 0;
        }

        public void UpdateUserDataFile()
        {
            StreamWriter streamWriter = File.CreateText(userFileSavePath);

            for (int i = 0; i < ListOfHuman.Count; i++)
            {
                streamWriter.WriteLine("<NAME>");
                streamWriter.WriteLine(ListOfHuman[i].dicordUserName);
                streamWriter.WriteLine(ListOfHuman[i].discordID);

                foreach (KeyValuePair<string, string> entry in ListOfHuman[i].HumanSiteData)
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