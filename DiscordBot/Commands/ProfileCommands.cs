using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot;

namespace DiscordBot.Commands
{
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Profile"), Alias("profile"), Summary("Prints out a packet of data of @person")]

        public async Task GetProfile([Remainder] string Input = "")
        {

            /*
            ID TAG 
            @ - Bot
            @! - Human
            @& - Role
            */

           

            Regex.Replace(Input, @"\s+", "");

            if (Input.Substring(0, 3) == ("<@!"))//Checks if @ is a human
            {
                for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
                {
                    if (Program.ListOfHumans[i].discordID == Input)
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
                            .WithThumbnailUrl("https://scontent-sea1-1.cdninstagram.com/vp/0d61cb4d1e534df2362d576808b49009/5CF44268/t51.2885-15/sh0.08/e35/p750x750/24327974_193834761176358_6741651937936015360_n.jpg?_nc_ht=scontent-sea1-1.cdninstagram.com")
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


                await Context.Channel.SendMessageAsync($"{Input} is not in our database");
            }
            else if (Input.Substring(0, 3) == ("<@&"))//Checks if @ is a Role
            {
                await Context.Channel.SendMessageAsync("I can only get data for a specific user, not a Role ");

            }
            else if (Input.Substring(0, 2) == ("<@"))//Checks if @ is a Role
            {
                await Context.Channel.SendMessageAsync("WOAH, WHY IS YOU TRYNNA GET INFO BOUT MY FELLOW BOTS, HUH?");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Command requierments not met");
            }
        }
    }
}
