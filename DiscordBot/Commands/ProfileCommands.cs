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
                user = Context.User;
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
                        int linkCount = 1;

                        foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[i].HumanSiteData)
                        {
                            userLinks += $"{linkCount}. [{entry.Key}]({Program.ListOfHumans[i].HumanSiteData[entry.Key]})\n\n";
                            linkCount++;
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

        [Command("ProfileAdd"), Alias("profileadd", "Profileadd", "profileAdd"), Summary("Adds a Link to a users profile")]

        public async Task AddProfile([Remainder]string Input = "None")
        {
            string[] linkAndURL = Input.Split();

            if (linkAndURL.Length < 2)
            {
                await Context.Channel.SendMessageAsync("To add a link, please type \"[Name of Site](space)[URL]\"");
                return;
            }
            else if (linkAndURL.Length > 2)
            {
                await Context.Channel.SendMessageAsync("To add a site name that contains a space, subsitute space with a underscore \'_\'");
                return;
            }

            linkAndURL[0] = linkAndURL[0].Replace('_', ' ');

            int updateResult = Program.UpdateUserDataList("<@!" + Context.Message.Author.Id.ToString() + ">",
                                                           Context.Message.Author.ToString(),
                                                           linkAndURL[0], linkAndURL[1]);
            if (updateResult == 1)
            {
                await Context.Channel.SendMessageAsync("There is already a Link with the same name as this, if you would like to edit, type ProfileEdit");
            }
            else if (updateResult == 0)
            {
                await Context.Channel.SendMessageAsync("Link Successfully Added");
            }

        }

        [Command("ProfileEdit"), Alias("profileedit", "Profileedit", "profileEdit"), Summary("Edit a Link on a users profile")]

        public async Task EditProfile([Remainder]string Input = "None")
        {
            string[] userMessage = Input.Split();

            int index;
            string siteName;
            string URL;
            int userIndex = -1;


            for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
            {
                if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
                {
                    userIndex = i;
                    break;
                }
            }
            if (userIndex == -1)
            {
                await Context.Channel.SendMessageAsync("You are not in the data base, use ProfileAdd to add a link");
                return;
            }

            //Checks if Input is yeety
            if (userMessage.Length < 3)
            {
                await Context.Channel.SendMessageAsync("To edit a link, \"[Link Number] [New Site Name] [New URL]\"");
                return;
            }
            else if (userMessage.Length > 3)
            {
                await Context.Channel.SendMessageAsync("To add a site name that contains a space, subsitute space with a underscore \'_\'");
                return;
            }
            if (Int32.TryParse(userMessage[0], out index) == false)
            {
                await Context.Channel.SendMessageAsync("To edit a link, \"[index] [New Site Name] [New URL]\"");
                return;
            }
            else if (index < 1 || index > Program.ListOfHumans[userIndex].HumanSiteData.Count)
            {
                await Context.Channel.SendMessageAsync("Link Index out of Bounds");
                return;
            }

            siteName = userMessage[1].Replace('_',' ');
            URL = userMessage[2];

            index--; // maching it so index 1 is array start
            Dictionary<string, string> NewList = new Dictionary<string, string>();

            int fakeIndex = 0;

            foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
            {
                if (fakeIndex == index)
                {
                    NewList.Add(siteName, URL);
                }
                else
                {
                    NewList.Add(entry.Key, entry.Value);
                }
                fakeIndex++;
            }

            fakeIndex = 0;

            Program.ListOfHumans[userIndex].HumanSiteData = NewList;
            Program.UpdateUserDataFile();

            await Context.Channel.SendMessageAsync("Link Successfully Edited");
        }

        [Command("ProfileDelete"), Alias("profiledelete", "Profiledelete", "profileDelete"), Summary("Delete a users profile")]

        public async Task DeleteProfile([Remainder]string Input = "None")
        {
            if (Input != "DELETE")
            {
                await Context.Channel.SendMessageAsync("Please Write \"DELETE\" after command to remove your profile");
                return;
            }

            int userIndex = -1;

            for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
            {
                if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
                {
                    Program.ListOfHumans.Remove(Program.ListOfHumans[i]);
                    Program.UpdateUserDataFile();
                    await Context.Channel.SendMessageAsync("Profile Deleted");
                    return;
                }
            }
            await Context.Channel.SendMessageAsync("You are not in the data base");
            return;

        }

        [Command("ProfileRemove"), Alias("profileremove", "Profileremove", "profileRemove"), Summary("Delete a Link on a users profile")]

        public async Task DeleteLinkProfile(int index = -1)
        {
            int userIndex = -1;

            if (index == -1)
            {
                await Context.Channel.SendMessageAsync("Please Specify what Link you would like to delete");
            }

            for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
            {
                if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
                {
                    userIndex = i;
                    break;
                }
            }

            if (userIndex == -1)
            {
                await Context.Channel.SendMessageAsync("You are not in the data base");
                return;
            }

            if (index < 1 || index > Program.ListOfHumans[userIndex].HumanSiteData.Count)
            {
                await Context.Channel.SendMessageAsync("Link Index out of Bounds");
                return;
            }

            //input is valid, lets deletin
            index--; // maching it so index 1 is array start
            Dictionary<string, string> NewList = new Dictionary<string, string>();

            int fakeIndex = 0;

            foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
            {
                if (fakeIndex == index)
                {
                    continue;
                }
                else
                {
                    NewList.Add(entry.Key, entry.Value);
                }
                fakeIndex++;
            }

            fakeIndex = 0;

            Program.ListOfHumans[userIndex].HumanSiteData = NewList;
            Program.UpdateUserDataFile();

            await Context.Channel.SendMessageAsync("Link Successfully Deleted");

        }

        /*
         * Things to add
         *
         * Profile Delete
         * profileSwap
         * 
         * help
         */


        [Command("UpdateList"), Alias("updatelist","Updatelist", "updateList"), Summary("Updates The human list from the file, admin only")]

        public async Task UpdateList()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(487403594300129291);

            if (user.Roles.Contains(AdminCode) == true)
            {
                Program.GetUserDataFromFile(Program.userFileSavePath, ref Program.ListOfHumans);
                await Context.Message.Channel.SendMessageAsync("Updated");
            }
            else
            {
                await Context.Message.Channel.SendMessageAsync("Admin Rights Required");
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
