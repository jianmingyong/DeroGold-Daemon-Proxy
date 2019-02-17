using System;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Daemon;

namespace DeroGoldRemoteDaemonProxy.Services.Console
{
    public class ConsoleCommandService : IInitializable
    {
        private LoggerService LoggerService { get; }

        private RemoteDaemonCollectionService RemoteDaemonCollectionService { get; }

        private bool IsRunning { get; set; }

        public ConsoleCommandService(LoggerService loggerService, RemoteDaemonCollectionService remoteDaemonCollectionService)
        {
            LoggerService = loggerService;
            RemoteDaemonCollectionService = remoteDaemonCollectionService;
        }

        public void Initialize()
        {
            Task.Factory.StartNew(async () =>
            {
                IsRunning = true;

                while (IsRunning)
                {
                    var input = await System.Console.In.ReadLineAsync().ConfigureAwait(false);

                    if (input.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    {
                        LoggerService.LogMessage("Exiting Application...", ConsoleColor.Green);
                        break;
                    }
                        

                    if (input.Equals("ConnectedPeers", StringComparison.OrdinalIgnoreCase))
                    {
                        LoggerService.LogMessage("==================================================");
                        LoggerService.LogMessage($"Connected Peers ({RemoteDaemonCollectionService.RemoteDaemonRpcClients.Count}):\n{string.Join('\n', RemoteDaemonCollectionService.RemoteDaemonRpcClients.Keys)}");
                        LoggerService.LogMessage("==================================================");
                        continue;
                    }

                    LoggerService.LogMessage("Invalid command.", ConsoleColor.Red);
                }

                IsRunning = false;
            }, TaskCreationOptions.LongRunning);
        }

        public async Task RunCommandLoopAsync()
        {
            while (IsRunning)
                await Task.Delay(1).ConfigureAwait(false);
        }
    }
}