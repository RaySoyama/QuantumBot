using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class MonsterHunterNicknames
    {
        public class NicknameData
        {
            public ulong creatorID = 6969696969;
            public string nickname = "";
        }

        public string monsterName = "Default Name";
        public string monsterIconURL = "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png";
        public List<NicknameData> nicknameData = new List<NicknameData>();
    }
}
