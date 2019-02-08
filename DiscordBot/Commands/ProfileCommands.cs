using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot;

namespace DiscordBot.Commands
{
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Profile"), Alias("profile"), Summary("Prints out a packet of data of @person")]

        public async Task GetProfile(IUser user = null)
        {
            if (user == null)
            {
                await Context.Message.Channel.SendMessageAsync("Not a valid name");
                return;
            }

            /*
            ID TAG 
            @ - Bot
            @! - Human
            @& - Role
            */
            
            
            if (user.IsBot == false)//Checks if @ is a human
            {
                for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
                {   
                    if (Program.ListOfHumans[i].discordID == $"<@!{user.Id}>")
                    {

                        string userLinks = "";

                        foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[i].HumanSiteData)
                        {
                            userLinks += "[" + entry.Key + "](" + Program.ListOfHumans[i].HumanSiteData[entry.Key] + ")\n\n";
                        }

                        //https://leovoel.github.io/embed-visualizer/


                        var builder = new EmbedBuilder()
                            .WithTitle("\n" + Program.ListOfHumans[i].dicordUserName)
                            .WithDescription(userLinks)
                            .WithColor(new Color(128, 128, 128))
                            .WithTimestamp(DateTimeOffset.Now)
                            .WithFooter(footer =>
                            {
                                footer
                                  .WithText("Quantum Bot")
                                  .WithIconUrl("https://avatars1.githubusercontent.com/u/42445829?s=400&v=4");
                            })
                            .WithThumbnailUrl(user.GetAvatarUrl())
                            //.WithImageUrl("Big dick image at the bottom")
                            .WithAuthor(author =>
                            {
                                author
                                .WithName("Summoned by " + Context.Message.Author.ToString())
                                .WithIconUrl(Context.Message.Author.GetAvatarUrl());
                            });


                        var embed = builder.Build();
                        await Context.Channel.SendMessageAsync("", embed: embed);
                        return;
                    }
                }//If the human is not in our data base, print out (below)


                await Context.Channel.SendMessageAsync($"{user.Username} is not in our database");
            }
            else if (user.IsBot == true)//Checks if @ is a Role
            {
                await Context.Channel.SendMessageAsync("WOAH, WHY IS YOU TRYNNA GET INFO BOUT MY FELLOW BOTS, HUH?");
            }
        }

        [Command("Quit"), Alias("quit"), Summary("Quits the bot exe, only Admins an run")]

        public async Task Quit()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(487403594300129291);

            if (user.Roles.Contains(AdminCode) == true)
            {
                await Context.Message.Channel.SendMessageAsync("I'll be back - Gandhi\nhttps://media.giphy.com/media/gFwZfXIqD0eNW/giphy.gif");
                System.Environment.Exit(1);
            }
            else
            {
                await Context.Message.Channel.SendMessageAsync("Admin Rights Required");
            }
        }
    }
}
