using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Newtonsoft.Json;


namespace QuantumBotv2.DataClass
{
    public class ClientLog : DataClass
    {
        public override string FileName()
        {
            return "ClientLogs.json";
        }

        public class ClientLogData
        {
            public DateTime timestamp = DateTime.UtcNow;
            public string logSeverity = "DefaultSeverity";
            public string logSource = "DefaultSource";
            public string logErrorMessage = "DefaultErrorMessage";

            public ClientLogData()
            {

            }

            public ClientLogData(LogMessage logMessage)
            {
                timestamp = DateTime.UtcNow;
                logSeverity = logMessage.Severity.ToString();
                logSource = logMessage.Source;
                logErrorMessage = logMessage.Message;
            }
        }
        public List<ClientLogData> allClientLogs = new List<ClientLogData>();

        public string ClientLogDataAsString(ClientLogData clientLogData)
        {
            if (clientLogData == null)
            {
                throw new NotImplementedException();
            }

            return JsonConvert.SerializeObject(clientLogData, Formatting.Indented);
        }
    }
}
