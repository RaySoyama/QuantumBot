using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Threading;
using System.IO;

namespace DiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        DiscordSocketClient client;

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            //setup
            client.Log += Log;
            string token = "NTQwNjc3MDgwMjk2MTk0MDc5.DzUbVQ.EBSdDBSLjVN_L3Ho_aES9MNG-Fo"; //Discord Bot Token
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            ConnectionState test = client.ConnectionState;

            client.MessageReceived += MessageReceived;
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            bool isNewSentence;
            string oldmsg = "";

            var msg = message as SocketUserMessage; // cast to a USER msg;

            if (msg.ToString() == oldmsg)
            {
                isNewSentence = false;
            }
            else
            {
                oldmsg = msg.ToString();
                isNewSentence = true;
            }

            if (msg == null || msg.Author.ToString() == "Quantum Bot#5354") //if msg is from bot,
            {
                return;
            }


            string path = "DiscordChatData.txt";



            path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(path)).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path = System.IO.Directory.GetParent(path).ToString();
            path += "\\DiscordChatData.txt";

            string chatLog = "Time: " + msg.Timestamp.ToString() + "\nChannel: " + msg.Channel.ToString() + "\nUsername: " + msg.Author.ToString() + "\nMessage: " + msg.ToString() + "\n\n";

            
            File.AppendAllText(path, chatLog);

            Console.WriteLine(chatLog);
            int argPos = 0;
            char prefix = '*';

            if (LanguageFilter(msg.ToString().Substring(argPos)) == true && isNewSentence == true)
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync("DONT FUCKING SWEAR IN MY FUCKING SERVER, REEEEEEEEEEE");
               
                await message.Channel.SendMessageAsync("jk, " + msg.Author.ToString() + " said  \"" + msg.ToString() + "\"");
                return;
            }

            if (msg.ToString().ToLower().Contains("lol"))
            {
                await message.Channel.SendMessageAsync("NO LAUGHING IN THIS SERVER");
            }
            else if (msg.ToString().ToLower().Contains("!help"))
            {
                await message.Channel.SendMessageAsync("I ain't Dyno bot, who the fuck?");
            }
            else if (msg.ToString().ToLower() == "!quit" && msg.Author.ToString() == "Quantum Blue#1234")
            {
                await message.Channel.SendMessageAsync("I'll be back - Steve Jobs");
                System.Environment.Exit(1);
            }
        }

        public bool LanguageFilter(string sentence)
        {
            string temp = sentence.ToLower();

            string[] BadWords =
                {
                "Fuck",
                "Shit",
                "Ass",
                "Terry",
                "UwU",
                "OwO",
                "Snuggles",
                "Bulgy",
                "o3o",
                "Rawr",
                "nuzzles",
                "daddy",
                "shaft",
                "nyan",
                "moans",
                "musky",
                "paws",
                "rubbies",
                "salty",
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

    }
}