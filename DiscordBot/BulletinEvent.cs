using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class BulletinEvent
    {
        public string Title;

        public DateTime EventDate;

        public string Description;

        public string EventURL;

        public string Cost;

        public string Location;

        public int Capacity;

        public string IconURL;


        public string authorIconURL;
        public ulong author;
        public DateTime embedCreated;
        public ulong MsgID;
    }
}
