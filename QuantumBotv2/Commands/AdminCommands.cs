using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuantumBotv2.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]
        public async Task Ping()
        {
            //var msg = await Context.Message.Channel.SendMessageAsync($"MS {Program.latency}");
            var botMsg = await Context.Message.Channel.SendMessageAsync($"Pong");

            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await botMsg.DeleteAsync();
            return;
        }

    }
}
