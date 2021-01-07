using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class UserProfile
    {
        public ulong userID;
        public string userNickname;

        public int GradYear;
        public bool isStudent;
        public bool isTeacher;
        public bool isGuest;

        public Dictionary<Program.WEBSITES, string> UserWebsiteIndex;
        public Dictionary<string, string> UserGameCodeIndex;
    }
}
