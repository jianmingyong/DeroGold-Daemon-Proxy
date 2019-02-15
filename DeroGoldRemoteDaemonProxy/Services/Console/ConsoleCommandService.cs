using System;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;

namespace DeroGoldRemoteDaemonProxy.Services.Console
{
    public class ConsoleCommandService : IInitializable
    {
        private LoggerService LoggerService { get; }

        private bool IsRunning { get; set; }

        public ConsoleCommandService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public void Initialize()
        {
            IsRunning = true;
        }

        public async Task RunCommandLoopAsync()
        {
            do
            {
                var input = await System.Console.In.ReadLineAsync().ConfigureAwait(false);

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    IsRunning = false;
                    continue;
                }

                LoggerService.LogMessage("Invalid command.", ConsoleColor.Red);
            } while (IsRunning);
        }
    }
}