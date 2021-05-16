using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class BlackoutQueue
    {
        public ulong shotTaker = 194151124704428032;
        public List<ulong> tokensUsed = new List<ulong>();
        public float secondsBetweenShots = 600;
        public string lastShot = "";
    }

}
