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
            }

            return;
        }

        /*
        [Command("UpdateAllWebEmbeds"), Alias("UpdateAllWeb"), Summary("Edits the already exsisting Web embeds, this reaches the Rate Limit, so dont call this")]
        public async Task UpdateAllWebEmbed()
        {
            List<IMessage> ChatReferences = new List<IMessage>
            {
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["Creddle"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["LinkedIn"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["GitHub"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["ArtStation"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["Personal"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["Twitter"], CacheMode.AllowDownload),
                await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["Instagram"], CacheMode.AllowDownload)
            };

            //I DON'T HAVE AN ERROR CHECK TO MAKE SURE THE CHATS EXIST, CUZ THEY SHOULD

            List<Embed> UpdatedWebsiteEmbeds = UpdateAllWebsiteEmbeds();

            for (int i = 0; i < UpdatedWebsiteEmbeds.Count; i++)
            {
                await ((IUserMessage)ChatReferences[i]).ModifyAsync(x => x.Embed = UpdatedWebsiteEmbeds[i]);
            }


            return;
        }
        */

        [Command("UpdateCreddleEmbed"), Alias("UpdateCreddle"), Summary("Updates the Creddle Embed")]

        public async Task UpdateCreddleEmbed()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }

            IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["Creddle"], CacheMode.AllowDownload);

            if (ChatReferences is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = GetCreddleEmbed());
            }
            return;
        }


        [Command("UpdateLinkedInEmbed"), Alias("UpdateLinkedIn"), Summary("Updates the LinkedIn Embed")]

        public async Task UpdateLinkedInEmbed()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }

            IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.PointersAnonWebsiteID["LinkedIn"], CacheMode.AllowDownload);

            if (ChatReferences is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = GetLinkedInEmbed());
            }
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
        

        //Embed Creation Scripts


        private List<Embed> CreateWebsiteEmbeds()
        {
            List<Embed> newWebsiteEmbedList = new List<Embed>();

            //Creddle Default Embed
            var CreddleEmbed = new EmbedBuilder()
               .WithColor(new Color(39, 130, 130))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626204428260737057/sUvz1kky_400x400.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");

            //////////////////////////////////////////////////////////

            //Linkedin Default Embed
            var LinkedInEmbed = new EmbedBuilder()
               .WithColor(new Color(0, 119, 181))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626311018498228235/LI-In-Bug.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");

            //////////////////////////////////////////////////////////

            //GitHub Default Embed
            var GitHubEmbed = new EmbedBuilder()
               .WithColor(new Color(27, 29, 35))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626311584175620109/GitHub-Mark-120px-plus.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");

            //////////////////////////////////////////////////////////

            //ArtStation Default Embed
            var ArtStationEmbed = new EmbedBuilder()
               .WithColor(new Color(19, 175, 240))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626313302397419530/logo-artstation-plain.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");

            //////////////////////////////////////////////////////////

            //Personal Default Embed
            var PersonalEmbed = new EmbedBuilder()
               .WithColor(new Color(0, 0, 0))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626313819408433170/map_023-globe-location-earth-website-512.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");


            //////////////////////////////////////////////////////////

            //Instagram Default Embed
            var InstagramEmbed = new EmbedBuilder()
               .WithColor(new Color(131, 58, 180))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626314091430150154/Instagram_AppIcon_Aug2017.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");

            //////////////////////////////////////////////////////////

            //Twitter Default Embed
            var TwitterEmbed = new EmbedBuilder()
               .WithColor(new Color(29, 161, 242))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626314390513254400/Twitter_Social_Icon_Square_Color.png")
               .AddField("Teachers", "Placeholder")
               .AddField("2020", "Placeholder")
               .AddField("2021", "Placeholder");


            newWebsiteEmbedList.Add(CreddleEmbed.Build());
            newWebsiteEmbedList.Add(LinkedInEmbed.Build());
            newWebsiteEmbedList.Add(GitHubEmbed.Build());
            newWebsiteEmbedList.Add(ArtStationEmbed.Build());
            newWebsiteEmbedList.Add(PersonalEmbed.Build());
            newWebsiteEmbedList.Add(TwitterEmbed.Build());
            newWebsiteEmbedList.Add(InstagramEmbed.Build());

            return newWebsiteEmbedList;
        }

        /*
        private List<Embed> UpdateAllWebsiteEmbeds()
        {
            List<Embed> newWebsiteEmbedList = new List<Embed>();


            //////////////////////////////////////////////////////////
            string teacherData = "";
            string gradData = "";
            string TwoZeroData = "";
            string TwoOneData = "";

            //Creddle Default Embed
            var CreddleEmbed = new EmbedBuilder()
               .WithColor(new Color(39, 130, 130))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626204428260737057/sUvz1kky_400x400.png");


            foreach (UserProfile user in Program.UserData)
            {
                if (user.Creddle != null)
                {
                    if (user.isTeacher == true)
                    {
                        teacherData += $"[{user.userNickname}]({user.Creddle})\n";
                        break;
                    }
                    else if (user.isStudent == true)
                    {
                        if (user.GradYear == 2020)
                        {
                            TwoZeroData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                        else if (user.GradYear == 2021)
                        {
                            TwoOneData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                        else if (user.GradYear < 2020)
                        {
                            gradData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                    }

                }
            }

            if (teacherData != "")
            {
                CreddleEmbed.AddField("Teachers", teacherData);
            }
            else if (gradData != "")
            {
                CreddleEmbed.AddField("Graduates", gradData);
            }
            else if (TwoZeroData != "")
            {
                CreddleEmbed.AddField("2020", TwoZeroData);
            }
            else if (TwoOneData != "")
            {
                CreddleEmbed.AddField("2021", TwoOneData);
            }

            teacherData = "";
            gradData = "";
            TwoZeroData = "";
            TwoOneData = "";


            //////////////////////////////////////////////////////////

            //Linkedin Default Embed
            var LinkedinEmbed = new EmbedBuilder()
               .WithColor(new Color(0, 119, 181))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626311018498228235/LI-In-Bug.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");


            //////////////////////////////////////////////////////////

            //GitHub Default Embed
            var GitHubEmbed = new EmbedBuilder()
               .WithColor(new Color(27, 29, 35))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626311584175620109/GitHub-Mark-120px-plus.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");

            //////////////////////////////////////////////////////////

            //ArtStation Default Embed
            var ArtStationEmbed = new EmbedBuilder()
               .WithColor(new Color(19, 175, 240))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626313302397419530/logo-artstation-plain.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");

            //////////////////////////////////////////////////////////

            //Personal Default Embed
            var PersonalEmbed = new EmbedBuilder()
               .WithColor(new Color(0, 0, 0))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626313819408433170/map_023-globe-location-earth-website-512.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");


            //////////////////////////////////////////////////////////

            //Instagram Default Embed
            var InstagramEmbed = new EmbedBuilder()
               .WithColor(new Color(131, 58, 180))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626314091430150154/Instagram_AppIcon_Aug2017.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");

            //////////////////////////////////////////////////////////

            //Twitter Default Embed
            var TwitterEmbed = new EmbedBuilder()
               .WithColor(new Color(29, 161, 242))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer => {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626314390513254400/Twitter_Social_Icon_Square_Color.png")
               .AddField("Teachers", "Updated Placeholder")
               .AddField("2020", "Updated Placeholder")
               .AddField("2021", "Updated Placeholder");


            newWebsiteEmbedList.Add(CreddleEmbed.Build());
            newWebsiteEmbedList.Add(LinkedinEmbed.Build());
            newWebsiteEmbedList.Add(GitHubEmbed.Build());
            newWebsiteEmbedList.Add(ArtStationEmbed.Build());
            newWebsiteEmbedList.Add(PersonalEmbed.Build());
            newWebsiteEmbedList.Add(TwitterEmbed.Build());
            newWebsiteEmbedList.Add(InstagramEmbed.Build());

            return newWebsiteEmbedList;
        }
         */

        private Embed GetCreddleEmbed()
        {
            string teacherData = "";
            string gradData = "";
            string TwoZeroData = "";
            string TwoOneData = "";

            //Creddle Default Embed
            var CreddleEmbed = new EmbedBuilder()
               .WithColor(new Color(39, 130, 130))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626204428260737057/sUvz1kky_400x400.png");


            foreach (UserProfile user in Program.UserData)
            {
                if (user.Creddle != null)
                {
                    if (user.isTeacher == true)
                    {
                        teacherData += $"[{user.userNickname}]({user.Creddle})\n";
                        break;
                    }
                    else if (user.isStudent == true)
                    {
                        if (user.GradYear == 2020)
                        {
                            TwoZeroData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                        else if (user.GradYear == 2021)
                        {
                            TwoOneData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                        else if (user.GradYear < 2020)
                        {
                            gradData += $"[{user.userNickname}]({user.Creddle})\n";
                            break;
                        }
                    }

                }
            }

            if (teacherData != "")
            {
                CreddleEmbed.AddField("Teachers", teacherData);
            }
            else if (gradData != "")
            {
                CreddleEmbed.AddField("Graduates", gradData);
            }
            else if (TwoZeroData != "")
            {
                CreddleEmbed.AddField("2020", TwoZeroData);
            }
            else if (TwoOneData != "")
            {
                CreddleEmbed.AddField("2021", TwoOneData);
            }

            return CreddleEmbed.Build();
        }

        private Embed GetLinkedInEmbed()
        {
            string teacherData = "";
            string gradData = "";
            string TwoZeroData = "";
            string TwoOneData = "";

            //LinkedIn Default Embed
            var LinkedInEmbed = new EmbedBuilder()
               .WithColor(new Color(39, 130, 130))
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/626311018498228235/LI-In-Bug.png");


            foreach (UserProfile user in Program.UserData)
            {
                if (user.LinkedIn != null)
                {
                    if (user.isTeacher == true)
                    {
                        teacherData += $"[{user.userNickname}]({user.LinkedIn})\n";
                        break;
                    }
                    else if (user.isStudent == true)
                    {
                        if (user.GradYear == 2020)
                        {
                            TwoZeroData += $"[{user.userNickname}]({user.LinkedIn})\n";
                            break;
                        }
                        else if (user.GradYear == 2021)
                        {
                            TwoOneData += $"[{user.userNickname}]({user.LinkedIn})\n";
                            break;
                        }
                        else if (user.GradYear < 2020)
                        {
                            gradData += $"[{user.userNickname}]({user.LinkedIn})\n";
                            break;
                        }
                    }

                }
            }

            if (teacherData != "")
            {
                LinkedInEmbed.AddField("Teachers", teacherData);
            }
            else if (gradData != "")
            {
                LinkedInEmbed.AddField("Graduates", gradData);
            }
            else if (TwoZeroData != "")
            {
                LinkedInEmbed.AddField("2020", TwoZeroData);
            }
            else if (TwoOneData != "")
            {
                LinkedInEmbed.AddField("2021", TwoOneData);
            }

            return LinkedInEmbed.Build();
        }
    }


}
