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
        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]

        public async Task Ping()
        {
            await Context.Message.Channel.SendMessageAsync($"MS {Program.latecy}");
            return;
        }

        // Personal Link Chunk


        //Initialization
        /*
        [Command("InitializeWebEmbeds"), Alias("InitializeWeb"), Summary("Seeds the website embeds")]

        public async Task InitializeWebsites()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);

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
        */

        [Command("UpdateWebEmbed"), Alias("UpdateEmbed"), Summary("Updates the Website Embed")]

        public async Task UpdateWebsiteEmbed(string WebQuery)
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }

            if (WebQuery == "All") //update all
            {
                foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
                {
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.serverConfigs.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

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
                string test = web.ToString();
                if (WebQuery.ToLower() == test.ToLower())
                {
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.serverConfigs.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

                    if (ChatReferences is IUserMessage msg)
                    {
                        await msg.ModifyAsync(x => x.Embed = GetEmbedWebsite(web));
                        await Context.User.SendMessageAsync($"Embed Updated");
                        await Context.Message.DeleteAsync();
                        return;
                    }
                }
            }
            await Context.User.SendMessageAsync($"{Context.Message.ToString()}\nInvalid Website Query");
            await Context.Message.DeleteAsync();
            return;
        }


        [Command("Website"), Alias("website"), Summary("Updates, or adds the users website")]

        public async Task UpdateWebsite([Remainder] string shitUserSaid)
        {
            if (Context.Channel.Id != Program.serverConfigs.PointersAnonChatID["Personal Links"])
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.serverConfigs.PointersAnonChatID["Personal Links"]}>");
                await Context.Message.DeleteAsync();
            }


            string[] splitMsg = shitUserSaid.Split();

            if (splitMsg.Length != 2)
            {
                return;
            }


            string WebQuery = splitMsg[0];
            string webURL = splitMsg[1];

            foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
            {
                if (WebQuery.ToLower() == web.ToString().ToLower())
                {

                    UserProfile user = GetUserProfile(Context.Message.Author.Id);

                    //Update User Data
                    user.UserWebsiteIndex[web] = webURL;
                    if (webURL == "null")
                    {
                        await Context.User.SendMessageAsync($"{Context.Message.ToString()}\nLink Removed");
                        user.UserWebsiteIndex[web] = null;
                    }


                    user.userNickname = ((IGuildUser)Context.Message.Author).Nickname;

                    var guildUser = Context.User as SocketGuildUser;

                    if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Guest"])) == true)
                    {
                        user.isGuest = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Teacher"])) == true)
                    {
                        user.isTeacher = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Certified"])) == true)
                    {
                        user.isStudent = true;

                        if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Class Of 2020"])) == true)
                        {
                            user.GradYear = 2020;
                        }
                        else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Class Of 2021"])) == true)
                        {
                            user.GradYear = 2021;
                        }
                        else
                        {
                            user.GradYear = 6969;
                        }
                    }

                    Program.SaveUserDataToFile();

                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.serverConfigs.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

                    if (ChatReferences is IUserMessage msg)
                    {
                        await msg.ModifyAsync(x => x.Embed = GetEmbedWebsite(web));
                        await Context.User.SendMessageAsync($"{Context.Message.ToString()}\nLink Updated");
                        await Context.Message.DeleteAsync();

                        return;
                    }
                }
            }
            await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\nInvalid Website");
            await Context.Message.DeleteAsync();
            return;
        }

        [Command("AdminWebsite"), Alias("adminwebsite"), Summary("Updates, or adds the users website")]

        public async Task AdminUpdateWebsite([Remainder] string shitUserSaid)
        {
            var adminCheck = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);

            if (adminCheck.Roles.Contains(AdminCode) == false)
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                    $"This Command can only be used by an Admin");
                await Context.Message.DeleteAsync();
                return;
            }

            string[] splitMsg = shitUserSaid.Split();

            if (splitMsg.Length != 3)
            {
                return;
            }

            string WebQuery = splitMsg[1];
            string webURL = splitMsg[2];

            foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
            {

                if (WebQuery.ToLower() == web.ToString().ToLower())
                {

                    UserProfile user = GetUserProfile(UInt64.Parse(splitMsg[0]));

                    //Update User Data
                    user.UserWebsiteIndex[web] = webURL;
                    if (webURL == "null")
                    {
                        user.UserWebsiteIndex[web] = null;
                    }


                    user.userNickname = ((IGuildUser)Context.Guild.GetUser(UInt64.Parse(splitMsg[0]))).Nickname;

                    var guildUser = Context.Guild.GetUser(UInt64.Parse(splitMsg[0])) as SocketGuildUser;

                    if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Guest"])) == true)
                    {
                        user.isGuest = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Teacher"])) == true)
                    {
                        user.isTeacher = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Certified"])) == true)
                    {
                        user.isStudent = true;

                        if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Class Of 2020"])) == true)
                        {
                            user.GradYear = 2020;
                        }
                        else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Class Of 2021"])) == true)
                        {
                            user.GradYear = 2021;
                        }
                        else
                        {
                            user.GradYear = 6969;
                        }
                    }

                    Program.SaveUserDataToFile();

                    IMessage ChatReferences = await Context.Guild.GetTextChannel(Program.serverConfigs.PointersAnonChatID["Personal Links"]).GetMessageAsync(Program.serverConfigs.WebsiteData[web].WebsiteChatID, RequestOptions.Default);

                    if (ChatReferences is IUserMessage msg)
                    {
                        await msg.ModifyAsync(x => x.Embed = GetEmbedWebsite(web));
                        await Context.User.SendMessageAsync($"{Context.Message.ToString()}\nLink Updated");
                        await Context.Message.DeleteAsync();

                        return;
                    }
                }
            }
            await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\nInvalid Website");
            await Context.Message.DeleteAsync();
            return;

        }

        [Command("UnityVersion"), Alias("unityverion", "unityVersion", "UnityVer", "unityVer", "Unityver", "unityver"), Summary("What Version of Unity are we using?")]

        public async Task UnityVersion()
        {
            string versions = "";
            foreach (string ver in Program.serverConfigs.UnityVersion)
            {
                versions += $"{ver}\n";
            }

            var builder = new EmbedBuilder()
            .WithTitle("Current Unity Versions AIE Supports")
            .WithDescription("[Click here to go to download page](https://unity3d.com/get-unity/download/archive)")
            .WithColor(new Color(0, 0, 0))
            .WithThumbnailUrl(Program.serverConfigs.UnityIconURL)
            .AddField("Versions", versions);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(
                null,
                embed: embed)
                .ConfigureAwait(false);
        }


        [Command("ProposalTemplate"), Alias("PPTemplate", "pptemplate", "proposalTemplate", "proposaltemplate", "Proposaltemplate")]

        public async Task SendProposalTemplate()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Here you go~ \n{Program.serverConfigs.ProjectProposalDocURL}");
            return;
        }


        [Command("AddLunchbox"), Alias("AddLB","addLB","addlb","addlunchbox", "addlunchBox", "addLunchBox", "Addlunchbox", "AddlunchBox", "AddLunchBox")]
        public async Task AddLunchboxEvent(int lunchboxDateYear, int lunchboxDateMonth, int lunchboxDateDay, string lunchboxTopic, string lunchboxSpeaker)
        {
            var adminCheck = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);

            if (adminCheck.Roles.Contains(AdminCode) == false)
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                    $"This Command can only be used by an Admin");
                await Context.Message.DeleteAsync();
                return;
            }


            //ToString("dddd, dd MMMM yyyy")
            DateTime newLunchboxDate = new DateTime(lunchboxDateYear, lunchboxDateMonth, lunchboxDateDay);

            var builder = new EmbedBuilder()
                .WithColor(new Color(37, 170, 225))
                //.WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/637797971752386560/DuEMx03WsAE1zhp.png")
                .AddField($"{lunchboxTopic}",$"{lunchboxSpeaker}\n{newLunchboxDate.ToString("dddd, dd MMMM yyyy")}");

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);

            //Test return
            //await Context.Message.Channel.SendMessageAsync($"Test Return 1 {newLunchboxDate.ToString("dddd, dd MMMM yyyy")}");
            //await Context.Message.Channel.SendMessageAsync($"Test Return 2 {lunchboxTopic}");
            //await Context.Message.Channel.SendMessageAsync($"Test Return 3 {lunchboxSpeaker}");
        }

        //[Command("Lunchbox"), Alias("lunchbox", "lunchBox", "LunchBox")]
        //public async Task GetLunchboxList()
        //{ 


        //}



        //Events
        /*
        [Command("Inktober"), Alias("inktober"), Summary("Returns todays inktober prompt")]

        public async Task TodaysInktoberPrompt()
        {
            DateTime dateTime = DateTime.Now;



            string[] inkPrompts = new string[] {"Ring",
                                                "Mindless",
                                                "Bait",
                                                "Freeze",
                                                "Build",
                                                "Husky",
                                                "Enchanted",
                                                "Frail",
                                                "Swing",
                                                "Pattern",
                                                "Snow",
                                                "Dragon",
                                                "Ash",
                                                "Overgrown",
                                                "Legend",
                                                "Wild",
                                                "Ornament",
                                                "Misfit",
                                                "Sling",
                                                "Tread",
                                                "Treasure",
                                                "Ghost",
                                                "Ancient",
                                                "Dizzy",
                                                "Tasty",
                                                "Dark",
                                                "Coat",
                                                "Ride",
                                                "Injured",
                                                "Catch",
                                                "Ripe"};


            var builder = new EmbedBuilder()
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/628301292346802204/Inktober.png")
                .AddField("Todays Inktober Prompt", $"\"{inkPrompts[int.Parse(dateTime.ToString("dd")) - 1]}\"");

            if (int.Parse(dateTime.ToString("dd")) < inkPrompts.Count())
            {
                builder.AddField("Tomorrows Inktober Prompt", $"\"{inkPrompts[int.Parse(dateTime.ToString("dd"))]}\"");
            }
            else
            {
                builder.AddField("Todays the last day of Inktober!~", $"");
            }

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);

            //await Context.Channel.SendMessageAsync($"Todays date is {inkPrompts[int.Parse(dateTime.ToString("dd")) - 1]}");
            return;
        }
        */

        [Command("Iceborne"), Alias("iceborne"), Summary("Returns Days till Iceborne")]

        public async Task DaysToMHW()
        {
            if (Context.Channel.Id != Program.serverConfigs.PointersAnonChatID["Monster Hunter"])
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.serverConfigs.PointersAnonChatID["Monster Hunter"]}>");
                await Context.Message.DeleteAsync();
                return;
            }

            DateTime endTime = new DateTime(2020, 01, 09, 0, 0, 0);
            TimeSpan ts = endTime.Subtract(DateTime.Now);
            string daysTill = ts.ToString("d' Days 'h' Hours 'm' Minutes 's' Seconds'");

            var builder = new EmbedBuilder()
                .WithColor(new Color(37, 170, 225))
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/637797971752386560/DuEMx03WsAE1zhp.png")
                .AddField("Monster Hunter World Iceborne Countdown", daysTill);

            var embed = builder.Build();
            await Context.Guild.GetTextChannel(Program.serverConfigs.PointersAnonChatID["Monster Hunter"]).SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }


        [Command("SendIntro"), Alias("sendintro", "sendIntro", "Sendintro")]

        public async Task SendIntro(ulong targetUser)
        {
            var adminCheck = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);

            if (adminCheck.Roles.Contains(AdminCode) == false)
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                    $"This Command can only be used by an Admin");
                await Context.Message.DeleteAsync();
                return;
            }

            if (Context.Guild.GetUser(targetUser) == null)
            {
                await Context.Guild.GetTextChannel(Program.serverConfigs.PointersAnonChatID["Bot History"]).SendMessageAsync("Invalid User ID");
            }
            else
            {
                await Context.Guild.GetUser(targetUser).SendMessageAsync($"Welcome {Context.Guild.GetUser(targetUser).Mention} to Pointers Anonymous, the unofficial AIE Discord server!\n" +
                                        $"I am the helper bot created by <@!173226502710755328> to maintain the server\n\n" +
                                        $"Read the rules at <#{Program.serverConfigs.PointersAnonChatID["The Law"]}> and to gain access to some of the server's channels, \n" +
                                        $"Introduce yourself at <#{Program.serverConfigs.PointersAnonChatID["Introductions"]}>! (It's okay if you don't)\n" +
                                        $"      Prefered Name:\n" +
                                        $"      Occupation:\n" +
                                        $"      Favorite food:\n\n" +
                                        $"If you are a AIE Student, please state your\n" +
                                        $"      Full Name:\n" +
                                        $"      Alias (Optional):\n" +
                                        $"      Graduating Year:\n" +
                                        $"      Enrolled Course:\n\n" +
                                        $"If you have any questions, feel free to DM one of the Admins"
                                        );
                                        
            }
        }


        //[Command("test"), Alias("test", "test", "test")]
        //public async Task testTest()
        //{
        //    //<@&487403594300129291>
        //    await Context.Guild.GetTextChannel(Program.serverConfigs.PointersAnonChatID["Admin"]).SendMessageAsync($"Welcome Sorry.I'll.Stop.Spam to Pointers Anonymous, the unofficial AIE Discord server!\n" +
        //                                 $"I am the helper bot created by <@!173226502710755328> to maintain the server\n" +
        //                                 $"To gain access to all of the server's channels, read the rules at <#{Program.serverConfigs.PointersAnonChatID["The Law"]}>\n" +
        //                                 $"introduce yourself at <#{Program.serverConfigs.PointersAnonChatID["Introductions"]}>, and tell us your\n" +
        //                                 $"      Full Name:\n" +
        //                                 $"      Alias (Optional):\n" +
        //                                 $"      Graduating Year:\n" +
        //                                 $"      Enrolled Course:\n\n" +
        //                                 $"If you have any questions, feel free to DM one of the Admins\n\n" +
        //                                 $"If you are not an AIE student, please tell us who you're associated with, so we can get a role set up for you~\n" +
        //                                 $"(If you're from a different campus, also include that info)"
        //                                 );

        //}



        [Command("Help"), Alias("help"), Summary("List of all commands")]

        public async Task HelpList()
        {
            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot Commands")
                          .AddField("General",$"[Command] - [Description]\n" +
                                              $"UnityVersion - Gets the Unity Version we are using\n" +
                                              $"ProposalTemplate - Gets the Project Proposal Template from the handbook\n" +
                                              $"Inktober - Gets the Inktober prompt")
                          .AddField("Personal Link Stuff", $"Website (Domain) (URL) - Posting your link\n" +
                                                           $"Example:\n" +
                                                           $"`Website LinkedIn https://www.linkedin.com/in/raysoyama/` \n" +
                                                           $"Website (Domain) null - Removes your link")
                           .AddField("Bot Stuff",$"Prefix is {Program.serverConfigs.prefix}\n" +
                                               $"Help - See list of Commands\n" +
                                               $"Ping - See the Latency of bot")
                          .WithColor(new Color(60, 179, 113))
                          .WithTimestamp(DateTimeOffset.Now)
                          .WithFooter(footer =>
                          {
                              footer
                                .WithText("Quantum Bot");
                                //.WithIconUrl("https://avatars1.githubusercontent.com/u/42445829?s=400&v=4");
                          });

            var embed = builder.Build();
            await Context.User.SendMessageAsync("", embed: embed);
            await Context.Message.DeleteAsync();


            return;
        }

        [Command("Quit"), Alias("quit"), Summary("Quits the bot exe, only Admins an run")]

        public async Task Quit()
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.serverConfigs.PointersAnonRoleID["Admin"]);
            
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




        //Raid Area 51, send msg
        /*
        [Command("RaidArea51")]

        public async Task SendMessege()
        {
            var adminCheck = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.PointersAnonRoleID["Admin"]);

            if (adminCheck.Roles.Contains(AdminCode) == false)
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                    $"This Command can only be used by an Admin");
                await Context.Message.DeleteAsync();
                return;
            }

           
            foreach (UserProfile user in Program.UserData)
            {
                var target = Context.Guild.GetUser(user.userID);
                
                await target.SendMessageAsync($" \n" +
                                          $"Hello~,\n" +
                                          $"I am Quantum Bot, the helper bot created by <@!{Program.PointersAnonUserID["Ray Soyama"]}> (Ray) to maintain the server~\n" +
                                          $"I recently added a new feature in the <#{Program.PointersAnonChatID["Personal Links"]}> channel, and I made it real nice lookin' ( ͡• ͜ʖ ͡•)\n\n" +
                                          $"`I ported over the old links to this squeeky clean version; so could you double check to see if you links are working?`\n\n" +
                                          $"Also if you want to add more links to  the posts, you can use the command \n" +
                                          $"{Program.prefix}Website [Domain] [URL]\n" +
                                          $"These are the current list of Domains you can choose from~\n" +
                                          $"`Creddle` `LinkedIn` `GitHub` `ArtStation` `Personal` `Twitter` `Instagram`\n\n" +
                                          $"If you have any questions, or have any requests for new features, you can put them in <#{Program.PointersAnonChatID["Quantum Bot"]}> and I'll peek at them later~");

                await Task.Delay(TimeSpan.FromSeconds(1));

            }
            

            //var target = Context.Guild.GetUser(Program.PointersAnonUserID["Ray Alt"]);

            //await target.SendMessageAsync($" \n" +
            //                              $"Hello~,\n" +
            //                              $"I am Quantum Bot, the helper bot created by <@!{Program.PointersAnonUserID["Ray Soyama"]}> (Ray) to maintain the server~\n" +
            //                              $"I recently added a new feature in the <#{Program.PointersAnonChatID["Personal Links"]}> channel, and I made it real nice lookin' ( ͡• ͜ʖ ͡•)\n\n" +
            //                              $"`I ported over the old links to this squeeky clean version; so could you double check to see if you links are working?`\n\n" +
            //                              $"Also if you want to add more links to  the posts, you can use the command \n" +
            //                              $"{Program.prefix}Website [Domain] [URL]\n" +
            //                              $"These are the current list of Domains you can choose from~\n" +
            //                              $"`Creddle` `LinkedIn` `GitHub` `ArtStation` `Personal` `Twitter` `Instagram`\n\n" +
            //                              $"If you have any questions, or have any requests for new features, you can put them in <#{Program.PointersAnonChatID["Quantum Bot"]}> and I'll peek at them later~");
            return;
        }
            */

        private UserProfile GetUserProfile(ulong userID)
        {
            foreach (UserProfile user in Program.UserData)
            {
                if (user.userID == userID)
                {
                    return user;
                }
            }

            UserProfile newProfile = new UserProfile
            {
                userID = userID,
                UserWebsiteIndex = new Dictionary<Program.WEBSITES, string>()
            };

            //makes an empty entry of null for each website.
            foreach (KeyValuePair<Program.WEBSITES, WebsiteProfile> web in Program.serverConfigs.WebsiteData)
            {
                newProfile.UserWebsiteIndex.Add(web.Key, null);
            }

            Program.UserData.Add(newProfile);
            return newProfile;


        }

        private List<Embed> CreateWebsiteEmbeds()
        {
            List<Embed> newWebsiteEmbedList = new List<Embed>();

            foreach (KeyValuePair<Program.WEBSITES, WebsiteProfile> web in Program.serverConfigs.WebsiteData)
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
               .WithColor(Program.serverConfigs.WebsiteData[web].WebsiteColor)
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl(Program.serverConfigs.WebsiteData[web].WebsiteIconURL);


            foreach (UserProfile user in Program.UserData)
            {
                if (user.UserWebsiteIndex[web] != null)
                {
                    if (user.isTeacher == true)
                    {
                        teacherData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                    }
                    else if (user.isStudent == true)
                    {
                        if (user.GradYear == 2020)
                        {
                            TwoZeroData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                        }
                        else if (user.GradYear == 2021)
                        {
                            TwoOneData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                        }
                        else if (user.GradYear < 2020)
                        {
                            gradData += $"[{user.userNickname}]({user.UserWebsiteIndex[web]})\n";
                        }
                    }

                }
            }

            if (teacherData != "")
            {
                WebsiteEmbed.AddField("Teachers", teacherData);
            }
            if (gradData != "")
            {
                WebsiteEmbed.AddField("Graduates", gradData);
            }
            if (TwoZeroData != "")
            {
                WebsiteEmbed.AddField("2020", TwoZeroData);
            }
            if (TwoOneData != "")
            {
                WebsiteEmbed.AddField("2021", TwoOneData);
            }

            return WebsiteEmbed.Build();
        }    
    }
}
