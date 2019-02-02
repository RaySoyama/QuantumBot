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
            GetDataFromConfig(userFileSavePath, ref ListOfHuman);

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
                        "\n" +
                        "Mod Commands\n" +
                        "Quit             - Closes Bot\n" +
                        "ToggleChatFilter - Turns Chat Filter On/Off\n" +
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
                    if (msgContent.Substring(8, 3) == ("<@!"))//Human
                    {
                        for(int i = 0; i < ListOfHuman.Count; i++)
                        {
                            if (ListOfHuman[i].discordID == msgContent.Substring(8, msgContent.Length - 8))
                            {
                                string userLinks = "";
                                string tempLink = "";

                                ////opens all the fucking keys cuz im a dumbass
                                //if (ListOfHuman[i].HumanSiteData.TryGetValue("LinkedIn", out tempLink) == true)
                                //{
                                //    userLinks += "Linkedin: <" + tempLink + ">\n";
                                //}

                                //if (ListOfHuman[i].HumanSiteData.TryGetValue("GitHub", out tempLink) == true)
                                //{
                                //    userLinks += "GitHub: <" + tempLink + ">\n";
                                //}

                                //if (ListOfHuman[i].HumanSiteData.TryGetValue("Creddle", out tempLink) == true)
                                //{
                                //    userLinks += "Creddle: <" + tempLink + ">\n";
                                //}

                                //if (ListOfHuman[i].HumanSiteData.TryGetValue("Instagram", out tempLink) == true)
                                //{
                                //    userLinks += "Instagram: <" + tempLink + ">\n";
                                //}

                                //if (ListOfHuman[i].HumanSiteData.TryGetValue("Twitter", out tempLink) == true)
                                //{
                                //    userLinks += "Twitter: <" + tempLink + ">\n";
                                //}

                                foreach (KeyValuePair<string, string> entry in ListOfHuman[i].HumanSiteData)
                                {
                                    userLinks += entry.Key + ": <" + ListOfHuman[i].HumanSiteData[entry.Key] + ">\n";
                                }

                                await message.Channel.SendMessageAsync(ListOfHuman[i].discordID + "\n" + userLinks);
                            }
                        }
                        

                    }
                    //{
                    //    Console.WriteLine("NAME FOUND");

                    //}

                    //@Name (returns all)
                    //Add link
                    //Edit Key
                    
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

            for(int i = 0; i < BadWords.Length; i++)
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

        public void GetDataFromConfig(string path, ref List<DiscordHuman> ListOfHuman)
        {
            ListOfHuman = new List<DiscordHuman>();

            string[] allFileLines = System.IO.File.ReadAllLines(path);

            DiscordHuman tempHuman = new DiscordHuman();

            for (int i = 0; i < allFileLines.Length; i++)
            {
                if (allFileLines[i].Equals("<NAME>"))
                {
                    tempHuman = new DiscordHuman();
                    i++;
                    tempHuman.discordID = allFileLines[i];
                }
                else if (allFileLines[i].Equals("<LINK>"))
                {
                    i++;
                    tempHuman.HumanSiteData.Add(allFileLines[i], allFileLines[i + 1]); //THIS IS BREAKING FOR SOME REASON  
                }
                else if (allFileLines[i].Equals("<END>"))
                {
                    ListOfHuman.Add(tempHuman);
                }
            }
        }
    }
}