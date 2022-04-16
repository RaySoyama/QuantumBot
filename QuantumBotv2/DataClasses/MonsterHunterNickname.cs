using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumBotv2.DataClass
{
    public class MonsterHunterNicknames : DataClass
    {
        public override string FileName()
        {
            return "MonsterHunterNicknames.json";
        }

        public class MonsterNicknames
        {
            public string monsterName = "Default Name";
            public string monsterIconURL = "https://monsterhunterworld.wiki.fextralife.com/file/Monster-Hunter-World/black_bandage-mhw-wiki-guide.png";
            public Dictionary<string, ulong> nicknameData = new Dictionary<string, ulong>(); //Nicknames + CreatorId
        }
        public List<MonsterNicknames> allMonsterHunterNicknames = new List<MonsterNicknames>();
    }
}