using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace QuantumBotv2.DataClass
{
    public class DataClassManager
    {
        private static DataClassManager instance = null;
        public static DataClassManager Instance
        {
            get
            {
                return instance;
            }
        }

        public DataClassManager()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public ServerConfig serverConfigs = new ServerConfig();
        public SlashCommands slashCommands = new SlashCommands();
        public MessageLog messageLog = new MessageLog();
        public string dataFolderPath = "QuantumBotDataFiles";

        public void LoadAllData()
        {
            serverConfigs = LoadData(serverConfigs);
            slashCommands = LoadData(slashCommands);
            messageLog = LoadData(messageLog);
            //add more
        }

        public string GetFilePath<T>(T dataClass) where T : DataClass
        {
            string path = Directory.GetParent(Path.GetFullPath(dataClass.FileName())).ToString();
            Directory.CreateDirectory($"{path}\\{dataFolderPath}\\");

            path += $"\\{dataFolderPath}\\{dataClass.FileName()}";

            //if no file exist, create
            if (File.Exists(path) == false)
            {
                var myFile = File.Create(path);
                myFile.Close();
            }

            return path;
        }

        public T LoadData<T>(T dataClass) where T : DataClass
        {
            //Checks if File Exists
            string path = GetFilePath(dataClass);

            //Grabs Contents
            string contents = File.ReadAllText(path);
            dataClass = JsonConvert.DeserializeObject<T>(contents);

            if (dataClass == null)
            {
                throw new System.NotImplementedException();
            };

            return dataClass;
        }

        public void SaveData<T>(T dataClass) where T : DataClass
        {
            string contents = JsonConvert.SerializeObject(dataClass, Formatting.Indented);
            File.WriteAllText(GetFilePath(dataClass), contents);
            return;
        }

        private void LoadMessageLogs()
        {
            //TODO
            //split by Year/Month/Week
        }
        private void SaveMessageLogs()
        {
            //TODO
            //split by Year/Month/Week
        }
    }
}