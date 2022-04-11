using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using QuantumBotv2.DataClass;
using QuantumBotv2.Commands;

namespace QuantumBotv2
{
    class Program
    {

        private static DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        private async Task MainAsync()
        {
            //Init Data
            DataClassManager dataClassManager = new DataClassManager();
            DataClassManager.Instance.LoadAllData();

            //Init Slash Command Logic
            SlashCommandLogic slashCommandLogic = new SlashCommandLogic();

            /*
            DataClassManager.Instance.slashCommands.AllSlashCommands.Add(
                new SlashCommands.SlashCommandData(new SlashCommandBuilder()
                .WithName("slash-ping")
                .WithDescription("WOOOOO")
                .AddOption("user", ApplicationCommandOptionType.User, "Variable Description", isRequired: true)
                .AddOption("string", ApplicationCommandOptionType.String, "Variable String Description", isRequired: true
                ), "SlashPingCommand"));

            DataClassManager.Instance.SaveData(DataClassManager.Instance.slashCommands);
            */

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true,
                ConnectionTimeout = 60000
            });

            commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            await client.LoginAsync(TokenType.Bot, DataClassManager.Instance.serverConfigs.Token);
            //attach event listeners
            await client.StartAsync();
            client.Ready += OnClientIsReady;
            client.MessageReceived += OnMessageReceived;
            client.SlashCommandExecuted += OnSlashCommandCalled;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            services = new ServiceCollection().BuildServiceProvider();

            await Task.Delay(-1);
        }

        private async Task OnClientIsReady()
        {
            await client.SetGameAsync($"{DataClassManager.Instance.serverConfigs.prefix}Help");
            await LoadSlashCommands();
        }

        private async Task LoadSlashCommands()
        {
            var guild = client.GetGuild(DataClassManager.Instance.serverConfigs.serverID);

            try
            {
                // With global commands we don't need the guild.
                SlashCommands slashCommands = DataClassManager.Instance.slashCommands;
                foreach (SlashCommands.SlashCommandData cmdData in slashCommands.AllSlashCommands)
                {
                    await guild.CreateApplicationCommandAsync(cmdData.slashCommandBuilder.Build());
                }
            }
            catch (Exception exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.ToString(), Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(client, message);

            if (context.User.IsBot == false)
            {
                Console.WriteLine(arg.ToString());
            }


            //Commands
            int argPos = 0;

            //if valid command prompt
            if (message.HasCharPrefix(DataClassManager.Instance.serverConfigs.prefix, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) //Checks for Prefix or @Quantum Bot
            {
                var result = await commands.ExecuteAsync(context, argPos, services);

                //if bot command is good
                if (result.IsSuccess == true)
                {
                    //TODO: Add logging when command gets called. Add admin only filter
                }
                else if (result.IsSuccess == false) //If the command failed, run this
                {

                }
            }

        }

        private async Task OnSlashCommandCalled(SocketSlashCommand command)
        {
            var guild = client.GetGuild(DataClassManager.Instance.serverConfigs.serverID);

            SlashCommands slashCommands = DataClassManager.Instance.slashCommands;

            foreach (SlashCommands.SlashCommandData cmdData in slashCommands.AllSlashCommands)
            {
                if (cmdData.slashCommandBuilder.Name == command.Data.Name)
                {
                    //reflect and call function
                    Type logicClass = SlashCommandLogic.Instance.GetType();
                    MethodInfo logic = logicClass.GetMethod(cmdData.commandMethodName);

                    object[] parameters = new object[] { command };
                    logic.Invoke(SlashCommandLogic.Instance, parameters);
                    return;
                }
            }

            //no logic
            await command.RespondAsync("No Logic For Slash Command Found");
        }
    }
}
