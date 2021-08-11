using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class Program
{
    private static string quantumBotClientConfigFileName = "QuantumBotClientConfig.json";
    private static string quantumBotClientConfigFullPath = "";

    private static QuantumBotClientConfig quantumBotClientConfig = null;
    static async Task Main(string[] args)
    {
        Console.WriteLine("Loading Config");
        LoadConfig();

        int totalTime = (int)(quantumBotClientConfig.MinutesBetweenRestarts * 60000);
        int currentTime = totalTime;

        while (true)
        {
            Console.WriteLine($"Restarting - {DateTime.Now.ToString("g")}");
            //restart
            RestartQuantumBot();

            //countdown
            while (currentTime > 0)
            {
                await Task.Delay(5000);
                currentTime -= 5000;
                Console.WriteLine($"Countdown - {currentTime / 1000}");
            }

            currentTime = totalTime;
        }
    }

    private static void RestartQuantumBot()
    {
        Process[] processList = Process.GetProcessesByName(quantumBotClientConfig.QuantumbBotName);

        if (processList.Length > 0)
        {
            Console.WriteLine($"Killing {quantumBotClientConfig.QuantumbBotName} process");
            foreach (Process proc in processList)
            {
                proc.Kill();
            }
        }
        Console.WriteLine($"Starting {quantumBotClientConfig.QuantumbBotName} process\n");

        ProcessStartInfo QuantumBotProcStartInfo = new ProcessStartInfo
        {
            FileName = quantumBotClientConfig.QuantumbBotShortcutPath,
            UseShellExecute = true,
            CreateNoWindow = true
        };

        var QuantumBotProc = new Process { StartInfo = QuantumBotProcStartInfo };

        QuantumBotProc.Start();
    }

    private static void LoadConfig()
    {
        quantumBotClientConfigFullPath = GetFilePath(quantumBotClientConfigFileName);

        string contents = File.ReadAllText(quantumBotClientConfigFullPath);
        quantumBotClientConfig = JsonConvert.DeserializeObject<QuantumBotClientConfig>(contents);

        if (quantumBotClientConfig == null)
        {
            quantumBotClientConfig = new QuantumBotClientConfig();
            File.WriteAllText(quantumBotClientConfigFullPath, JsonConvert.SerializeObject(quantumBotClientConfig, Formatting.Indented));
        }

        if (File.Exists(quantumBotClientConfig.QuantumbBotShortcutPath) == false)
        {
            Console.WriteLine($"Quantum Bot shortcut invalid");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }

    private static string GetFilePath(string textFileName)
    {
        string fullPath = "";

        fullPath = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(textFileName)).ToString();

        Directory.CreateDirectory(fullPath + "\\ConfigFiles\\");

        fullPath += "\\ConfigFiles\\" + textFileName;

        //if no file exist, create
        if (File.Exists(fullPath) == false)
        {
            var myFile = File.Create(fullPath);
            myFile.Close();
        }
        return fullPath;
    }

}

