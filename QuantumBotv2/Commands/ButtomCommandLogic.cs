using Discord;
using Discord.Commands;
using Discord.WebSocket;
using QuantumBotv2.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuantumBotv2.Commands
{
    public class ButtonCommandLogic
    {
        private static ButtonCommandLogic instance = null;
        public static ButtonCommandLogic Instance
        {
            get
            {
                return instance;
            }
        }

        public ButtonCommandLogic()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public async Task RoleButtonClicked(SocketMessageComponent component)
        {
            SocketGuildUser guildUser = (SocketGuildUser)component.User;
            switch (component.Data.CustomId)
            {
                case "programming-button":
                    await ToggleRoles("Programming", component);
                    break;
                case "art-button":
                    await ToggleRoles("Art", component);
                    break;
                case "design-button":
                    await ToggleRoles("Design", component);
                    break;
                case "techart-button":
                    await ToggleRoles("TechArt", component);
                    break;
                default:
                    await component.RespondAsync("You clicked a button that shouldn't exist. Hmmm", ephemeral: true);
                    break;
            }
        }
        public async Task<bool> ButtonCommandUserHasRoles(string[] roles, SocketMessageComponent component)
        {
            SocketGuildUser guildUser = (SocketGuildUser)component.User;

            foreach (string role in roles)
            {
                //get role from database
                var guildRole = guildUser.Guild.GetRole(DataClassManager.Instance.serverConfigs.roleID[role]);

                if (guildUser.Roles.Contains(guildRole))
                {
                    return true;
                }
            }

            await component.RespondAsync($"You do not have have the required roles to use this command. ({string.Join(", ", roles)}). \nMessage a Admin if you think this is wrong", ephemeral: true);
            return false;
        }


        public async Task OnButtonCommandInvoked(SocketMessageComponent component, bool isFailed)
        {
            SocketGuildUser guildUser = (SocketGuildUser)component.User;

            var embedBuiler = new EmbedBuilder()
                .WithThumbnailUrl(guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithAuthor(guildUser.ToString())
                .WithDescription($"ButtonID: \"{component.Data.CustomId}\"\nChannel: <#{component.Channel.Id}>")
                .WithCurrentTimestamp();

            if (isFailed)
            {
                embedBuiler.WithTitle($":warning: Failed Button Command Invoked! :warning:");
            }
            else
            {
                embedBuiler.WithTitle($"Button Command Invoked!");
            }

            await guildUser.Guild.GetTextChannel(DataClassManager.Instance.serverConfigs.channelID["Bot History"]).SendMessageAsync(embed: embedBuiler.Build());

            //Not RAM friendly
            DataClassManager.Instance.telemetryLog.allCommandTelemetryLogs.Add(new TelemetryLog.CommandTelemetryData(component));
            DataClassManager.Instance.SaveData(DataClassManager.Instance.telemetryLog);
        }
        private async Task ToggleRoles(string roleName, SocketMessageComponent component)
        {
            if (await ButtonCommandUserHasRoles(new string[] { "Student", "Guest", "Teacher" }, component) == false)
            {
                return;
            }

            SocketGuildUser guildUser = (SocketGuildUser)component.User;

            //check if they have the role,
            var guildRole = guildUser.Guild.GetRole(DataClassManager.Instance.serverConfigs.roleID[roleName]);

            if (guildUser.Roles.Contains(guildRole)) //they have the role, remove the role
            {
                try
                {
                    await guildUser.RemoveRoleAsync(guildRole);
                    await component.RespondAsync($":warning: Removing the {roleName} Role :warning: ", ephemeral: true);
                }
                catch (Exception)
                {
                    await component.RespondAsync($"Failed trying to remove the {roleName} Role. Are you an admin?", ephemeral: true);
                }
            }
            else
            {
                try
                {
                    await guildUser.AddRoleAsync(guildRole);
                    await component.RespondAsync($":sparkles: You gained the {roleName} Role! :sparkles:", ephemeral: true);
                }
                catch (Exception)
                {
                    await component.RespondAsync($"Failed trying to add the {roleName} Role. Are you an admin?", ephemeral: true);
                }
            }
        }

    }
}