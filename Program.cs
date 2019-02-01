using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace DiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

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
            if (message.Content == "OwO")
            {
                await message.Channel.SendMessageAsync("UwU");
            }
            if (message.Content == "!commands")
            {
                await message.Channel.SendMessageAsync("Suck my robot dick, Carter");
            }
            if (message.Content.Contains("Roasted"))
            {
                await message.Channel.SendMessageAsync("I heard someone needed this https://bit.ly/2CZbNrL");
            }
        }

    }
}