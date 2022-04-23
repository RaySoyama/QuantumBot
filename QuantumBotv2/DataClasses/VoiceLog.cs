using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;


namespace QuantumBotv2.DataClass
{
    public class VoiceLog : DataClass
    {
        public override string FileName()
        {
            return "VoiceLogs.json";
        }

        public class VoiceData
        {
            public string authorName = "NoAuthorName";
            public ulong authorID = 6969696969696969;
            public DateTimeOffset timestamp = DateTimeOffset.Now;
            public string channelName = "NoChannelName";
            public ulong channelID = 6969696969696969;
            public bool isStreaming = false;
            public bool isMuted = false;
            //Are they connecting or disconnecting from the VC?
            public bool isConnecting = false;
            public VoiceData()
            {

            }
        }
        public class UserVoiceStats
        {
            public ulong userID = 69696969;
            public TimeSpan totalTimeInCall = new TimeSpan();
            public TimeSpan totalTimeMuted = new TimeSpan();

            public TimeSpan longestCallTime = new TimeSpan();
            public DateTimeOffset longestCallDate = new DateTimeOffset();
            public ulong longestCallChannel = 69696969;
        }

        public List<VoiceData> allVoiceLogs = new List<VoiceData>();
        public List<VoiceData> ParseVoiceData(SocketUser socketUser, SocketVoiceState oldSocketVoiceState, SocketVoiceState newSocketVoiceState)
        {
            List<VoiceData> parsedVoiceData = new List<VoiceData>();

            SocketGuildUser guildUser = socketUser as SocketGuildUser;

            /*
                Null -> Thing = Connecting to Channel
                Thing -> Null = Disconnecting
                Thing -> Thing = Tranfering Channels (disconnect & connect)
            */

            if (oldSocketVoiceState.VoiceChannel != null) //they are transfering from a different channel (or disconnecting)
            {
                parsedVoiceData.Add(new VoiceData() //disconnect from old channel
                {
                    authorName = $"{guildUser.Username}#{guildUser.Discriminator}",
                    authorID = guildUser.Id,
                    timestamp = DateTimeOffset.UtcNow,
                    channelName = oldSocketVoiceState.VoiceChannel.Name,
                    channelID = oldSocketVoiceState.VoiceChannel.Id,
                    isStreaming = oldSocketVoiceState.IsStreaming,
                    isMuted = (oldSocketVoiceState.IsMuted || guildUser.IsSelfMuted),
                    isConnecting = false
                });

                if (newSocketVoiceState.VoiceChannel != null) //They are connecting to the new channel
                {
                    parsedVoiceData.Add(new VoiceData() //connect to new channel
                    {
                        authorName = $"{guildUser.Username}#{guildUser.Discriminator}",
                        authorID = guildUser.Id,
                        timestamp = DateTimeOffset.UtcNow,
                        channelName = newSocketVoiceState.VoiceChannel.Name,
                        channelID = newSocketVoiceState.VoiceChannel.Id,
                        isStreaming = newSocketVoiceState.IsStreaming,
                        isMuted = (newSocketVoiceState.IsMuted || guildUser.IsSelfMuted),
                        isConnecting = true
                    });
                }
            }
            else
            {
                parsedVoiceData.Add(new VoiceData()
                {
                    authorName = $"{guildUser.Username}#{guildUser.Discriminator}",
                    authorID = guildUser.Id,
                    timestamp = DateTimeOffset.UtcNow,
                    channelName = newSocketVoiceState.VoiceChannel.Name,
                    channelID = newSocketVoiceState.VoiceChannel.Id,
                    isStreaming = newSocketVoiceState.IsStreaming,
                    isMuted = (newSocketVoiceState.IsMuted || guildUser.IsSelfMuted),
                    isConnecting = true
                });
            }

            return parsedVoiceData;
        }

        public UserVoiceStats GetUserVoiceCallStats(ulong userID)
        {
            VoiceData onConnect = null;
            UserVoiceStats userVoiceStats = new UserVoiceStats() { userID = userID };

            foreach (VoiceData vd in allVoiceLogs)
            {
                if (vd.authorID == userID)
                {
                    if (vd.isConnecting == true)
                    {
                        onConnect = vd;
                    }
                    else
                    {
                        if (onConnect == null)
                        {
                            //Bad Data? You're disconnecting but we didn't catch you connecting
                            continue;
                        }

                        //Assume we are disconnecting from OnConnect 
                        userVoiceStats.totalTimeInCall = userVoiceStats.totalTimeInCall.Add(vd.timestamp.Subtract(onConnect.timestamp));

                        if (vd.isMuted == true)
                        {
                            userVoiceStats.totalTimeMuted = userVoiceStats.totalTimeMuted.Add(vd.timestamp.Subtract(onConnect.timestamp));
                        }

                        //I can add more telemetry here

                        onConnect = null;
                    }
                }
            }

            return userVoiceStats;
        }
    }
}
