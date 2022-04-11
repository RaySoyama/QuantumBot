using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBotv2.DataClass
{
    public class ServerConfig : DataClass
    {
        public override string FileName()
        {
            return "ServerConfig.json";
        }

        //Discord Server Token
        public string Token;

        //Server Bot Prefix
        public char prefix;

        //List of Channel ID's
        public Dictionary<string, ulong> channelID = new Dictionary<string, ulong>();

        //List of Message ID's
        public Dictionary<string, ulong> messageID = new Dictionary<string, ulong>();

        //List of Role ID's
        public Dictionary<string, ulong> roleID = new Dictionary<string, ulong>();

        //List of User ID's
        public Dictionary<string, ulong> userID = new Dictionary<string, ulong>();
    }
}
