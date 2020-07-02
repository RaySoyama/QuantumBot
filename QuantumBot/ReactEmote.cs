using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public class ReactEmote
    {
        public string emoteShortHand;
        public string ChannelReactEmoteID;
        public Discord.IEmote ChannelReactEmote = Discord.Emote.Parse("<a:partyparrot:647210646177447936>");
    }
}