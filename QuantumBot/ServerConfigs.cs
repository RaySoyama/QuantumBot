using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class ServerConfigs
    {
        //Discord Server Token
        public string TOKEN;

        //Server Bot Prefix
        public char prefix;

        //List of Chat References
        public Dictionary<string, ulong> PointersAnonChatID = new Dictionary<string, ulong>();
        //List of Role References
        public Dictionary<string, ulong> PointersAnonRoleID = new Dictionary<string, ulong>();
        //List of User References
        public Dictionary<string, ulong> PointersAnonUserID = new Dictionary<string, ulong>();
        public Dictionary<Program.WEBSITES, WebsiteProfile> WebsiteData = new Dictionary<Program.WEBSITES, WebsiteProfile>();

        //Msg ID for the Role set up
        public ulong ServerRoleSetUpMsgID;

        //Duration in Miliseconds, how long to wait for the Music bot deletetion
        public int MusicBoxPurgeTimer = 1500;
        public List<string> UnityVersion;
        public int UnityCrashCount;
        public int AlatreonFailCount;
        public string UnityIconURL;
        public string ProjectProposalDocURL;
        public string AIESchoolCalender;
        public string LunchboxIconURL;
    }
}
