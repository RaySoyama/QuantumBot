using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;


namespace QuantumBotv2.DataClass
{
    public class UserProfile : DataClass
    {
        public override string FileName()
        {
            return "UserProfiles.json";
        }
        public class UserData
        {
            public enum GamePlatforms
            {
                Steam = 1,
                RiotID = 2,
                Switch = 3,
                GenshinImpact = 4,
            }

            public string userName = "NoUserName";
            public string userNickname = "NoUserNickname";
            public ulong userID = 6969696969696969;
            public Dictionary<GamePlatforms, string> UserGameCodeIndex = new Dictionary<GamePlatforms, string>();

            public UserData()
            {

            }
            public UserData(SocketGuildUser guildUser)
            {
                userName = $"{guildUser.Username}#{guildUser.Discriminator}";
                userNickname = $"{guildUser.Nickname}";
                userID = guildUser.Id;
            }
        }

        /// <summary>
        /// Tries to find UserData from database, if it doesn't exist, create a profile
        /// </summary>
        /// <param name="guildUser"></param>
        /// <returns></returns>
        public UserData GetUserData(SocketGuildUser guildUser)
        {
            UserData userData = null;

            foreach (UserData user in DataClassManager.Instance.userProfile.allUserData)
            {
                if (user.userID == guildUser.Id)
                {
                    userData = user;
                    break;
                }
            }

            //If user profile doesn't exist, make one and save
            if (userData == null)
            {
                userData = new UserData(guildUser);
                DataClassManager.Instance.userProfile.allUserData.Add(userData);
            }

            userData = UpdateUserData(userData, guildUser);

            DataClassManager.Instance.SaveData(DataClassManager.Instance.userProfile);
            return userData;
        }

        public UserData UpdateUserData(UserData userData, SocketGuildUser guildUser)
        {
            userData.userName = $"{guildUser.Username}#{guildUser.Discriminator}";
            userData.userNickname = $"{guildUser.Nickname}";
            userData.userID = guildUser.Id;
            return userData;
        }

        public List<UserData> allUserData = new List<UserData>();

    }
}