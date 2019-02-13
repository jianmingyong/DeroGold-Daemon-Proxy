using System;
using System.IO;
using System.Threading.Tasks;
using TheDialgaTeam.DependencyInjection;

namespace DeroGoldDaemonProxy.Services.IO
{
    public sealed class FilePathService : IInitializableAsync
    {
        public string ConsoleLogFilePath { get; private set; }

        public string CheckpointsFilePath { get; private set; }

        private static string ResolveFullPath(string filePath)
        {
            return Path.GetFullPath(ResolveFilePath(filePath));
        }

        private static string ResolveFilePath(string filePath)
        {
            return filePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        public Task InitializeAsync()
        {
            ConsoleLogFilePath = ResolveFullPath($"{Environment.CurrentDirectory}/Logs/{DateTime.UtcNow:yyyy-MM-dd}.txt");
            CheckpointsFilePath = ResolveFullPath($"{Environment.CurrentDirectory}/Data/checkpoints.csv");

            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Logs")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Logs"));

            if (!Directory.Exists(ResolveFullPath($"{Environment.CurrentDirectory}/Data")))
                Directory.CreateDirectory(ResolveFullPath($"{Environment.CurrentDirectory}/Data"));

            return Task.CompletedTask;
        }
    }
}