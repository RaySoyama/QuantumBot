using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class QuantumBotCommands : ModuleBase<SocketCommandContext>
    {
        #region General 

        [Command("Help"), Alias("help"), Summary("List of all commands")]
        public async Task HelpList()
        {
            var builder = new EmbedBuilder()
                          .WithTitle("Quantum Bot - Commands")
                          .WithDescription($"Ping one of the moderators, or <@{Program.ServerConfigData.PointersAnonUserID["Ray Soyama"]}> if you have any questions!\n" +
                                            $"Current Prefix is \"{Program.ServerConfigData.prefix}\"\n" +
                                            $"Note: {{Parameters}} are wrapped with curly brackets\n" +
                                            $"Sentences should be wrapped in \"Quotation Marks\"")
                          .AddField("General",
                                    $"Ping - returns current server latency\n" +
                                    $"VaultSeeker - Gives you access to the vault\n" +
                                    $"Suggestion {{your suggestion}} - Makes a suggestion post in  <#{Program.ServerConfigData.PointersAnonChatID["Suggestions"]}>\n" +
                                    $"GetServerStats - Returns data on the server members\n" +
                                    $"Crash - Adds to the \"Unity Crash Counter\"\n" +
                                    $"CrashCounter - Returns the \"Unity Crash Counter\"\n" +
                                    $"React {{Shorthand}} - Adds a animated emote to the previous msg\n" +
                                    $"ReactList - Prints the list of animated gif shorthands")
                          .AddField("School",
                                    $"UnityVersion - Returns the version of Unity AIE is currently using\n" +
                                    $"ProposalTemplate - Returns the download link to the doc\n" +
                                    $"AIESchedule - Returns the download link to the doc\n" +
                                    $"Graduation - Returns some graduation stats\n" +
                                    $"Website {{Domain}} {{URL}} - Adds/Updates link in <#{Program.ServerConfigData.PointersAnonChatID["Personal Links"]}>\n" +
                                    $"AdminWebsite {{userID}} {{Domain}} {{URL}} - Admins Add/Update links in <#{Program.ServerConfigData.PointersAnonChatID["Personal Links"]}>")
                           .AddField("Bulletin Board",
                                     $"Lunchbox - Returns the next lunchbox event\n" +
                                     $"AddLunchbox {{Year}} {{Month}} {{Day}} {{Topic}} {{Speaker}} - Adds a lunchbox event\n" +
                                     $"RemoveLunchbox {{Topic}} - Removes a lunchbox event with the same topic\n" +
                                     $"UpdateLunchbox - Updates the Lunchbox embeds\n" +
                                     $"NewEvent {{Year}} {{Month}} {{Day}} {{Hour}} {{Minute}} {{Title}} - Create a new bulletin event\n" +
                                     $"InfoEvent {{Description}} {{Location}} {{Cost}} {{Capacity}} {{EventURL}} {{IconURL}} - Add information to the bulletin Event\n" +
                                     $"RemoveEvent {{Topic}} - Removes a bulletin event with the same topic\n" +
                                     $"UpdateEvents - Updates the bulletin embeds")
                            .AddField("Game Codes",
                                     $"GameCode - Returns User Game Codes\n" +
                                     $"AddGameCode {{\"Platform\"}} {{\"Code\"}} - Adds a Game Code to the users profile\n" +
                                     $"RemoveGameCode {{\"Platform\"}} - Remove a Game Code\n")
                            .AddField("Monster Hunter World",
                                    $"Adopt {{\"Monster Name\"}} {{\"Nickname\"}} - Give a Monster a nick name\n" +
                                    $"ViewNames - Returns the list of all MHW names\n" +
                                    $"ViewNames {{Monster Name}} - Returns the nicknames given to the Monster\n" +
                                    $"RemoveAdopt {{Monster Name}}  {{Nickname}} - Removes a nickname from the Monster\n" +
                                    $"MonsterStats - Returns stats about the adopt system\n" +
                                    $"Alatreon/Fatalis - Adds one to the Alatreon/Fatalis cart counter\n" +
                                    $"AlatreonCount/FatalisCount - Returns the current Alatreon/Fatalis cart counter\n" +
                                    $"\n" +
                                    $"\n")
                            .AddField("Accountabilibuddy",
                                    $"StartDay {{\"Work Duration in Min\"}} {{\"Break Duration in Min\"}} - Starts a looping timer that pings you when you should take breaks\n" +
                                    $"EndDay - Ends the timer")
                            .AddField("Admin",
                                     $"SendIntro {{@User}} - Sends the user the introduction msg\n" +
                                     $"UpdateUserList - Logs server members\n" +
                                     $"GetUserAnomalies - Returns server members without roles/nicknames\n" +
                                     $"Purge {{count}} - Mass deletes messages in a channel\n" +
                                     $"Quit - Bot commits Seppuku")
                          .WithColor(new Color(60, 179, 113));

            var embed = builder.Build();
            await Context.User.SendMessageAsync("", embed: embed);
            await Context.Message.DeleteAsync();


            return;
        }

        [Command("Ping"), Alias("ping"), Summary("Returns the latency")]
        public async Task Ping()
        {
            var msg = await Context.Message.Channel.SendMessageAsync($"MS {Program.latency}");

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

        [Command("Suggestion"), Alias("suggestion", "suggest", "Suggest")]

        public async Task HelpSuggestion()
        {
            await Context.Channel.SendMessageAsync($"Command Help\n{Program.ServerConfigData.prefix}Suggestion" + " {type your suggestion here}");
        }
        [Command("ServerStats")]
        public async Task GetServerStats()
        {
            int ProgrammerCount = 0;
            int ArtistCount = 0;
            int DesignerCount = 0;
            int VFXCount = 0;
            int StudentCount = 0;
            int TeacherCount = 0;
            int IndustryCount = 0;
            int GuestCount = 0;
            int BotCount = 0;
            int CO2019 = 0;
            int CO2020 = 0;
            int CO2021 = 0;
            int CO2022 = 0;

            var AllUsers = Context.Guild.Users;

            var ProgramerRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Programming"]);
            var ArtistRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Art"]);
            var DesignerRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Design"]);
            var VFXRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["VFX"]);
            var StudentRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Student"]);
            var TeacherRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Teacher"]);
            var IndustryRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Industry"]);
            var GuestRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Guest"]);
            var CO2019Role = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2019"]);
            var CO2020Role = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2020"]);
            var CO2021Role = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2021"]);
            var CO2022Role = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2022"]);


            foreach (SocketGuildUser users in AllUsers)
            {
                if (users.Roles.Contains(ProgramerRole) == true)
                {
                    ProgrammerCount++;
                }

                if (users.Roles.Contains(ArtistRole) == true)
                {
                    ArtistCount++;
                }

                if (users.Roles.Contains(DesignerRole) == true)
                {
                    DesignerCount++;
                }

                if (users.Roles.Contains(VFXRole) == true)
                {
                    VFXCount++;
                }

                if (users.Roles.Contains(StudentRole) == true)
                {
                    StudentCount++;
                }

                if (users.Roles.Contains(TeacherRole) == true)
                {
                    TeacherCount++;
                }

                if (users.Roles.Contains(IndustryRole) == true)
                {
                    IndustryCount++;
                }

                if (users.Roles.Contains(GuestRole) == true)
                {
                    GuestCount++;
                }

                if (users.IsBot == true)
                {
                    BotCount++;
                }

                if (users.Roles.Contains(CO2019Role) == true)
                {
                    CO2019++;
                }

                if (users.Roles.Contains(CO2020Role) == true)
                {
                    CO2020++;
                }

                if (users.Roles.Contains(CO2021Role) == true)
                {
                    CO2021++;
                }

                if (users.Roles.Contains(CO2022Role) == true)
                {
                    CO2022++;
                }
            }


            string StatsMsg = "❤️ Pointers Anonymous Stats\n" +
                              "```autohotkey\n" +
                              $"Programmers     :{ServerStatBoxPrinter(ProgrammerCount)}\n" +
                              $"Artists         :{ServerStatBoxPrinter(ArtistCount)}\n" +
                              $"Designers       :{ServerStatBoxPrinter(DesignerCount)}\n" +
                              $"VFX             :{ServerStatBoxPrinter(VFXCount)}\n" +
                              $"\n" +
                              $"Students        :{ServerStatBoxPrinter(StudentCount)}\n" +
                              $"Teachers        :{ServerStatBoxPrinter(TeacherCount)}\n" +
                              $"Industry        :{ServerStatBoxPrinter(IndustryCount)}\n" +
                              $"Guests          :{ServerStatBoxPrinter(GuestCount)}\n" +
                              $"Bots            :{ServerStatBoxPrinter(BotCount)}\n" +
                              $"\n" +
                              $"\n" +
                              $"Member Count    : {AllUsers.Count}\n" +
                              $"Class of 2019   :{ServerStatBoxPrinter(CO2019)}\n" +
                              $"Class of 2020   :{ServerStatBoxPrinter(CO2020)}\n" +
                              $"Class of 2021   :{ServerStatBoxPrinter(CO2021)}\n" +
                              $"Class of 2022   :{ServerStatBoxPrinter(CO2022)}\n" +
                              $"\n" +
                              $"Stats from      : {DateTime.Now.ToString("MM/d/yyyy H:mm")}\n" +
                              "```";
            await Context.Channel.SendMessageAsync(StatsMsg);
        }

        //[Command("FormatThis"), Alias("format")]
        //public async Task FormatThisPlz()
        //{
        //    string path = System.IO.Directory.GetParent(System.IO.Path.GetFullPath("FormatThis.png")).ToString();

        //    //path = System.IO.Directory.GetParent(path).ToString();
        //    //path = System.IO.Directory.GetParent(path).ToString();
        //    //path = System.IO.Directory.GetParent(path).ToString();

        //    Directory.CreateDirectory(path + "\\Pointers Anonymous Bot Files\\");

        //    path += "\\Pointers Anonymous Bot Files\\" + "FormatThis.png";

        //    await Context.Channel.SendFileAsync(path, "https://gist.github.com/Almeeida/41a664d8d5f3a8855591c2f1e0e07b19", false,null,null,false);
        //}

        //--------------------------------------------------------------------------------

        [Command("Crash"), Alias("UnityCrash", "CrashUnity")]
        public async Task UnityCrashCounter()
        {
            Program.ServerConfigData.UnityCrashCount += 1;
            Program.SaveServerDataToFile();

            await Context.Channel.SendMessageAsync($"Current Unity Crash count: {Program.ServerConfigData.UnityCrashCount}");
        }

        [Command("Crash"), Alias("UnityCrash", "CrashUnity")]
        public async Task UnityCrashCounter(int unit)
        {
            Program.ServerConfigData.UnityCrashCount += unit;
            Program.SaveServerDataToFile();

            await Context.Channel.SendMessageAsync($"Current Unity Crash count: {Program.ServerConfigData.UnityCrashCount}");
        }

        [Command("CrashCount"), Alias("UnityCrashCount")]
        public async Task UnityCrashCountDisplayer()
        {
            await Context.Channel.SendMessageAsync($"Current Unity Crash count: {Program.ServerConfigData.UnityCrashCount}");
        }

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

        [Command("AIESchedule"), Alias("AIECalender", "Calender", "Schedule")]
        public async Task SendAIECalender()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Here you go~ \n{Program.ServerConfigData.AIESchoolCalender}");
            return;
        }

        [Command("Graduation"), Alias("Grad")]
        public async Task DaysTillGraduation()
        {
            var user = Context.User as SocketGuildUser;
            string output = "";

            if (user.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2020"])) == true)
            {
                output += GraduationDialoguePrinter(2020);
            }
            else if (user.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2021"])) == true)
            {
                output += GraduationDialoguePrinter(2021);
            }
            else if (user.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2022"])) == true)
            {
                output += GraduationDialoguePrinter(2022);
            }
            else
            {
                output += GraduationDialoguePrinter(42069);
            }

            await Context.Channel.SendMessageAsync(output);
        }

        [Command("Graduation"), Alias("Grad")]
        public async Task DaysTillAllGraduation(string year)
        {
            string output = "";
            int yearNum = 42069;

            if (year.ToLower().Equals("all"))
            {
                yearNum = 42069;
            }
            else
            {
                try
                {
                    yearNum = Int32.Parse(year);
                }
                catch (FormatException)
                {
                    yearNum = 42069;
                }
            }

            output += GraduationDialoguePrinter(yearNum);

            await Context.Channel.SendMessageAsync(output);
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

            DateTime endTime = new DateTime(2020, 01, 09, 09, 0, 0);
            TimeSpan ts = endTime.Subtract(DateTime.Now);
            string daysTillDescription = ts.ToString("d' Days 'h' Hours 'm' Minutes 's' Seconds'");
            daysTillDescription += ($"\n\n[Releases](https://store.steampowered.com/app/1118010/Monster_Hunter_World_Iceborne/) at 9:00 AM PST on January 9th 2020");

            if (ts <= TimeSpan.Zero)
            {
                daysTillDescription = ":confetti_ball: :confetti_ball: ICEBORNE IS OUT YALL, HAVE AT IT HUNTERS:confetti_ball: :confetti_ball: \n\n[Released](https://store.steampowered.com/app/1118010/Monster_Hunter_World_Iceborne/) at 9:00 AM PST on January 9th 2020";
            }

            var builder = new EmbedBuilder()
                .WithColor(new Color(37, 170, 225))
                .WithTitle("PC Monster Hunter World Iceborne Countdown")
                .WithUrl("https://www.monsterhunter.com/update/mhw-steam/us/")
                .WithDescription($"{daysTillDescription}")//\n\n[Releases](https://store.steampowered.com/app/1118010/Monster_Hunter_World_Iceborne/) at 9:00 AM PST on January 9th 2020")
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/637797971752386560/DuEMx03WsAE1zhp.png")
                .AddField("Download Size", "48GB (+ 45GB for HighResTex)");


            await Context.Message.DeleteAsync();
            var embed = builder.Build();
            await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Monster Hunter"]).SendMessageAsync(null, embed: embed).ConfigureAwait(false);
            //await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
        }

        [Command("Capture"), Alias("Adopt")]
        public async Task GiveNewNicknameToMonster(string monsterName, [Remainder] string monsterNickname)
        {
            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterName.ToLower() == monsterName.ToLower()) //Get Monster Name
                {
                    foreach (MonsterHunterNicknames.NicknameData nicknameData in monsterData.nicknameData)
                    {
                        if (nicknameData.nickname.ToLower() == monsterNickname.ToLower())
                        {
                            var botMsg1 = await Context.Channel.SendMessageAsync($"Nickname {monsterNickname} already exists");
                            await Task.Delay(5000);
                            await botMsg1.DeleteAsync();
                            return;
                        }
                    }

                    monsterData.nicknameData.Add(new MonsterHunterNicknames.NicknameData() { creatorID = Context.User.Id, nickname = monsterNickname });
                    Program.SaveMonsterHunterDataToFile();
                    try
                    {
                        await Context.Channel.SendMessageAsync(null, embed: CreateMonsterNicknameEmbed(monsterData)).ConfigureAwait(false);
                    }
                    catch (Discord.Net.HttpException) // Catch Invalid URL
                    {
                        monsterData.monsterIconURL = "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png";
                        Program.SaveMonsterHunterDataToFile();

                        await Context.Channel.SendMessageAsync(null, embed: CreateMonsterNicknameEmbed(monsterData)).ConfigureAwait(false);
                    }

                    await Context.Message.DeleteAsync();
                    return;
                }
            }

            var botMsg = await Context.Channel.SendMessageAsync($"Monster with name {monsterName} not found");
            await Task.Delay(5000);
            await botMsg.DeleteAsync();
            return;
        }

        [Command("RemoveAdopt"), Alias("removeNickname", "removeName", "unAdopt")]
        public async Task RemoveMonsterNickname(string monsterName, [Remainder] string monsterNickname)
        {
            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterName.ToLower() == monsterName.ToLower()) //Get Monster Name
                {
                    foreach (MonsterHunterNicknames.NicknameData nicknameData in monsterData.nicknameData)
                    {
                        if (nicknameData.nickname.ToLower() == monsterNickname.ToLower())
                        {
                            monsterData.nicknameData.Remove(nicknameData);
                            Program.SaveMonsterHunterDataToFile();

                            var botMsg2 = await Context.Channel.SendMessageAsync($"Nickname {nicknameData.nickname} removed from {monsterData.monsterName}");
                            await Context.Message.DeleteAsync();
                            await Task.Delay(5000);
                            await botMsg2.DeleteAsync();
                            return;
                        }
                    }

                    var botMsg1 = await Context.Channel.SendMessageAsync($"Nickname {monsterNickname} doesn't exists");
                    await Task.Delay(5000);
                    await botMsg1.DeleteAsync();
                    return;

                }
            }

            var botMsg = await Context.Channel.SendMessageAsync($"Monster with name {monsterName} not found");
            await Task.Delay(5000);
            await botMsg.DeleteAsync();
            return;
        }

        [Command("ViewNames"), Alias("ViewName")]
        public async Task DisplayMonsterNicknames([Remainder] string monsterName)
        {
            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterName.ToLower() == monsterName.ToLower()) //Get Monster Name
                {
                    try
                    {
                        await Context.Channel.SendMessageAsync(null, embed: CreateMonsterNicknameEmbed(monsterData)).ConfigureAwait(false);
                    }
                    catch (Discord.Net.HttpException) // Catch Invalid URL
                    {
                        monsterData.monsterIconURL = "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png";
                        Program.SaveMonsterHunterDataToFile();

                        await Context.Channel.SendMessageAsync(null, embed: CreateMonsterNicknameEmbed(monsterData)).ConfigureAwait(false);
                    }

                    await Context.Message.DeleteAsync();
                    return;
                }
            }

            var botMsg = await Context.Channel.SendMessageAsync($"Monster with name {monsterName} not found");
            await Task.Delay(5000);
            await botMsg.DeleteAsync();
            return;
        }

        [Command("ViewNames"), Alias("ViewName")]
        public async Task DisplayMonsterNicknames()
        {
            string msg = "Monster Hunter World Monsters\n**A**\n";
            char startIdx = 'A';

            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterName[0] == startIdx)
                {
                    msg += $"{monsterData.monsterName}, ";
                }
                else
                {
                    startIdx = monsterData.monsterName[0];
                    msg += $"\n**{startIdx}**\n";
                    msg += $"{monsterData.monsterName}, ";
                }
            }
            await Context.Channel.SendMessageAsync(msg);
        }

        /*
        [Command("MonsterIcon")]
        public async Task AssignMonsterIcon(string monsterName, [Remainder] string monsterIconURL)
        {
            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterName.ToLower() == monsterName.ToLower())
                {
                    monsterData.monsterIconURL = monsterIconURL;
                    Program.SaveMonsterHunterDataToFile();

                    try
                    {
                        await Context.Channel.SendMessageAsync(null, embed: CreateMonsterNicknameEmbed(monsterData)).ConfigureAwait(false);
                    }
                    catch (Discord.Net.HttpException) // Catch Invalid URL
                    {
                        monsterData.monsterIconURL = "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png";
                        Program.SaveMonsterHunterDataToFile();

                        await Context.Channel.SendMessageAsync($"{monsterIconURL} is a invalid URL");
                    }
                    await Context.Message.DeleteAsync();
                    return;
                }
            }
            var botMsg = await Context.Channel.SendMessageAsync($"Monster with name {monsterName} not found");
            await Task.Delay(5000);
            await botMsg.DeleteAsync();
            return;
        }
        */
        [Command("MonsterStats")]
        public async Task MonsterStats()
        {
            Dictionary<ulong, int> nicknameCount = new Dictionary<ulong, int>();

            string msg = "";

            foreach (MonsterHunterNicknames monsterData in Program.MonsterHunterData)
            {
                if (monsterData.monsterIconURL == "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png")
                {
                    msg += $"{monsterData.monsterName} :no_entry:\n";
                }

                foreach (MonsterHunterNicknames.NicknameData nicknameData in monsterData.nicknameData)
                {
                    if (nicknameCount.ContainsKey(nicknameData.creatorID) == true)
                    {
                        nicknameCount[nicknameData.creatorID] += 1;
                    }
                    else
                    {
                        nicknameCount.Add(nicknameData.creatorID, 1);
                    }
                }
            }

            msg += "\n\n";

            foreach (var nickCount in nicknameCount)
            {
                msg += $"{Context.Guild.GetUser(nickCount.Key).Nickname} made {nickCount.Value} nicknames!\n";
            }

            if (msg.Count() > 1000)
            {
                for (int i = 0; i < msg.Count(); i += 1000)
                {
                    if (i + 1000 > msg.Count())
                    {
                        await Context.Channel.SendMessageAsync(msg.Substring(i, msg.Count() - i));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(msg.Substring(i, 1000));
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync(msg);
            }
            await Context.Message.DeleteAsync();
        }

        [Command("Alatreon")]
        public async Task AlatreonFailCounter()
        {
            Program.ServerConfigData.AlatreonFailCount += 1;
            Program.SaveServerDataToFile();

            await Context.Channel.SendMessageAsync($"Current Alatreon Fail Count: {Program.ServerConfigData.AlatreonFailCount}");
        }

        [Command("AlatreonCount"), Alias("AlatreonFailCount")]
        public async Task AlatreonDeathCountDisplayer()
        {
            await Context.Channel.SendMessageAsync($"Current Alatreon Fail Count: {Program.ServerConfigData.AlatreonFailCount}");
        }

        [Command("Fatalis")]
        public async Task FatalisFailCounter()
        {
            Program.ServerConfigData.FatalisFailCount += 1;
            Program.SaveServerDataToFile();

            await Context.Channel.SendMessageAsync($"Current Fatalis Fail Count: {Program.ServerConfigData.FatalisFailCount}");
        }

        [Command("FatalisCount"), Alias("FatalisFailCount")]
        public async Task FatalisDeathCountDisplayer()
        {
            await Context.Channel.SendMessageAsync($"Current Fatalis Fail Count: {Program.ServerConfigData.FatalisFailCount}");
        }

        [Command("AddGameCode"), Alias("SetGameCode", "AddGameCodes"), Summary("Add a Game code")]
        public async Task AddGameCode(string Platform, [Remainder] string platformCode)
        {
            foreach (UserProfile profile in Program.UserData)
            {
                if (profile.userID == Context.User.Id)
                {
                    if (profile.UserGameCodeIndex == null)
                    {
                        profile.UserGameCodeIndex = new Dictionary<string, string>();
                    }

                    if (profile.UserGameCodeIndex.ContainsKey(Platform))
                    {
                        profile.UserGameCodeIndex[Platform] = platformCode;
                        await Context.Message.DeleteAsync();
                        var botMsg = await Context.Channel.SendMessageAsync($"Updated game code for {Platform}");
                        await Task.Delay(5000);
                        await botMsg.DeleteAsync();
                    }
                    else
                    {
                        profile.UserGameCodeIndex.TryAdd(Platform, platformCode);
                        await Context.Message.DeleteAsync();
                        var botMsg = await Context.Channel.SendMessageAsync($"Added game code for {Platform}");
                        await Task.Delay(5000);
                        await botMsg.DeleteAsync();
                    }
                    Program.SaveUserDataToFile();
                    return;
                }
            }

            UserProfile user = new UserProfile();

            user.userID = Context.User.Id;
            user.userNickname = ((IGuildUser)Context.User).Nickname;

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
            Program.UserData.Add(user);

            user.UserGameCodeIndex = new Dictionary<string, string>();
            user.UserGameCodeIndex.Add(Platform, platformCode);

            await Context.Message.DeleteAsync();
            var botMsg1 = await Context.Channel.SendMessageAsync($"Added game code for {Platform}");
            await Task.Delay(5000);
            await botMsg1.DeleteAsync();

            Program.SaveUserDataToFile();
            return;
        }

        [Command("RemoveGameCode"), Alias("DeleteGameCode"), Summary("Delete a Game code")]
        public async Task RemoveGameCode(string Platform)
        {
            foreach (UserProfile profile in Program.UserData)
            {
                if (profile.userID == Context.User.Id)
                {
                    if (profile.UserGameCodeIndex == null)
                    {
                        profile.UserGameCodeIndex = new Dictionary<string, string>();
                    }

                    if (profile.UserGameCodeIndex.ContainsKey(Platform))
                    {
                        profile.UserGameCodeIndex.Remove(Platform);
                        await Context.Message.DeleteAsync();
                        var botMsg = await Context.Channel.SendMessageAsync($"Removed game code for {Platform}");
                        await Task.Delay(5000);
                        await botMsg.DeleteAsync();

                        Program.SaveUserDataToFile();
                        return;
                    }
                    break;
                }
            }

            await Context.Message.DeleteAsync();
            var botMsg1 = await Context.Channel.SendMessageAsync($"No game code found for {Platform}");
            await Task.Delay(5000);
            await botMsg1.DeleteAsync();
            return;
        }

        [Command("GameCode"), Alias("GetGameCode", "ShowGameCode", "ViewGameCode", "GameCodes"), Summary("Show a users Game code")]
        public async Task ViewGameCode()
        {
            await Context.Message.DeleteAsync();

            foreach (UserProfile profile in Program.UserData)
            {
                if (profile.userID == Context.User.Id)
                {
                    if (profile.UserGameCodeIndex == null)
                    {
                        profile.UserGameCodeIndex = new Dictionary<string, string>();
                    }

                    var builder = new EmbedBuilder()
                                        .WithTitle($"Game Codes")
                                        .WithColor(new Color(0x259FEE))
                                        .WithThumbnailUrl($"{Context.User.GetAvatarUrl()}")
                                        .WithAuthor(author =>
                                                    {
                                                        author
                                                       .WithName($"{((IGuildUser)Context.User).Nickname}");
                                                    });

                    if (profile.UserGameCodeIndex == null || profile.UserGameCodeIndex.Count == 0)
                    {
                        builder.AddField($"No Game Codes Entries", $":(");
                    }
                    else
                    {
                        foreach (KeyValuePair<string, string> gameCode in profile.UserGameCodeIndex)
                        {
                            builder.AddField($"{gameCode.Key}", $"{gameCode.Value}", false);
                        }

                    }

                    var embed = builder.Build();

                    await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);
                    return;
                }
            }

            var botMsg1 = await Context.Channel.SendMessageAsync("No User Data found");
            await botMsg1.DeleteAsync();
            return;
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

        [Command("React"), Alias("Reaction")]
        public async Task ReactEmote(string shorthand)
        {
            foreach (ReactEmote reactEmote in Program.ReactEmoteData)
            {
                if (reactEmote.emoteShortHand.ToLower() == shorthand.ToLower())
                {
                    IEmote reaction = null;
                    reaction = reactEmote.ChannelReactEmote;

                    //get previous msg
                    var messages = await Context.Channel.GetMessagesAsync(2).FlattenAsync();

                    //Potential issues when not enough msg's
                    await Context.Message.DeleteAsync();

                    await (messages.ElementAt(1) as IUserMessage).AddReactionAsync(reaction, null);
                    //await Context.Channel.SendMessageAsync($"Msg 1: {(messages.ElementAt(0).Content)}\nMsg 2: {(messages.ElementAt(1).Content)}");
                    return;
                }
            }

            //No Emote found
            await Context.Message.DeleteAsync();
            var msg = await Context.Channel.SendMessageAsync($"\"{shorthand}\" is not a valid shorthand");
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }

        [Command("ReactList"), Alias("ReactionList")]
        public async Task ReactList()
        {
            string content = "Emote  Shorthand\n\n";

            foreach (ReactEmote reactEmote in Program.ReactEmoteData)
            {
                content += $"{reactEmote.ChannelReactEmoteID} {reactEmote.emoteShortHand}\n";
            }

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(content);
        }

        [Command("Shot")]
        public async Task TakeShot()
        {
            if (Program.BlackoutQueueData.tokensUsed.Contains(Context.User.Id))
            {
                //token already used
                await Context.Channel.SendMessageAsync("You have already redeemed your token");
                return;
            }

            if (Program.BlackoutQueueData.lastShot == "") //invalid time. Take shot
            {
                Program.BlackoutQueueData.tokensUsed.Add(Context.User.Id);
                Program.BlackoutQueueData.lastShot = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                Program.SaveBlackoutQueueDataToFile();

                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> used redeemed a shot! Bottoms up <@194151124704428032>");
                return;
            }
            else
            {
                DateTime lastShotTime = DateTime.Parse(Program.BlackoutQueueData.lastShot);
                TimeSpan duration = DateTime.Now - lastShotTime;

                if (duration.TotalSeconds > Program.BlackoutQueueData.secondsBetweenShots) //valid take shot
                {
                    Program.BlackoutQueueData.tokensUsed.Add(Context.User.Id);
                    Program.BlackoutQueueData.lastShot = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                    Program.SaveBlackoutQueueDataToFile();

                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> used redeemed a shot! Bottoms up <@194151124704428032>");
                    return;
                }
                else
                {
                    double shit = Program.BlackoutQueueData.secondsBetweenShots - duration.TotalSeconds;
                    await Context.Channel.SendMessageAsync($"Still on cooldown: {(int)(shit / 60)} Min. {(int)(shit % 60)} Secs. Left");
                    return;
                }
            }
        }
        #endregion

        #region Accountabilibuddy
        public static int MiliToMin = 1000;


        [Command("StartDay")]
        public async Task StartDay(int workTime, int breakTime)
        {
            if (Context.Channel.Id != Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"])
            {
                var botMsg = await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"]}>");
                await Context.Message.DeleteAsync();
                return;
            }

            if (Program.AccountabilibuddyTimers.ContainsValue(Context.User.Id) == true)
            {
                await Context.Channel.SendMessageAsync("You have already started your day. Use the \"EndDay\" command before trying again");
                return;
            }

            var botMsg1 = await Context.Channel.SendMessageAsync($"Day Started! Current schedule is to work for {workTime} min(s) and then take a break for {breakTime} min(s)");

            AccountabilibuddyQueue.Accountabilibuddy currentBuddy = GetBuddyFromUID(Context.User.Id);

            currentBuddy.isWorking = true;
            currentBuddy.workTime = workTime;
            currentBuddy.breakTime = breakTime;
            currentBuddy.startTime = DateTime.Now;

            Program.SaveAccountabilibuddyDataToFile();

            //dont forget to fix timer mult
            QuantumTimer newTimer = new QuantumTimer(workTime * MiliToMin) { AutoReset = false };
            newTimer.Elapsed += OnAccountabilibuddyTimedEvent;
            newTimer.Enabled = true;
            newTimer.Start();

            Program.AccountabilibuddyTimers.Add(newTimer, Context.User.Id);
        }

        [Command("EndDay"), Alias("StopDay")]
        public async Task EndDay()
        {
            if (Context.Channel.Id != Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"])
            {
                var botMsg = await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"]}>");
                await Context.Message.DeleteAsync();
                return;
            }

            //Check if Timer Exists
            if (Program.AccountabilibuddyTimers.ContainsValue(Context.User.Id) == false)
            {
                await Context.Channel.SendMessageAsync($"You need to start a day to end one");
                return;
            }

            //Update Buddy Data
            AccountabilibuddyQueue.Accountabilibuddy currentBuddy = GetBuddyFromUID(Context.User.Id);
            currentBuddy.timeWorked += (int)(currentBuddy.startTime - DateTime.Now).TotalMinutes;

            //Find delete work timer
            foreach (var kvPair in Program.AccountabilibuddyTimers)
            {
                if (kvPair.Value == Context.User.Id)
                {
                    Program.AccountabilibuddyTimers.Remove(kvPair.Key);
                    kvPair.Key.Dispose();
                    break;
                }
            }

            //find and delete lunch timer
            foreach (var kvPair in Program.AccountabilibuddyLunchTimers)
            {
                if (kvPair.Value == Context.User.Id)
                {
                    Program.AccountabilibuddyLunchTimers.Remove(kvPair.Key);
                    kvPair.Key.Dispose();
                    break;
                }
            }

            var botMsg1 = await Context.Channel.SendMessageAsync($"Day Ended!");
        }

        [Command("StartLunch"), Alias("Lunch")]
        public async Task StartLunch(int duration)
        {
            if (Context.Channel.Id != Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"])
            {
                var botMsg = await Context.User.SendMessageAsync($"> {Context.Message.ToString()}\n" +
                                                    $"This Command can only be used in <#{Program.ServerConfigData.PointersAnonChatID["Accountabilibuddy"]}>");
                await Context.Message.DeleteAsync();
                return;
            }

            //Check if Timer Exists
            if (Program.AccountabilibuddyTimers.ContainsValue(Context.User.Id) == false)
            {
                await Context.Channel.SendMessageAsync($"You need to start a day to start a Lunch");
                return;
            }

            //check if Lunch timer Exists
            if (Program.AccountabilibuddyLunchTimers.ContainsValue(Context.User.Id) == true)
            {
                await Context.Channel.SendMessageAsync($"You have already started your lunch");
                return;
            }

            await Context.Channel.SendMessageAsync($"Now starting timer for a {duration} min lunch.");

            //dont forget to fix timer mult
            QuantumTimer newTimer = new QuantumTimer(duration * MiliToMin) { AutoReset = false };
            newTimer.Elapsed += OnAccountabilibuddyLunchTimedEvent;
            newTimer.Enabled = true;
            newTimer.Start();

            Program.AccountabilibuddyLunchTimers.Add(newTimer, Context.User.Id);

            //find and pause work timer
            foreach (var kvPair in Program.AccountabilibuddyTimers)
            {
                if (kvPair.Value == Context.User.Id)
                {
                    kvPair.Key.Pause();
                    return;
                }
            }
        }

        //EndLunch

        private AccountabilibuddyQueue.Accountabilibuddy GetBuddyFromUID(ulong user)
        {
            AccountabilibuddyQueue.Accountabilibuddy currentBuddy = null;

            foreach (var buddy in Program.AccountabilibuddyData.AllAccountabilibuddy)
            {
                if (buddy.userToken == Context.User.Id) //id already exists
                {
                    currentBuddy = buddy;
                    break;
                }
            }

            if (currentBuddy == null) //no user data, create profile
            {
                currentBuddy = new AccountabilibuddyQueue.Accountabilibuddy()
                {
                    userToken = Context.User.Id,
                };

                Program.AccountabilibuddyData.AllAccountabilibuddy.Add(currentBuddy);
            }

            return currentBuddy;
        }
        private void OnAccountabilibuddyTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //I might not need to do this with the way Contex doesn't change

            AccountabilibuddyQueue.Accountabilibuddy currentBuddy = GetBuddyFromUID(Program.AccountabilibuddyTimers[(QuantumTimer)source]);

            if (currentBuddy.isWorking == true) //go to break
            {
                Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Time to take a {currentBuddy.breakTime} min break!" +
                " Don't forget to post what you did this \"working period\" in <#795698234893795368>");
                //change timer
                currentBuddy.isWorking = false;
                ((QuantumTimer)source).Interval = currentBuddy.breakTime * MiliToMin;
                ((QuantumTimer)source).Enabled = true;
                ((QuantumTimer)source).Start();
            }
            else
            {
                Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Break ended! Time to get back to work!");
                //change timer
                currentBuddy.isWorking = true;
                ((QuantumTimer)source).Interval = currentBuddy.workTime * MiliToMin;
                ((QuantumTimer)source).Enabled = true;
                ((QuantumTimer)source).Start();
            }

            Program.SaveAccountabilibuddyDataToFile();
            return;
        }
        private void OnAccountabilibuddyLunchTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            AccountabilibuddyQueue.Accountabilibuddy currentBuddy = GetBuddyFromUID(Program.AccountabilibuddyLunchTimers[(QuantumTimer)source]);

            Context.Channel.SendMessageAsync($"<@{Context.User.Id}> Lunch timer has ended! Time to get back to work!");

            //destroy and remove lunch timer
            Program.AccountabilibuddyTimers.Remove((QuantumTimer)source);
            ((QuantumTimer)source).Dispose();

            //find and start work timer
            foreach (var kvPair in Program.AccountabilibuddyTimers)
            {
                if (kvPair.Value == Context.User.Id)
                {
                    kvPair.Key.Resume();
                    return;
                }
            }
            return;
        }


        #endregion

        #region Personal Links

        [Command("UpdateWebEmbed"), Alias("UpdateEmbed"), Summary("Updates the Website Embed")]
        public async Task UpdateWebsiteEmbed(string WebQuery)
        {
            if (await IsUserAuthorized("Admin", "Teacher"))
            {


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
        #endregion

        #region Lunchbox

        [Command("Lunchbox"), Alias("LB")]
        public async Task DisplayNextLunchbox()
        {
            foreach (Lunchbox lb in Program.BulletinBoardData.Lunchboxes)
            {
                if (lb.date > DateTime.Now)
                {
                    await Context.Channel.SendMessageAsync($"Topic:{lb.topic}\nSpeaker:{lb.speaker}\nDate:{lb.date}");
                    return;
                }
            }

            await Context.Channel.SendMessageAsync("No future lunchboxes scheduled");
        }

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

            //var builder = new EmbedBuilder()
            //    .WithTitle("New Lunchbox Added")
            //    .WithColor(new Color(37, 170, 225))
            //    .WithThumbnailUrl(Program.ServerConfigData.LunchboxIconURL)
            //    .AddField($"{lunchboxTopic}", $"{lunchboxSpeaker}\n{NewLunchbox.date.ToString("dddd, d MMMM yyyy")}");

            //var embed = builder.Build();
            //var msg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).SendMessageAsync(null, embed: embed).ConfigureAwait(false);

            await Context.Message.DeleteAsync();
            await UpdateLunchboxEvents();

            //await Task.Delay(3000);
            //await msg.DeleteAsync();
        }

        [Command("UpdateLunchbox"), Alias("UpdateLB", "updateLB", "updatelb", "updatelunchbox", "updatelunchBox", "updateLunchBox", "Updatelunchbox", "UpdatelunchBox", "UpdateLunchBox")]
        public async Task UpdateLunchboxEvents()
        {
            Program.BulletinBoardData.Lunchboxes.Sort((a, b) => a.date.CompareTo(b.date));
            Program.SaveBulletinBoardDataToFile();


            var pastLunchboxBuilder = new EmbedBuilder()
            .WithDescription("```fix\nPast Lunchbox Events\n```")
            .WithColor(new Color(37, 170, 225))
            .WithThumbnailUrl(Program.ServerConfigData.LunchboxIconURL);

            var futureLunchboxBuilder = new EmbedBuilder()
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
                pastLunchboxBuilder.AddField($"{lb.topic}", $"{lb.speaker}\n{lb.date.ToString("d MMMM yyyy")}");

            }

            //Future Embed
            for (int i = 0; i < Program.BulletinBoardData.FutureLunchboxesEmbedCount; i++)
            {
                if (EventSplitIdx + i >= Program.BulletinBoardData.Lunchboxes.Count())
                {
                    break;
                }

                Lunchbox lb = Program.BulletinBoardData.Lunchboxes[EventSplitIdx + i];
                futureLunchboxBuilder.AddField($"{lb.topic}", $"{lb.speaker}\n{lb.date.ToString("d MMMM yyyy")}");
            }

            var embed = pastLunchboxBuilder.Build();


            //Updates Past Embed
            IMessage pastChatReferences = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).GetMessageAsync(Program.BulletinBoardData.PastLunchboxesMsgID);

            if (pastChatReferences is IUserMessage pastMsg)
            {
                await pastMsg.ModifyAsync(x => x.Embed = embed);
            }

            embed = futureLunchboxBuilder.Build();

            //Updates Future Embed
            IMessage futureChatReferences = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Bulletin Board"]).GetMessageAsync(Program.BulletinBoardData.FutureLunchboxesMsgID);

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
        #endregion

        #region Bulletin Events
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

        [Command("InfoEvent"), Alias("eventinfo", "eventsinfo", "infoevents")]
        public async Task AddInfoBulletinEvent(string description, string location, string cost, string capacity, string eventURL, string iconURL)
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

                        EmbedBuilder builder;
                        try
                        {
                            builder = new EmbedBuilder()
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
                        }
                        catch (ArgumentException e) //incase a URL is dead
                        {
                            await Context.User.SendMessageAsync($"AHHHH You broke something. . . plz msg <@173226502710755328>\nMsg: {Context.Message}");
                            await Context.Guild.GetUser(173226502710755328).SendMessageAsync($"<@{Context.User.Id}> broke something.\n Msg: {Context.Message}\nContext: {e.Message}");
                            await Context.Message.DeleteAsync();
                            return;
                        }

                        Program.SaveBulletinBoardDataToFile();

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
                    EmbedBuilder builder;

                    //if incomplete event
                    if (bulletinEvent.Description == null)
                    {
                        builder = new EmbedBuilder()
                           .WithTitle(bulletinEvent.Title)
                           .WithUrl($"https://discordapp.com/invite/xQAcyyX")
                           .WithColor(new Color(0, 0, 255))
                           .WithDescription($"Missing")
                           .WithThumbnailUrl("https://cdn.discordapp.com/attachments/489949750762668035/647733785118375947/NoPhoto.png")
                           .AddField($"Time", bulletinEvent.EventDate.ToString("MMMM d yyyy \ndddd h:mm tt"), true)
                           .AddField($"Location", "Missing", true)
                           .AddField($"Cost", "Missing", true)
                           .AddField($"Capacity", "Missing", true)
                           .AddField($"Attending", "0", true)
                           .WithFooter($"By {Context.Guild.GetUser(bulletinEvent.author).Nickname}", $"{bulletinEvent.authorIconURL}")
                           .WithTimestamp(bulletinEvent.embedCreated);
                    }
                    else
                    {
                        try
                        {
                            builder = new EmbedBuilder()
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
                        }
                        catch (ArgumentException e) //incase a URL is dead
                        {
                            await Context.User.SendMessageAsync($"AHHHH You broke something. . . plz msg <@173226502710755328>\nMsg: {Context.Message}");
                            await Context.Guild.GetUser(173226502710755328).SendMessageAsync($"<@{Context.User.Id}> broke something.\n Msg: {Context.Message}\nContext: {e.Message}");
                            continue;
                        }

                    }

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
                if (bulletinEvent.Title.ToLower() == topic.ToLower() && (Context.User.Id == bulletinEvent.author || await IsUserAuthorized("Admin", "Teacher")))
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

        #endregion

        #region Admin Commands

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

        [Command("UpdateUserList")]
        public async Task UpdateUserList()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            var AllUsers = Context.Guild.Users;

            List<ulong> allActiveIDs = new List<ulong>();

            foreach (SocketGuildUser users in AllUsers)
            {
                allActiveIDs.Add(users.Id);

                UserProfile user = GetUserProfile(users.Id);

                if (users.Nickname == null || users.Nickname == "" || users.Nickname == "null")
                {
                    user.userNickname = "NaN_NO_NICKNAME";
                }
                else
                {
                    user.userNickname = users.Nickname;
                }

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
                    else if (users.Roles.Contains(Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID["Class Of 2022"])) == true)
                    {
                        user.GradYear = 2022;
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
            }

            foreach (UserProfile user in Program.UserData)
            {
                if (allActiveIDs.Contains(user.userID) == false)
                {
                    user.userNickname = "NaN_NO_LONGER_IN_SERVER";
                }
            }

            Program.UserData = Program.UserData.OrderBy(x => x.userNickname).ToList();

            Program.SaveUserDataToFile();
            var msg = await Context.Channel.SendMessageAsync("User List Updated");
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }

        [Command("GetUserAnomalies")]
        public async Task GetUserAnomalies()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            await UpdateUserList();

            //Program.UserData.Sort((b, a) => a.isStudent.CompareTo(b.isStudent));

            string reportMsg = ":no_entry: = Missing Username, :poop: = Missing Roles\n";

            foreach (UserProfile user in Program.UserData)
            {
                if ((user.isGuest == false && user.isTeacher == false && user.isStudent == false))
                {
                    if (user.isGuest == false && user.isTeacher == false && user.isStudent == false)
                    {
                        reportMsg += $"<@{user.userID}>: :poop:\n";
                    }
                }
                else
                {
                    if (user.userNickname == null)
                    {
                        reportMsg += $"<@{user.userID}>: :no_entry:\n";
                    }
                }
            }

            if (reportMsg.Length > 2000)
            {
                //too lazy to implement this shi
            }

            await Context.Channel.SendMessageAsync(reportMsg);
        }

        [Command("Purge", RunMode = RunMode.Async)]
        public async Task PurgeMessages(int msgCount)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            if (msgCount > 100)
            {
                msgCount = 100;
            }
            else if (msgCount < 0)
            {
                msgCount = 0;
            }

            var messages = await Context.Channel.GetMessagesAsync(msgCount).FlattenAsync();

            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);

            var botMsg = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["Admin"]).SendMessageAsync($"User: <@{Context.User.Id}> purged {msgCount} messages in the <#{Context.Channel.Id}>");
        }

        [Command("SeedRoles")]
        public async Task SeedRoles()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            var newMsg = await Context.Channel.SendMessageAsync("THIS COMMAND IS CURRENTLY DEPRECATED");
            await Task.Delay(5000);
            await newMsg.DeleteAsync();
            await Context.Message.DeleteAsync();
            return;

            /*
            List<IEmote> reactions = new List<IEmote>();

            var builder = new EmbedBuilder()
                        .WithDescription("React with these emotes to get access to the corresponding channels!~")
                        .WithAuthor("Pointers Anonymous", "https://cdn.discordapp.com/icons/487403414741975040/a_c01491057777dfa5b5313a65868fd1a4.webp?size=128", null);


            foreach (KeyValuePair<string, ChannelRoles> EmoteData in Program.ChannelRolesData)
            {
                builder.AddField($"{EmoteData.Key}", $"{EmoteData.Value.ChannelReactEmote}", false);
                reactions.Add(EmoteData.Value.ChannelReactEmote);
            }

            var embed = builder.Build();
            var msg = await Context.Channel.SendMessageAsync(null, embed: embed).ConfigureAwait(false);


            await msg.AddReactionsAsync(reactions.ToArray(), null);

            Program.ServerConfigData.ServerRoleSetUpMsgID = msg.Id;
            Program.SaveServerDataToFile();

            await Context.Message.DeleteAsync();
            */
        }

        [Command("UpdateSeedRoles")]
        public async Task UpdateSeededRoles()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            IMessage message;
            message = await Context.Guild.GetTextChannel(Program.ServerConfigData.PointersAnonChatID["The Law"]).GetMessageAsync(Program.ServerConfigData.ServerRoleSetUpMsgID);

            if (message == null) //if MSG not found
            {
                var badMsg = await Context.Channel.SendMessageAsync($"The Role Message is not found in <#{Program.ServerConfigData.PointersAnonChatID["The Law"]}>");
                await Task.Delay(5000);
                await badMsg.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }

            //remake and update embed
            List<IEmote> reactions = new List<IEmote>();

            var builder = new EmbedBuilder()
                        .WithDescription("React with these emotes to get access to the corresponding channels!~")
                        .WithAuthor("Pointers Anonymous", "https://cdn.discordapp.com/icons/487403414741975040/a_c01491057777dfa5b5313a65868fd1a4.webp?size=128", null);


            foreach (KeyValuePair<string, ChannelRoles> EmoteData in Program.ChannelRolesData)
            {
                builder.AddField($"{EmoteData.Key}", $"{EmoteData.Value.ChannelReactEmote}", false);
                reactions.Add(EmoteData.Value.ChannelReactEmote);
            }

            var embed = builder.Build();
            await (message as IUserMessage).ModifyAsync(x => x.Embed = embed);

            await (message as IUserMessage).AddReactionsAsync(reactions.ToArray(), null);

            var goodMsg = await Context.Channel.SendMessageAsync("Updated");
            await Task.Delay(5000);
            await goodMsg.DeleteAsync();
            await Context.Message.DeleteAsync();

        }

        [Command("UpdateData")]
        public async Task UpdateData()
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            Program.UpdateAllDataFromFiles();
            await Context.Message.DeleteAsync();
            var botMsg = await Context.Channel.SendMessageAsync("Data Updated");
            await Task.Delay(5000);
            await botMsg.DeleteAsync();
        }

        [Command("DM")]
        public async Task DM(ulong targetUser, [Remainder] string msg)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }

            await Context.Guild.GetUser(targetUser).SendMessageAsync(msg);
        }

        [Command("SecretSanta")]
        public async Task SecretSantaDM(ulong sender, string receiver, string discordID, string interest1, string interest2, string interest3, string address)
        {
            if (await IsUserAuthorized("Admin") == false)
            {
                return;
            }
            var builder = new EmbedBuilder()
                .WithTitle("Pointers Anonymous Secret Santa 2020")
                .WithDescription($"Hello! You are officially a Pointers Anonymous Secret Santa~ \nThey are: {receiver} ({discordID})\nI have listed their interests below~\n\nPlease keep the gifts under $20, and message Ray once the gift has been sent.")
                .WithColor(new Color(0xF7833))
                .WithFooter(footer =>
                {
                    footer
                        .WithText("Sent by Ray (AKA Quantum Blue)");
                })
                .WithThumbnailUrl("https://cdn.discordapp.com/attachments/545297341368762368/785383889463148614/unknown.png")
                .AddField("Interest 1", $"{interest1}")
                .AddField("Interest 2", $"{interest2}")
                .AddField("Interest 3", $"{interest3}")
                .AddField("Address", $"{address}");
            var embed = builder.Build();

            await Context.Guild.GetUser(sender).SendMessageAsync(
                null,
                embed: embed)
                .ConfigureAwait(false);
        }

        #endregion

        #region Private Methods
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
            var roleCheckUser = Context.User as SocketGuildUser;

            foreach (string role in roles)
            {
                var AuthRole = Context.Guild.GetRole(Program.ServerConfigData.PointersAnonRoleID[role]);

                if (roleCheckUser.Roles.Contains(AuthRole) == true)
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

        private string ServerStatBoxPrinter(int userCount)
        {
            int boxPerUser = 5;
            string box = " ";


            for (int i = 0; i < userCount / boxPerUser + 1; i++)
            {
                box += "▉";
            }

            box += $" {userCount}";
            return box;
        }

        private string GraduationDialoguePrinter(int year)
        {
            string output = ":video_game:\n";
            DateTime startDate;
            DateTime gradDate;
            int daysDiff;

            if (year == 2020)
            {
                startDate = new DateTime(2018, 8, 16);
                gradDate = new DateTime(2020, 7, 22);
                daysDiff = ((TimeSpan)(gradDate - DateTime.Today)).Days;

                output += $"> :confetti_ball:Class of 2020 Graduation!:confetti_ball:\n" +
                               $"Graduation was on July 22nd 2020\n\n" +
                               $"Congrats on graduating gamers";
            }
            else if (year == 2021)
            {
                startDate = new DateTime(2019, 8, 12);
                gradDate = new DateTime(2021, 7, 14);
                daysDiff = ((TimeSpan)(gradDate - DateTime.Today)).Days;

                output += $"> :confetti_ball:Class of 2021 Graduation!:confetti_ball:\n" +
                                $"Graduation is on July 14th 2021\n\n" +
                                $"~ {daysDiff} days to go! ~\n" +
                                $"```\n" +
                                $"{GraduationStatBoxPrinter(startDate, gradDate)}\n" +
                                $"```";
            }
            else if (year == 2022)
            {
                startDate = new DateTime(2020, 8, 12);
                gradDate = new DateTime(2022, 7, 14);
                daysDiff = ((TimeSpan)(gradDate - DateTime.Today)).Days;

                output += $"> :confetti_ball:Class of 2021 Graduation!:confetti_ball:\n" +
                                $"Graduation is on UNKOWN 2022\n\n" +
                                $"~ {daysDiff} days to go?! ~\n" +
                                $"```\n" +
                                $"{GraduationStatBoxPrinter(startDate, gradDate)}\n" +
                                $"```";
            }
            else
            {
                //this could lead to a infinite loop if the years arn't supported
                output = GraduationDialoguePrinter(2020);
                output += GraduationDialoguePrinter(2021);
                output += GraduationDialoguePrinter(2022);
            }

            return output;
        }
        private string GraduationStatBoxPrinter(DateTime startDate, DateTime endDate)
        {
            int gradDiff = (endDate - startDate).Days;
            int nowDiff = (DateTime.Now - startDate).Days;


            string output = $"~Progress Bar~ ({(float)((int)((float)nowDiff / (float)gradDiff * 10000.0f) / 100.0f)}%)\n";

            output += "";
            for (int i = 0; i < 20; i++)
            {
                if (i == 10)
                {
                    output += " Year 1\n";
                }

                if ((float)nowDiff / (float)gradDiff > 1.0f / 20.0f * i)
                {
                    output += "▉";
                }
                else
                {
                    if (i == 10)
                    {
                        output += "□";
                    }

                    output += " □";
                }
            }
            output += " Year 2";


            return output;
        }
        private Discord.Embed CreateMonsterNicknameEmbed(MonsterHunterNicknames monsterData)
        {
            EmbedBuilder builder = new EmbedBuilder();
            try
            {
                builder = new EmbedBuilder()
                                        .WithTitle("Monster Hunter Nicknames!~")
                                        .WithDescription($"Nicknames for {monsterData.monsterName}")
                                        .WithThumbnailUrl(monsterData.monsterIconURL);
            }
            catch (ArgumentException) //incase a URL is dead
            {
                builder = new EmbedBuilder()
                                        .WithTitle("Monster Hunter Nicknames!~")
                                        .WithDescription($"Nicknames for {monsterData.monsterName}");
            }

            Dictionary<ulong, string> fieldContents = new Dictionary<ulong, string>();

            foreach (MonsterHunterNicknames.NicknameData printnicknames in monsterData.nicknameData)
            {
                if (fieldContents.ContainsKey(printnicknames.creatorID))
                {
                    fieldContents[printnicknames.creatorID] += $"\n{printnicknames.nickname}";
                }
                else
                {
                    fieldContents.Add(printnicknames.creatorID, printnicknames.nickname);
                }
            }

            if (fieldContents.Count() == 0)
            {
                builder.AddField($"No Nicknames created", $"Use >Adopt \"{monsterData.monsterName}\" \"NICKNAME\"");
            }

            foreach (var item in fieldContents)
            {
                builder.AddField($"By {Context.Guild.GetUser(item.Key).Nickname}", item.Value);
            }

            return builder.Build();
        }
        #endregion
    }
}
