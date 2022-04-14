using System.Collections.Generic;
using Discord;

namespace QuantumBotv2.DataClass
{
    public class SlashCommands : DataClass
    {
        public override string FileName()
        {
            return "SlashCommands.json";
        }

        public class SlashCommandData
        {
            public SlashCommandBuilder slashCommandBuilder = new SlashCommandBuilder();
            public string commandMethodName = "NOLOGIC";

            public SlashCommandData(SlashCommandBuilder _slashCommandBuilder, string _commandMethodName)
            {
                slashCommandBuilder = _slashCommandBuilder;
                commandMethodName = _commandMethodName;
            }
        }

        public List<SlashCommandData> allSlashCommands = new List<SlashCommandData>();
        public Dictionary<string, string> allButtonCommands = new Dictionary<string, string>();
    }
}
