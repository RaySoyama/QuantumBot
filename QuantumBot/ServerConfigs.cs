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



        public List<string> UnityVersion;

        public string UnityIconURL;

        public string ProjectProposalDocURL;

        public string AIESchoolCalender;

        public string LunchboxIconURL;
    }
}
