using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;


namespace QuantumBotv2.DataClass
{
    public class MessageLog : DataClass
    {
        public override string FileName()
        {
            return "MessageLogs.json";
        }

        public class MessageData
        {
            public string authorName = "NoAuthorName";
            public string authorNickname = "NoAuthorNickname";
            public ulong authorID = 6969696969696969;
            public DateTimeOffset timestamp = DateTimeOffset.Now;
            public string channelName = "NoChannelName";
            public ulong channelID = 6969696969696969;
            public string messageContents = "";
            public List<string> messageAttachments = new List<string>();

            public MessageData()
            {

            }

            public MessageData(SocketMessage message)
            {
                authorName = $"{message.Author.Username}#{message.Author.Discriminator}";

                try
                {
                    authorNickname = $"{((IGuildUser)message.Author).Nickname}";
                }
                catch (System.InvalidCastException)
                {
                    authorNickname = "NoAuthorNickname - DM";
                }

                authorID = message.Author.Id;
                timestamp = message.Timestamp;
                channelName = message.Channel.Name;
                channelID = message.Channel.Id;
                messageContents = message.Content;

                foreach (var fileLink in message.Attachments)
                {
                    messageAttachments.Add(fileLink.Url);
                }
            }
        }
        public List<MessageData> allMessageLogs = new List<MessageData>();

        public string MessageDataAsString(MessageData messageData)
        {
            if (messageData == null)
            {
                throw new NotImplementedException();
            }

            return JsonConvert.SerializeObject(messageData, Formatting.Indented);
        }

        public int NumberOfMessagesFromUser(ulong userId)
        {
            //note: this does not load the data, so you're gonna have to do that yourself
            int msgCount = 0;
            foreach (MessageData messageData in allMessageLogs)
            {
                if (messageData.authorID == userId)
                {
                    msgCount++;
                }
            }
            return msgCount;
        }
    }
}
