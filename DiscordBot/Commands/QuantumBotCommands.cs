using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class QuantumBotCommands : ModuleBase<SocketCommandContext>
    {
        /*       _____                                        _ 
         *      / ____|                                      | |
         *     | |  __    ___   _ __     ___   _ __    __ _  | |
         *     | | |_ |  / _ \ | '_ \   / _ \ | '__|  / _` | | |
         *     | |__| | |  __/ | | | | |  __/ | |    | (_| | | |
         *      \_____|  \___| |_| |_|  \___| |_|     \__,_| |_|                                                   
         */

        [Command("Help"), Alias("help"), Summary("List of all commands")]
        public async Task HelpList()
        {
            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot Commands")
                          .AddField("General", $"[Command] - [Description]\n" +
                                              $"UnityVersion - Gets the Unity Version we are using\n" +
                                              $"ProposalTemplate - Gets the Project Proposal Template from the handbook\n" +
                                              $"Inktober - Gets the Inktober prompt")
                          .AddField("Personal Link Stuff", $"Website (Domain) (URL) - Posting your link\n" +
                                                           $"Example:\n" +
                                                           $"`Website LinkedIn https://www.linkedin.com/in/raysoyama/` \n" +
                                                           $"Website (Domain) null - Removes your link")
                           .AddField("Bot Stuff", $"Prefix is {Program.ServerConfigData.prefix}\n" +
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

        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]
        public async Task Ping()
        {
            var msg = await Context.Message.Channel.SendMessageAsync($"MS {Program.latecy}");

            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await msg.DeleteAsync();
            return;
        }

        [Command("VaultSeeker"), Alias("vaultSeeker", "vaultseeker", "Vaultseeker")]
        public async Task ToggleVault()
        {
            if (await IsUserAuthorized("Student") == false)
            {
                return;
            }

            if ((Context.User as IGuildUser).RoleIds.Contains(Program.ServerConfigData.PointersAnonRoleID["Vault Seeker"]) == false)
            {
                await (Context.User as IGuildUser).AddRoleAsync(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Vault Seeker"]));
                await Context.User.SendMessageAsync("Vault Seeker Enabled");
            }
            else
            {
                await (Context.User as IGuildUser).RemoveRoleAsync(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Vault Seeker"]));
                await Context.User.SendMessageAsync("Vault Seeker Disabled");
            }

            await Context.Message.DeleteAsync();
        }

        [Command("Suggestion"), Alias("suggestion", "suggest", "Suggest")]
        public async Task AddSuggestion([Remainder] string input)
        {

            var builder = new EmbedBuilder()
                              .WithDescription(input);

            var embed = builder.Build();
            var botMsg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Suggestions"]).SendMessageAsync($"Suggestion by <@!{Context.User.Id}>", embed: embed).ConfigureAwait(false);

            await botMsg.AddReactionAsync(new Emoji("\U0001f44D"));
            await botMsg.AddReactionAsync(new Emoji("\U0001f44E"));

            await Context.Message.DeleteAsync();
        }


        //--------------------------------------------------------------------------------

        [Command("UnityVersion"), Alias("unityversion", "unityVersion", "UnityVer", "unityVer", "Unityver", "unityver"), Summary("What Version of Unity are we using?")]
        public async Task UnityVersion()
        {
            string versions = "";
            foreach (string ver in Program.ServerConfigData.UnityVersion)
            {
                versions += $"{ver}\n";
            }

            var builder = new EmbedBuilder()
            .WithTitle("Current Unity Versions AIE Supports")
            .WithDescription("[Click here to go to download page](https://unity3d.com/get-unity/download/archive)")
            .WithColor(new Color(0, 0, 0))
            .WithThumbnailUrl(Program.ServerConfigData.UnityIconURL)
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
            await Context.User.SendMessageAsync($"Here you go~ \n{Program.ServerConfigData.ProjectProposalDocURL}");
            return;
        }

        [Command("Iceborne"), Alias("iceborne"), Summary("Returns Days till Iceborne")]
        public async Task DaysToMHW()
        {
            if (Context.Channel.Id != Program.ServerConfigData.PointersAnonChatID["Monster Hunter"])
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.ServerConfigData.PointersAnonChatID["Monster Hunter"]}>");
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
            await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Monster Hunter"]).SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }

        //Inktober
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




        /*
         *      _____                    __   _   _        
         *     |  __ \                  / _| (_) | |       
         *     | |__) |  _ __    ___   | |_   _  | |   ___ 
         *     |  ___/  | '__|  / _ \  |  _| | | | |  / _ \
         *     | |      | |    | (_) | | |   | | | | |  __/
         *     |_|      |_|     \___/  |_|   |_| |_|  \___|
         *                                                                                                
         */

        [Command("UpdateWebEmbed"), Alias("UpdateEmbed"), Summary("Updates the Website Embed")]
        public async Task UpdateWebsiteEmbed(string WebQuery)
        {
            var user = Context.User as SocketGuildUser;
            var AdminCode = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Admin"]);

            if (user.Roles.Contains(AdminCode) == false)
            {
                return;
            }

            if (WebQuery == "All") //update all
            {
                foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
                {
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.ServerConfigData.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

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
                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.ServerConfigData.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

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
            if (Context.Channel.Id != Program.ServerConfigData.PointersAnonChatID["Personal Links"])
            {
                await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.ServerConfigData.PointersAnonChatID["Personal Links"]}>");
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

                    if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Guest"])) == true)
                    {
                        user.isGuest = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Teacher"])) == true)
                    {
                        user.isTeacher = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Student"])) == true)
                    {
                        user.isStudent = true;

                        if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2020"])) == true)
                        {
                            user.GradYear = 2020;
                        }
                        else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2021"])) == true)
                        {
                            user.GradYear = 2021;
                        }
                        else
                        {
                            user.GradYear = 6969;
                        }
                    }

                    Program.SaveUserDataToFile();

                    IMessage ChatReferences = await Context.Channel.GetMessageAsync(Program.ServerConfigData.WebsiteData[web].WebsiteChatID, CacheMode.AllowDownload);

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
        public async Task AdminUpdateWebsite(ulong TargetUser, string WebQuery, string webURL)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            foreach (Program.WEBSITES web in Enum.GetValues(typeof(Program.WEBSITES)))
            {

                if (WebQuery.ToLower() == web.ToString().ToLower())
                {

                    UserProfile user = GetUserProfile(TargetUser);

                    //Update User Data
                    user.UserWebsiteIndex[web] = webURL;
                    if (webURL == "null")
                    {
                        user.UserWebsiteIndex[web] = null;
                    }

                    user.userNickname = ((IGuildUser)Context.Guild.GetUser(TargetUser)).Nickname;

                    var guildUser = Context.Guild.GetUser(TargetUser) as SocketGuildUser;

                    if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Guest"])) == true)
                    {
                        user.isGuest = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Teacher"])) == true)
                    {
                        user.isTeacher = true;
                    }
                    else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Student"])) == true)
                    {
                        user.isStudent = true;

                        if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2020"])) == true)
                        {
                            user.GradYear = 2020;
                        }
                        else if (guildUser.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2021"])) == true)
                        {
                            user.GradYear = 2021;
                        }
                        else
                        {
                            user.GradYear = 6969;
                        }
                    }

                    Program.SaveUserDataToFile();

                    IMessage ChatReferences = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Personal Links"]).GetMessageAsync(Program.ServerConfigData.WebsiteData[web].WebsiteChatID, RequestOptions.Default);

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




        /*      _                               _       _                    
         *     | |                             | |     | |                   
         *     | |       _   _   _ __     ___  | |__   | |__     ___   __  __
         *     | |      | | | | | '_ \   / __| | '_ \  | '_ \   / _ \  \ \/ /
         *     | |____  | |_| | | | | | | (__  | | | | | |_) | | (_) |  >  < 
         *     |______|  \__,_| |_| |_|  \___| |_| |_| |_.__/   \___/  /_/\_\                                                                                                
         */

        [Command("AddLunchbox"), Alias("AddLB", "addLB", "addlb", "addlunchbox", "addlunchBox", "addLunchBox", "Addlunchbox", "AddlunchBox", "AddLunchBox")]
        public async Task AddLunchboxEvent(int lunchboxDateYear, int lunchboxDateMonth, int lunchboxDateDay, string lunchboxTopic, string lunchboxSpeaker)
        {
            if (await IsUserAuthorized("Admin", "Teacher") == false)
            {
                return;
            }


            Lunchbox NewLunchbox = new Lunchbox()
            {
                date = new DateTime(lunchboxDateYear, lunchboxDateMonth, lunchboxDateDay, 14, 00, 00),
                topic = lunchboxTopic,
                speaker = lunchboxSpeaker,
                author = Context.User.Id,
                created = DateTime.UtcNow,
            };

            Program.BulletinBoardData.Lunchboxes.Add(NewLunchbox);
            Program.SaveBulletinBoardDataToFile();

            var builder = new EmbedBuilder()
                .WithTitle("New Lunchbox Added")
                .WithColor(new Color(37, 170, 225))
                .WithThumbnailUrl(Program.ServerConfigData.LunchboxIconURL)
                .AddField($"{lunchboxTopic}", $"{lunchboxSpeaker}\n{NewLunchbox.date.ToString("dddd, d MMMM yyyy")}");

            var embed = builder.Build();
            var msg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).SendMessageAsync(null, embed: embed).ConfigureAwait(false);

            await Context.Message.DeleteAsync();
            await UpdateLunchboxEvents();

            //await Task.Delay(3000);
            await msg.DeleteAsync();
        }

        [Command("UpdateLunchbox"), Alias("UpdateLB", "updateLB", "updatelb", "updatelunchbox", "updatelunchBox", "updateLunchBox", "Updatelunchbox", "UpdatelunchBox", "UpdateLunchBox")]
        public async Task UpdateLunchboxEvents()
        {
            Program.BulletinBoardData.Lunchboxes.Sort((a, b) => a.date.CompareTo(b.date));
            Program.SaveBulletinBoardDataToFile();


            var pastLuncboxBuilder = new EmbedBuilder()
            .WithDescription("```fix\nPast Lunchbox Events\n```")
            .WithColor(new Color(37, 170, 225))
            .WithThumbnailUrl(Program.ServerConfigData.LunchboxIconURL);

            var futureLuncboxBuilder = new EmbedBuilder()
            .WithDescription("```fix\nFuture Lunchbox Events\n```")
            .WithColor(new Color(37, 170, 225))
            .WithThumbnailUrl(Program.ServerConfigData.LunchboxIconURL);

            int EventSplitIdx = 0;

            //gets the index in which the event date is in the future
            for (int i = 0; i < Program.BulletinBoardData.Lunchboxes.Count(); i++)
            {
                if (Program.BulletinBoardData.Lunchboxes[i].date.CompareTo(DateTime.Now) < 0)
                {
                    EventSplitIdx++;
                }
            }

            //Past Embed
            for (int i = 0; i < Program.BulletinBoardData.PastLunchboxesEmbedCount; i++)
            {
                if (EventSplitIdx - Program.BulletinBoardData.PastLunchboxesEmbedCount + i < 0)
                {
                    continue;
                }

                Lunchbox lb = Program.BulletinBoardData.Lunchboxes[EventSplitIdx - Program.BulletinBoardData.PastLunchboxesEmbedCount + i];
                pastLuncboxBuilder.AddField($"{lb.topic}", $"{lb.speaker}\n{lb.date.ToString("d MMMM yyyy")}");

            }

            //Future Embed
            for (int i = 0; i < Program.BulletinBoardData.FutureLunchboxesEmbedCount; i++)
            {
                if (EventSplitIdx + i >= Program.BulletinBoardData.Lunchboxes.Count())
                {
                    break;
                }

                Lunchbox lb = Program.BulletinBoardData.Lunchboxes[EventSplitIdx + i];
                futureLuncboxBuilder.AddField($"{lb.topic}", $"{lb.speaker}\n{lb.date.ToString("d MMMM yyyy")}");
            }

            var embed = pastLuncboxBuilder.Build();


            //Updates Past Embed
            IMessage pastChatReferences = await Context.Channel.GetMessageAsync(Program.BulletinBoardData.PastLunchboxesMsgID, CacheMode.AllowDownload);

            if (pastChatReferences is IUserMessage pastMsg)
            {
                await pastMsg.ModifyAsync(x => x.Embed = embed);
            }

            embed = futureLuncboxBuilder.Build();
            //Updates Future Embed
            IMessage futureChatReferences = await Context.Channel.GetMessageAsync(Program.BulletinBoardData.FutureLunchboxesMsgID, CacheMode.AllowDownload);

            if (futureChatReferences is IUserMessage futureMsg)
            {
                await futureMsg.ModifyAsync(x => x.Embed = embed);
            }

            var botMsg = await Context.Channel.SendMessageAsync("Updated");

            await Task.Delay(5000);

            await botMsg.DeleteAsync();

            await Context.Message.DeleteAsync();
        }

        [Command("RemoveLunchbox"), Alias("RemoveLB", "removeLB", "removelb", "removelunchbox", "removelunchBox", "removeLunchBox", "Removelunchbox", "RemovelunchBox", "RemoveLunchBox")]
        public async Task RemoveLunchboxEvent(string topic)
        {
            if (await IsUserAuthorized("Admin", "Teacher") == false)
            {
                return;
            }

            foreach (Lunchbox lb in Program.BulletinBoardData.Lunchboxes)
            {
                if (lb.topic.ToLower() == topic.ToLower())
                {
                    Program.BulletinBoardData.Lunchboxes.Remove(lb);

                    var msgRemoved = await Context.Channel.SendMessageAsync("Lunchbox Event Removed");
                    await UpdateLunchboxEvents();
                    await Task.Delay(2000);
                    await msgRemoved.DeleteAsync();

                    return;
                }
            }

            var msgNope = await Context.Channel.SendMessageAsync($"Lunchbox topic\n> {topic}\nNot found");
            await Task.Delay(5000);
            await Context.Message.DeleteAsync();
            await msgNope.DeleteAsync();
        }





        /*      ____            _   _          _     _             ______                          _         
         *     |  _ \          | | | |        | |   (_)           |  ____|                        | |        
         *     | |_) |  _   _  | | | |   ___  | |_   _   _ __     | |__    __   __   ___   _ __   | |_   ___ 
         *     |  _ <  | | | | | | | |  / _ \ | __| | | | '_ \    |  __|   \ \ / /  / _ \ | '_ \  | __| / __|
         *     | |_) | | |_| | | | | | |  __/ | |_  | | | | | |   | |____   \ V /  |  __/ | | | | | |_  \__ \
         *     |____/   \__,_| |_| |_|  \___|  \__| |_| |_| |_|   |______|   \_/    \___| |_| |_|  \__| |___/
         */


        [Command("NewEvent"), Alias("newEvent", "Newevent", "newevent", "addEvent", "addevent", "addEvent")]
        public async Task NewBulletinEvent(int year, int month, int day, int hour, int min, string title)
        {
            if (await IsUserAuthorized("Admin", "Teacher", "Student") == false)
            {
                return;
            }

            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (bulletinEvent.author == Context.User.Id)
                {
                    if (bulletinEvent.Description == null)
                    {
                        await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\nPlease finish filling in the details of the event \"{bulletinEvent.Title}\" before creating a new one\nUse Command InfoEvent");
                        await Context.Message.DeleteAsync();
                        return;
                    }
                }

            }

            BulletinEvent NewBulletinEvent = new BulletinEvent()
            {
                Title = title,
                EventDate = new DateTime(year, month, day, hour, min, 00),
                authorIconURL = Context.User.GetAvatarUrl(),
                author = Context.User.Id,
                embedCreated = DateTime.Now
            };


            var builder = new EmbedBuilder()
                .WithTitle(NewBulletinEvent.Title)
                .WithUrl($"https://discordapp.com/invite/xQAcyyX")
                .WithColor(new Color(0, 0, 255))
                .WithDescription($"Missing")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/647733785118375947/NoPhoto.png")
                .AddField($"Time", NewBulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                .AddField($"Location", "Missing", true)
                .AddField($"Cost", "Missing", true)
                .AddField($"Capacity", "Missing", true)
                .AddField($"Attending", "0", true)
                .WithFooter($"By {Context.User.Username}", $"{NewBulletinEvent.authorIconURL}")
                .WithCurrentTimestamp();

            var embed = builder.Build();
            var msg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).SendMessageAsync(null, embed: embed).ConfigureAwait(true);

            //Saving data
            NewBulletinEvent.MsgID = msg.Id;
            Program.BulletinBoardData.BulletinEvents.Add(NewBulletinEvent);
            Program.SaveBulletinBoardDataToFile();
            await Context.Message.DeleteAsync();
        }

        [Command("InfoEvent"), Alias("infoEvent", "Infoevent", "infoevent")]
        public async Task AddInfoBulletinEvent(string description, string location, string cost, int capacity, string eventURL, string iconURL)
        {
            if (await IsUserAuthorized("Admin", "Teacher", "Student") == false)
            {
                return;
            }

            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (bulletinEvent.author == Context.User.Id)
                {
                    if (bulletinEvent.Description == null)
                    {
                        bulletinEvent.Description = description;
                        bulletinEvent.Location = location;
                        bulletinEvent.Cost = cost;
                        bulletinEvent.Capacity = capacity;
                        bulletinEvent.EventURL = eventURL;
                        bulletinEvent.IconURL = iconURL;

                        Program.SaveBulletinBoardDataToFile();


                        var builder = new EmbedBuilder()
                            .WithTitle(bulletinEvent.Title)
                            .WithUrl($"{bulletinEvent.EventURL}")
                            .WithColor(new Color(0, 0, 255))
                            .WithDescription($"{bulletinEvent.Description}")
                            .WithThumbnailUrl($"{bulletinEvent.IconURL}")
                            .AddField($"Time", bulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                            .AddField($"Location", $"{bulletinEvent.Location}", true)
                            .AddField($"Cost", $"{bulletinEvent.Cost}", true)
                            .AddField($"Capacity", $"{bulletinEvent.Capacity}", true)
                            .AddField($"Attending", $"{bulletinEvent.AttendingUsers.Count}", true)
                            .WithFooter($"By {Context.Guild.GetUser(bulletinEvent.author).Nickname}", $"{bulletinEvent.authorIconURL}")
                            .WithCurrentTimestamp();

                        var embed = builder.Build();

                        var embedEvent = await (Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]) as ISocketMessageChannel).GetMessageAsync(bulletinEvent.MsgID, CacheMode.AllowDownload);

                        if (embedEvent is IUserMessage embedMsg)
                        {
                            await embedMsg.ModifyAsync(x => x.Embed = embed);
                            await embedMsg.AddReactionAsync(Program.BulletinBoardData.BulletinAttendingEmote);

                        }

                        await Context.Message.DeleteAsync();

                        return;
                    }
                }
            }

            //No Event found
            await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\nNo Event Found\nUse Command InfoEvent");
            await Context.Message.DeleteAsync();
            return;
        }

        [Command("UpdateEvents"), Alias("updateEvents", "Updateevents", "updateevents")]
        public async Task UpdateBulletinEvents()
        {
            Program.BulletinBoardData.BulletinEvents.Sort((a, b) => a.EventDate.CompareTo(b.EventDate));
            Program.SaveBulletinBoardDataToFile();

            int embedCount = 0;

            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents) //Delete all embeds
            {
                if (bulletinEvent.MsgID != 1234567890)
                {
                    var eventEmbed = await (Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]) as ISocketMessageChannel).GetMessageAsync(bulletinEvent.MsgID, CacheMode.AllowDownload);

                    if (eventEmbed is IUserMessage embedMsg)
                    {

                        //Updating attending users
                        foreach (IUser attendee in await (eventEmbed as IUserMessage).GetReactionUsersAsync(Program.BulletinBoardData.BulletinAttendingEmote, 100).FlattenAsync())
                        {
                            if (bulletinEvent.AttendingUsers.Contains(attendee.Id) == false && attendee.Id != Program.ServerConfigData.PointersAnonUserID["Quantum Bot"])
                            {
                                bulletinEvent.AttendingUsers.Add(attendee.Id);
                            }
                        }

                        await embedMsg.DeleteAsync();
                        bulletinEvent.MsgID = 1234567890;
                        Program.SaveBulletinBoardDataToFile();
                        await Task.Delay(1000);
                    }
                }
            }

            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents) //Remake Embeds
            {
                if (bulletinEvent.EventDate.CompareTo(DateTime.Now) > 0 && embedCount < Program.BulletinBoardData.BulletinEventMax)
                {
                    var builder = new EmbedBuilder()
                        .WithTitle(bulletinEvent.Title)
                        .WithUrl($"{bulletinEvent.EventURL}")
                        .WithColor(new Color(0, 0, 255))
                        .WithDescription($"{bulletinEvent.Description}")
                        .WithThumbnailUrl($"{bulletinEvent.IconURL}")
                        .AddField($"Time", bulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                        .AddField($"Location", $"{bulletinEvent.Location}", true)
                        .AddField($"Cost", $"{bulletinEvent.Cost}", true)
                        .AddField($"Capacity", $"{bulletinEvent.Capacity}", true)
                        .AddField($"Attending", $"{bulletinEvent.AttendingUsers.Count}", true)
                        .WithFooter($"By {Context.Guild.GetUser(bulletinEvent.author).Nickname}", $"{bulletinEvent.authorIconURL}")
                        .WithTimestamp(bulletinEvent.embedCreated);

                    var embed = builder.Build();

                    var msg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).SendMessageAsync(null, embed: embed).ConfigureAwait(true);

                    //add atending emote
                    await msg.AddReactionAsync(Program.BulletinBoardData.BulletinAttendingEmote);

                    //Saving data
                    bulletinEvent.MsgID = msg.Id;
                    Program.SaveBulletinBoardDataToFile();

                    embedCount++;
                    await Task.Delay(1000);
                }
            }

            await Context.Message.DeleteAsync();

        }

        [Command("RemoveEvent"), Alias("removeEvent", "RemoveEvent", "removeevent")]
        public async Task RemoveBulletinEvent(string topic)
        {
            if (await IsUserAuthorized("Admin", "Teacher", "Student") == false)
            {
                return;
            }

            foreach (BulletinEvent bulletinEvent in Program.BulletinBoardData.BulletinEvents)
            {
                if (bulletinEvent.Title.ToLower() == topic.ToLower() && ( Context.User.Id == bulletinEvent.author || await IsUserAuthorized("Admin", "Teacher")) )
                {
                    if (bulletinEvent.MsgID != 1234567890)
                    {
                        var eventEmbed = await (Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]) as ISocketMessageChannel).GetMessageAsync(bulletinEvent.MsgID, CacheMode.AllowDownload);

                        if (eventEmbed is IUserMessage embedMsg)
                        {
                            await embedMsg.DeleteAsync();
                            Program.SaveBulletinBoardDataToFile();
                            await Task.Delay(1000);
                        }
                    }

                    Program.BulletinBoardData.BulletinEvents.Remove(bulletinEvent);

                    var msgRemoved = await Context.Channel.SendMessageAsync("Lunchbox Event Removed");
                    await UpdateBulletinEvents();
                    await Task.Delay(2000);
                    await msgRemoved.DeleteAsync();

                    return;
                }
            }

            var msgNope = await Context.Channel.SendMessageAsync($"Event title\n> {topic}\nNot found, or you are not the author");
            await Task.Delay(5000);
            await msgNope.DeleteAsync();
            await Context.Message.DeleteAsync();
        }



        /*                     _               _         
         *         /\         | |             (_)        
         *        /  \      __| |  _ __ ___    _   _ __  
         *       / /\ \    / _` | | '_ ` _ \  | | | '_ \ 
         *      / ____ \  | (_| | | | | | | | | | | | | |
         *     /_/    \_\  \__,_| |_| |_| |_| |_| |_| |_|
         *                                               
         */

        [Command("Quit"), Alias("quit"), Summary("Quits the bot exe, only Admins an run")]

        public async Task Quit()
        {
            if (await IsUserAuthorized("Admin", "Teacher"))
            {
                var msg = await Context.Message.Channel.SendMessageAsync("I'll be back - Gandhi\nhttps://media.giphy.com/media/gFwZfXIqD0eNW/giphy.gif");
                await Task.Delay(5000);
                await Context.Message.DeleteAsync();
                await msg.DeleteAsync();
                System.Environment.Exit(1);
            }
            else
            {
                await Context.Message.DeleteAsync();
                await Context.User.SendMessageAsync("Admin Rights Required");
            }
        }

        [Command("SendIntro"), Alias("sendintro", "sendIntro", "Sendintro")]
        public async Task SendIntro(ulong targetUser)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            if (Context.Guild.GetUser(targetUser) == null)
            {
                await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bot History"]).SendMessageAsync("Invalid User ID");
            }
            else
            {
                await Program.SendIntroductionMessage(Context.Guild.GetUser(targetUser));
            }
        }

        [Command("SendIntro"), Alias("sendintro", "sendIntro", "Sendintro")]
        public async Task SendIntro(params string[] test)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            foreach (var targetUser in Context.Message.MentionedUsers)
            {
                await Program.SendIntroductionMessage(targetUser as SocketGuildUser);
            }
        }

        [Command("SeedEmbed"), Alias("Seedembed", "seedEmbed", "seedembed")]
        public async Task SeedEmbed()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            var builder = new EmbedBuilder()
                        .WithTitle("Seed");

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);

            await Context.Message.DeleteAsync();
        }

        
        [Command("ServerStats")]
        public async Task GetServerStats()
        { 
        
        
        }


        [Command("UpdateUserList")]
        public async Task UpdateUserList()
        {
            if(await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            var AllUsers = Context.Guild.Users;

            bool yeet = false;

            foreach (SocketGuildUser users in AllUsers)
            {
                foreach (UserProfile person in Program.UserData)
                {
                    if (users.Id == person.userID)
                    {
                        //yeet = true;
                        break;
                    }
                }
                
                if (yeet == false)
                {
                    UserProfile user = GetUserProfile(users.Id);

                    user.userNickname = users.Nickname;


                    if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Guest"])) == true)
                    {
                        user.isGuest = true;                   
                    }
                    else if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Teacher"])) == true)
                    {
                        user.isTeacher = true;
                    }
                    else if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Student"])) == true)
                    {
                        user.isStudent = true;

                        if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2020"])) == true)
                        {
                            user.GradYear = 2020;
                        }
                        else if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2021"])) == true)
                        {
                            user.GradYear = 2021;
                        }
                        else if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2019"])) == true)
                        {
                            user.GradYear = 2019;                            
                        }
                        else
                        {
                            user.GradYear = 6969;
                        }
                    }

                    Program.SaveUserDataToFile();
                }
                yeet = false;
            }
        }

        [Command("GetUserAnomalies")]
        public async Task GetUserAnomalies()
        {
            
            Program.UserData.Sort((b, a) => a.isStudent.CompareTo(b.isStudent));

            string reportMsg = "";

            foreach (UserProfile user in Program.UserData)
            { 
                if(user.userNickname == null)
                {
                    reportMsg += $"<@{user.userID}> : Missing Nickname :no_entry:\n";
                }

                if (user.isGuest == false && user.isTeacher == false && user.isStudent == false)
                { 
                    reportMsg += $"<@{user.userID}> : Not a Student,Teacher,or Guest :no_entry:\n";
                }
            }

            await Context.Channel.SendMessageAsync(reportMsg);
        }

        /*      __  __          _     _                   _       
         *     |  \/  |        | |   | |                 | |      
         *     | \  / |   ___  | |_  | |__     ___     __| |  ___ 
         *     | |\/| |  / _ \ | __| | '_ \   / _ \   / _` | / __|
         *     | |  | | |  __/ | |_  | | | | | (_) | | (_| | \__ \
         *     |_|  |_|  \___|  \__| |_| |_|  \___/   \__,_| |___/
         */

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

        private async Task<bool> IsUserAuthorized(params string[] roles)
        {
            var adminCheck = Context.User as SocketGuildUser;

            foreach (string role in roles)
            {
                var AuthRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID[role]);
               
                if (adminCheck.Roles.Contains(AuthRole) == true)
                {
                    return true;
                }
            }

            await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
               $"You do not have permission to use this command");
            await Context.Message.DeleteAsync();
            return false;

        }

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
            foreach (KeyValuePair<Program.WEBSITES, WebsiteProfile> web in Program.ServerConfigData.WebsiteData)
            {
                newProfile.UserWebsiteIndex.Add(web.Key, null);
            }

            Program.UserData.Add(newProfile);
            return newProfile;
        }

        private List<Embed> CreateWebsiteEmbeds()
        {
            List<Embed> newWebsiteEmbedList = new List<Embed>();

            foreach (KeyValuePair<Program.WEBSITES, WebsiteProfile> web in Program.ServerConfigData.WebsiteData)
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
               .WithColor(Program.ServerConfigData.WebsiteData[web].WebsiteColor)
               .WithTimestamp(DateTimeOffset.Now)
               .WithFooter(footer =>
               {
                   footer
                    .WithText("Last Updated");
               })
               .WithThumbnailUrl(Program.ServerConfigData.WebsiteData[web].WebsiteIconURL);


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
