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
                LogLevel = LogSeverity.Info
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

            // Let's do our global command
            var globalCommand = new SlashCommandBuilder()
                .WithName("SlashPing")
                .WithDescription("WOOOOO")
                .AddOption("First Var", ApplicationCommandOptionType.User, "Variable Description", isRequired: true);

            try
            {
                // With global commands we don't need the guild.
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

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
            switch (command.Data.Name)
            {
                case "SlashPing":
                    SocketGuildUser guildUser = (SocketGuildUser)command.Data.Options.First().Value;
                    var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

                    var embedBuiler = new EmbedBuilder()
                        .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                        .WithTitle("Roles")
                        .WithDescription(roleList)
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp();

                    // Now, Let's respond with the embed.
                    await command.RespondAsync(embed: embedBuiler.Build());
                    break;

            }
            await command.RespondAsync($"You executed {command.Data.Name}");
        }
    }
}
