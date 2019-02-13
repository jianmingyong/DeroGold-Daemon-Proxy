using System;
using System.Threading.Tasks;
using DeroGoldDaemonProxy.Services.Bootstrap;
using DeroGoldDaemonProxy.Services.Console;
using DeroGoldDaemonProxy.Services.Daemon;
using DeroGoldDaemonProxy.Services.IO;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.DependencyInjection;

namespace DeroGoldDaemonProxy
{
    public class Program
    {
        private DependencyInjectionManager DependencyInjectionManager { get; set; }

        private LoggerService LoggerService { get; set; }

        private FilePathService FilePathService { get; set; }

        private DaemonProxyService DaemonProxyService { get; set; }

        private bool IsRunning { get; set; } = true;

        public static void Main(string[] args)
        {
            var program = new Program();
            program.MainAsync(args).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            Console.Title = "DeroGold Daemon Proxy (.Net Core)";

            var serviceCollection = new ServiceCollection();
            serviceCollection.BindInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<BootstrapService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<DaemonProxyService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            DependencyInjectionManager = new DependencyInjectionManager(serviceProvider);
            await DependencyInjectionManager.StartProgramLoopAsync().ConfigureAwait(false);

            LoggerService = serviceProvider.GetService<LoggerService>();
            FilePathService = serviceProvider.GetService<FilePathService>();
            DaemonProxyService = serviceProvider.GetService<DaemonProxyService>();

            await DaemonProxyService.AddDaemonProxyAsync("97.64.253.98", 6969).ConfigureAwait(false);
            await DaemonProxyService.AddDaemonProxyAsync("51.255.209.200", 6969).ConfigureAwait(false);
            //await DaemonProxyService.AddDaemonProxyAsync("23.96.93.180", 6969).ConfigureAwait(false);
            await DaemonProxyService.AddDaemonProxyAsync("185.3.94.76", 6969).ConfigureAwait(false);
            await DaemonProxyService.AddDaemonProxyAsync("5.172.219.172", 6969).ConfigureAwait(false);

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            do
            {
                var commandInput = await Console.In.ReadLineAsync().ConfigureAwait(false);
                commandInput = commandInput?.Trim();

                if (commandInput == null)
                    break;

                if (commandInput.Equals("PeersConnected", StringComparison.OrdinalIgnoreCase))
                {
                    await LoggerService.LogMessageAsync($"There are {DaemonProxyService.DaemonProxies.Count} connected peers.", ConsoleColor.Green).ConfigureAwait(false);
                    await LoggerService.LogMessageAsync($"{string.Join('\n', DaemonProxyService.DaemonProxies.Keys)}").ConfigureAwait(false);
                    continue;
                }

                await LoggerService.LogMessageAsync("Unknown command. Please try again.", ConsoleColor.Red).ConfigureAwait(false);
            } while (IsRunning);

            await DependencyInjectionManager.StopProgramLoopAsync().ConfigureAwait(false);
        }

        private async void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            IsRunning = false;

            await DependencyInjectionManager.StopProgramLoopAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }
    }
}