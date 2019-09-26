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
    public class QuantumBotCommands : ModuleBase<SocketCommandContext>
    {
        #region depricated
        //[Command("Profile"), Alias("profile"), Summary("Prints out a packet of data of @person")]

        //public async Task GetProfile(IUser user = null)
        //{
        //    if (user == null)
        //    {
        //        user = Context.User;
        //    }

        //    /*
        //    ID TAG 
        //    @ - Bot
        //    @! - Human
        //    @& - Role
        //    */

        //    if (user.IsBot == false)//Checks if @ is a human
        //    {
        //        for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //        {
        //            if (Program.ListOfHumans[i].discordID == $"<@!{user.Id}>")
        //            {

        //                string userLinks = "";
        //                int linkCount = 1;

        //                foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[i].HumanSiteData)
        //                {
        //                    userLinks += $"{linkCount}. [{entry.Key}]({Program.ListOfHumans[i].HumanSiteData[entry.Key]})\n\n";
        //                    linkCount++;
        //                }

        //                //https://leovoel.github.io/embed-visualizer/


        //                var builder = new EmbedBuilder()
        //                    .WithTitle("\n" + Program.ListOfHumans[i].dicordUserName)
        //                    .WithDescription(userLinks)
        //                    .WithColor(new Color(128, 128, 128))
        //                    .WithTimestamp(DateTimeOffset.Now)
        //                    .WithFooter(footer =>
        //                    {
        //                        footer
        //                          .WithText("Quantum Bot")
        //                          .WithIconUrl("https://avatars1.githubusercontent.com/u/42445829?s=400&v=4");
        //                    })
        //                    .WithThumbnailUrl(user.GetAvatarUrl())
        //                    //.WithImageUrl("Big dick image at the bottom")
        //                    .WithAuthor(author =>
        //                    {
        //                        author
        //                        .WithName("Summoned by " + Context.Message.Author.ToString())
        //                        .WithIconUrl(Context.Message.Author.GetAvatarUrl());
        //                    });


        //                var embed = builder.Build();
        //                await Context.Channel.SendMessageAsync("", embed: embed);
        //                return;
        //            }
        //        }//If the human is not in our data base, print out (below)


        //        await Context.Channel.SendMessageAsync($"{user.Username} is not in our database");
        //    }
        //    else if (user.IsBot == true)//Checks if @ is a Role
        //    {
        //        await Context.Channel.SendMessageAsync("WOAH, WHY IS YOU TRYNNA GET INFO BOUT MY FELLOW BOTS, HUH?");
        //    }
        //}

        //[Command("ProfileAdd"), Alias("profileadd", "Profileadd", "profileAdd"), Summary("Adds a Link to a users profile")]

        //public async Task AddProfile([Remainder]string Input = "None")
        //{
        //    string[] linkAndURL = Input.Split();

        //    if (linkAndURL.Length < 2)
        //    {
        //        await Context.Channel.SendMessageAsync("To add a link, please type \"[Name of Site](space)[URL]\"");
        //        return;
        //    }
        //    else if (linkAndURL.Length > 2)
        //    {
        //        await Context.Channel.SendMessageAsync("To add a site name that contains a space, subsitute space with a underscore \'_\'");
        //        return;
        //    }

        //    linkAndURL[0] = linkAndURL[0].Replace('_', ' ');

        //    try
        //    {
        //        var link = new Uri(linkAndURL[1]);
        //    }
        //    catch (UriFormatException e)
        //    {
        //        linkAndURL[1] = "https://" + linkAndURL[1];
        //    }

        //    try
        //    {
        //        var link = new Uri(linkAndURL[1]);
        //    }
        //    catch (UriFormatException e)
        //    {
        //        await Context.Channel.SendMessageAsync("Link Invalid");
        //        return;
        //    }


        //    int updateResult = Program.UpdateUserDataList("<@!" + Context.Message.Author.Id.ToString() + ">",
        //                                                   Context.Message.Author.ToString(),
        //                                                   linkAndURL[0], linkAndURL[1]);
        //    if (updateResult == 1)
        //    {
        //        await Context.Channel.SendMessageAsync("There is already a Link with the same name as this, if you would like to edit, type ProfileEdit");
        //    }
        //    else if (updateResult == 0)
        //    {
        //        await Context.Channel.SendMessageAsync("Link Successfully Added");
        //    }

        //}

        //[Command("ProfileEdit"), Alias("profileedit", "Profileedit", "profileEdit"), Summary("Edit a Link on a users profile")]

        //public async Task EditProfile([Remainder]string Input = "None")
        //{
        //    string[] userMessage = Input.Split();

        //    int index;
        //    string siteName;
        //    string URL;
        //    int userIndex = -1;


        //    for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //    {
        //        if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
        //        {
        //            userIndex = i;
        //            break;
        //        }
        //    }
        //    if (userIndex == -1)
        //    {
        //        await Context.Channel.SendMessageAsync("You are not in the data base, use ProfileAdd to add a link");
        //        return;
        //    }

        //    //Checks if Input is yeety
        //    if (userMessage.Length < 3)
        //    {
        //        await Context.Channel.SendMessageAsync("To edit a link, \"[Link Number] [New Site Name] [New URL]\"");
        //        return;
        //    }
        //    else if (userMessage.Length > 3)
        //    {
        //        await Context.Channel.SendMessageAsync("To add a site name that contains a space, subsitute space with a underscore \'_\'");
        //        return;
        //    }
        //    if (Int32.TryParse(userMessage[0], out index) == false)
        //    {
        //        await Context.Channel.SendMessageAsync("To edit a link, \"[index] [New Site Name] [New URL]\"");
        //        return;
        //    }
        //    else if (index < 1 || index > Program.ListOfHumans[userIndex].HumanSiteData.Count)
        //    {
        //        await Context.Channel.SendMessageAsync("Link Index out of Bounds");
        //        return;
        //    }

        //    siteName = userMessage[1].Replace('_', ' ');
        //    URL = userMessage[2];

        //    index--; // maching it so index 1 is array start
        //    Dictionary<string, string> NewList = new Dictionary<string, string>();

        //    int fakeIndex = 0;

        //    foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
        //    {
        //        if (fakeIndex == index)
        //        {
        //            NewList.Add(siteName, URL);
        //        }
        //        else
        //        {
        //            NewList.Add(entry.Key, entry.Value);
        //        }
        //        fakeIndex++;
        //    }

        //    fakeIndex = 0;

        //    Program.ListOfHumans[userIndex].HumanSiteData = NewList;
        //    Program.UpdateUserDataFile();

        //    await Context.Channel.SendMessageAsync("Link Successfully Edited");
        //}

        //[Command("ProfileDelete"), Alias("profiledelete", "Profiledelete", "profileDelete"), Summary("Delete a users profile")]

        //public async Task DeleteProfile([Remainder]string Input = "None")
        //{
        //    if (Input != "DELETE")
        //    {
        //        await Context.Channel.SendMessageAsync("Please Write \"DELETE\" after command to remove your profile");
        //        return;
        //    }

        //    int userIndex = -1;

        //    for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //    {
        //        if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
        //        {
        //            Program.ListOfHumans.Remove(Program.ListOfHumans[i]);
        //            Program.UpdateUserDataFile();
        //            await Context.Channel.SendMessageAsync("Profile Deleted");
        //            return;
        //        }
        //    }
        //    await Context.Channel.SendMessageAsync("You are not in the data base");
        //    return;

        //}

        //[Command("ProfileRemove"), Alias("profileremove", "Profileremove", "profileRemove"), Summary("Delete a Link on a users profile")]

        //public async Task DeleteLinkProfile(int index = -1)
        //{
        //    int userIndex = -1;

        //    if (index == -1)
        //    {
        //        await Context.Channel.SendMessageAsync("Please Specify what Link you would like to delete");
        //        return;
        //    }

        //    for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //    {
        //        if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
        //        {
        //            userIndex = i;
        //            break;
        //        }
        //    }

        //    if (userIndex == -1)
        //    {
        //        await Context.Channel.SendMessageAsync("You are not in the data base");
        //        return;
        //    }

        //    if (index < 1 || index > Program.ListOfHumans[userIndex].HumanSiteData.Count)
        //    {
        //        await Context.Channel.SendMessageAsync("Link Index out of Bounds");
        //        return;
        //    }

        //    //input is valid, lets deletin
        //    index--; // maching it so index 1 is array start
        //    Dictionary<string, string> NewList = new Dictionary<string, string>();

        //    int fakeIndex = 0;

        //    foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
        //    {
        //        if (fakeIndex == index)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            NewList.Add(entry.Key, entry.Value);
        //        }
        //        fakeIndex++;
        //    }

        //    fakeIndex = 0;

        //    Program.ListOfHumans[userIndex].HumanSiteData = NewList;
        //    Program.UpdateUserDataFile();

        //    await Context.Channel.SendMessageAsync("Link Successfully Deleted");

        //}

        //[Command("ProfileSwap"), Alias("profileswap", "Profileswap", "profileSwap"), Summary("Delete a Link on a users profile")]

        //public async Task SwapLink(int index1 = -1, int index2 = -1)
        //{
        //    int userIndex = -1;

        //    if (index1 == -1 || index2 == -1)
        //    {
        //        await Context.Channel.SendMessageAsync("Please Specify what Link you would like to swap by giving the index");
        //        return;
        //    }

        //    for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //    {
        //        if (Program.ListOfHumans[i].discordID == $"<@!{Context.Message.Author.Id.ToString()}>")
        //        {
        //            userIndex = i;
        //            break;
        //        }
        //    }

        //    if (userIndex == -1)
        //    {
        //        await Context.Channel.SendMessageAsync("You are not in the data base");
        //        return;
        //    }

        //    if (index1 < 1 || index1 > Program.ListOfHumans[userIndex].HumanSiteData.Count || index2 < 1 || index2 > Program.ListOfHumans[userIndex].HumanSiteData.Count)
        //    {
        //        await Context.Channel.SendMessageAsync("Link Index out of Bounds");
        //        return;
        //    }

        //    //input is valid, lets deletin
        //    index1--; // maching it so index 1 is array start
        //    index2--; // maching it so index 1 is array start
        //    Dictionary<string, string> NewList = new Dictionary<string, string>();
        //    KeyValuePair<string, string> index1Data = new KeyValuePair<string, string>();
        //    KeyValuePair<string, string> index2Data = new KeyValuePair<string, string>();

        //    int fakeIndex = 0;

        //    foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
        //    {
        //        if (fakeIndex == index1)
        //        {
        //            index1Data = entry;
        //        }

        //        if (fakeIndex == index2)
        //        {
        //            index2Data = entry;
        //        }
        //        fakeIndex++;
        //    }
        //    fakeIndex = 0;

        //    foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[userIndex].HumanSiteData)
        //    {
        //        if (fakeIndex == index1)
        //        {
        //            NewList.Add(index2Data.Key, index2Data.Value);
        //            fakeIndex++;
        //            continue;
        //        }

        //        if (fakeIndex == index2)
        //        {
        //            NewList.Add(index1Data.Key, index1Data.Value);
        //            fakeIndex++;
        //            continue;
        //        }

        //        else
        //        {
        //            NewList.Add(entry.Key, entry.Value);
        //            fakeIndex++;
        //        }
        //    }

        //    fakeIndex = 0;

        //    Program.ListOfHumans[userIndex].HumanSiteData = NewList;
        //    Program.UpdateUserDataFile();

        //    await Context.Channel.SendMessageAsync("Link Successfully Swapped");
        //}

        //[Command("UpdateList"), Alias("updatelist", "Updatelist", "updateList"), Summary("Rearanges order of links")]

        //public async Task UpdateList()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    var AdminCode = Context.Guild.GetRole(487403594300129291);

        //    if (user.Roles.Contains(AdminCode) == true)
        //    {
        //        Program.GetUserDataFromFile(Program.userFileSavePath, ref Program.ListOfHumans);
        //        await Context.Message.Channel.SendMessageAsync("Updated");
        //    }
        //    else
        //    {
        //        await Context.Message.Channel.SendMessageAsync("Admin Rights Required");
        //    }
        //}
        
        //[Command("PrintAll"), Alias("printall", "printAll", "Printall"), Summary("Prints everyones profile")]

        //public async Task PrintAll()
        //{
        //    var user = Context.User as SocketGuildUser;
        //    var AdminCode = Context.Guild.GetRole(487403594300129291);

        //    if (user.Roles.Contains(AdminCode) == true)
        //    {
        //        //Program.GetUserDataFromFile(Program.userFileSavePath, ref Program.ListOfHumans);
        //        //await Context.Message.Channel.SendMessageAsync("Updated");

        //        for (int i = 0; i < Program.ListOfHumans.Count; i++) //goes through database, 
        //        {

        //            string userLinks = "";
        //            int linkCount = 1;

        //            foreach (KeyValuePair<string, string> entry in Program.ListOfHumans[i].HumanSiteData)
        //            {
        //                userLinks += $"{linkCount}. [{entry.Key}]({Program.ListOfHumans[i].HumanSiteData[entry.Key]})\n\n";
        //                linkCount++;
        //            }

        //            //https://leovoel.github.io/embed-visualizer/


        //            var builder = new EmbedBuilder()
        //                .WithTitle("\n" + Program.ListOfHumans[i].dicordUserName)
        //                .WithDescription(userLinks)
        //                .WithColor(new Color(128, 128, 128))
        //                .WithTimestamp(DateTimeOffset.Now)
        //                .WithFooter(footer =>
        //                {
        //                    footer
        //                      .WithText("Quantum Bot")
        //                      .WithIconUrl("https://avatars1.githubusercontent.com/u/42445829?s=400&v=4");
        //                })
        //                .WithThumbnailUrl(user.GetAvatarUrl())
        //                //.WithImageUrl("Big dick image at the bottom")
        //                .WithAuthor(author =>
        //                {
        //                    author
        //                    .WithName("Summoned by " + Context.Message.Author.ToString())
        //                    .WithIconUrl(Context.Message.Author.GetAvatarUrl());
        //                });


        //            var embed = builder.Build();
        //            await Context.Channel.SendMessageAsync("", embed: embed);
        //            //return;
        //        }

        //        await Context.Channel.SendMessageAsync($"{user.Username} is not in our database");

        //    }
        //    else
        //    {
        //        await Context.Message.Channel.SendMessageAsync("Admin Rights Required");
        //    }
        //}

        #endregion

        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]

        public async Task Ping()
        {
            await Context.Message.Channel.SendMessageAsync($"MS {Program.latecy}");
            return;
        }

        [Command("InitializeWebEmbeds"), Alias("InitializeWeb"), Summary("Seeds the website embeds")]

        public async Task InitializeWebsites()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }   

            List<Embed> newWebsiteEmbedList = CreateWebsiteEmbeds();

            foreach (Embed embed in newWebsiteEmbedList)
            {
                await Context.Channel.SendMessageAsync(null,embed: embed).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return;
        }

        [Command("UpdateWebEmbed"), Alias("UpdateEmbed"), Summary("Updates the Website Embed")]

        public async Task UpdateWebsiteEmbed(string WebQuery)
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }

            if (WebQuery == "All") //update all
            {
                foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
                {
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

                    if (ChatReferences is IUserMessage msg)
                    {
                        await msg.ModifyAsync(x => x.Embed = GetEmbedWebsite(web));
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                return;
            }


            foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
            {
                if (WebQuery.ToLower() == web.ToString().ToLower())
                {
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

                    if (ChatReferences is IUserMessage msg)
                    {
                        await msg.ModifyAsync(x => x.Embed = GetEmbedWebsite(web));
                        return;
                    }
                }
            }

            await Context.Message.Channel.SendMessageAsync($"Invalid Website Query");
            return;
        }


        
        [Command("Help"), Alias("help"), Summary("List of all commands")]

        public async Task HelpList()
        {
            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot Commands")
                          .WithDescription(
                                           $"Prefix - '{Program.prefix}'\n" +
                                           $"Help\n" +
                                           $""

                                           )
                          .WithColor(new Color(60, 179, 113))
                          .WithTimestamp(DateTimeOffset.Now)
                          .WithFooter(footer =>
                          {
                              footer
                                .WithText("Quantum Bot");
                                //.WithIconUrl("https://avatars1.githubusercontent.com/u/42445829?s=400&v=4");
                          });

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync("", embed: embed);
            return;
        }

        [Command("Quit"), Alias("quit"), Summary("Quits the bot exe, only Admins an run")]

        public async Task Quit()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);
            
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
        




        private List<Embed> CreateWebsiteEmbeds()
        {
            List<Embed> newWebsiteEmbedList = new List<Embed>();

            foreach (KeyValuePair<Program.WEBSITES, WebsiteProfile> web in Program.WebsiteData)
            {
                var WebsiteEmbed = new EmbedBuilder()
                   .WithColor(web.Value.WebsiteColor)
                   .WithTimestamp(DateTimeOffset.Now)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText("Last Updated");
                   })
                   .WithThumbnailUrl(web.Value.WebsiteIconURL)
                   .AddField("Teachers", "Placeholder")
                   .AddField("2020", "Placeholder")
                   .AddField("2021", "Placeholder");

                 newWebsiteEmbedList.Add(WebsiteEmbed.Build());
            }


            return newWebsiteEmbedList;
        }

        private Embed GetEmbedWebsite(Program.WEBSITES web)
        {
            string teacherData = "";
            string gradData = "";
            string TwoZeroData = "";
            string TwoOneData = "";

            //LinkedIn Default Embed
            var WebsiteEmbed = new EmbedBuilder()
               .WithColor(Program.WebsiteData[web].WebsiteColor)
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl(Program.WebsiteData[web].WebsiteIconURL);


            foreach (UserProfile user in Program.UserData)
            {
                if (user.UserWebsiteIndex[web] != null)
                {
                    if (user.isTeacher == true)
                    {
                        teacherData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                        break;
                    }
                    else if (user.isStudent == true)
                    {
                        if (user.GradYear == 2020)
                        {
                            TwoZeroData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                            break;
                        }
                        else if (user.GradYear == 2021)
                        {
                            TwoOneData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                            break;
                        }
                        else if (user.GradYear < 2020)
                        {
                            gradData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                            break;
                        }
                    }

                }
            }

            if (teacherData != "")
            {
                WebsiteEmbed.AddField("Teachers", teacherData);
            }
            else if (gradData != "")
            {
                WebsiteEmbed.AddField("Graduates", gradData);
            }
            else if (TwoZeroData != "")
            {
                WebsiteEmbed.AddField("2020", TwoZeroData);
            }
            else if (TwoOneData != "")
            {
                WebsiteEmbed.AddField("2021", TwoOneData);
            }

            return WebsiteEmbed.Build();
        }
    }


}
