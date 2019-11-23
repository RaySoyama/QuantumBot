using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class BulletinBoard
    {
        public int PastLunchboxesEmbedCount = 3;
        public int FutureLunchboxesEmbedCount = 3;

        public ulong PastLunchboxesMsgID = 66969696969;
        public ulong FutureLunchboxesMsgID = 66969696969;

        public List<Lunchbox> Lunchboxes = new List<Lunchbox>();

        public int BulletinEventMax = 5;

        public List<BulletinEvent> BulletinEvents = new List<BulletinEvent>();

    }
}
