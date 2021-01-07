using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class BlackoutQueue
    {
        public List<ulong> tokensUsed = new List<ulong>();

        public float secondsBetweenShots = 600;
        public string lastShot = "";
    }

}
