﻿using System;
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
using System.Collections.Generic;

namespace QuantumBotv2
{
    class Program
    {
        /*
        TODO
        Logging - User Messages
        Logging - Split Via Month? Week? Day?
        Logging - Server Logs, Seperate

        Telemetry
            Bot Uptime?
            User Messages Per Chat
            User Command Usage

        When User Joins
            Latency Logging/Ping
            Send Msg to "I am Logs"
            Create Profile

        When Username changed?
            Update Profile?

        When User Leaves
            Log out data

        Reaction Added Set Up
        Reaction Removed Set Up
        
        -Message Receieved
        When someone DM's the Bot, relay to Ray. Add edge case If Ray msm Bot
        
        Custom Keywords
            - Happy Birthday
            - Good Bot

        Ability to sync files through a command
        Ability to Spit out files through a command
        Ability to Update files through a command

        Custom Data Class's
            ReactEmote
                ServerRoleSetUpMsgID
            Monster Hunter
                Monster Hunter Monster Fail Count
                    Generic
                    Add/Subtract
                MonsterHunterNicknames
            Blackout Queue
            ChannelRoles (for react to get roles)
                add ephemeral response support
            WebsiteProfile

            
            (Depricated) Lunchbox
            (Depricated) BullietinEvent
            (Depricated) BulletinBoard
            (Depricated) Accountabilitbuddy

        */


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
                    try
                    {
                        Type logicClass = SlashCommandLogic.Instance.GetType();
                        MethodInfo logic = logicClass.GetMethod(cmdData.commandMethodName);

                        object[] parameters = new object[] { command };
                        logic.Invoke(SlashCommandLogic.Instance, parameters);
                    }
                    catch (Exception exception)
                    {
                        //TODO: log errors
                    }

                    return;

                }
            }

            //no logic
            await command.RespondAsync("No Logic For Slash Command Found");
        }


        private void ADMIN_ManuallyAddSlashCommand()
        {
            SlashCommands.SlashCommandData newScd = new SlashCommands.SlashCommandData(new SlashCommandBuilder()
                .WithName("slash-ping")
                .WithDescription("WOOOOO")
                .AddOption("user", ApplicationCommandOptionType.User, "Variable Description", isRequired: true)
                .AddOption("string", ApplicationCommandOptionType.String, "Variable String Description", isRequired: true
                ), "SlashPingCommand");

            //check if slashcommand with the same name exists
            List<SlashCommands.SlashCommandData> allSlashCommands = DataClassManager.Instance.slashCommands.AllSlashCommands;

            foreach (SlashCommands.SlashCommandData scd in allSlashCommands)
            {
                if (scd.slashCommandBuilder.Name == newScd.slashCommandBuilder.Name)
                {
                    //ERROR
                    return;
                }
            }
            DataClassManager.Instance.slashCommands.AllSlashCommands.Add(newScd);
            DataClassManager.Instance.SaveData(DataClassManager.Instance.slashCommands);

        }
    }
}
