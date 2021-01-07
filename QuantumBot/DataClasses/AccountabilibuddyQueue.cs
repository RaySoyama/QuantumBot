using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class AccountabilibuddyQueue
    {
        public class Accountabilibuddy
        {
            public ulong userToken;
            public bool isWorking = false;

            public int workTime = 60;
            public int breakTime = 5;

            public DateTime startTime;

            public int timeWorked = 0;
        }

        public List<Accountabilibuddy> AllAccountabilibuddy = new List<Accountabilibuddy>();
    }
}