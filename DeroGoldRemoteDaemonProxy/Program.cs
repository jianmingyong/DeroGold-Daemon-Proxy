using System;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Bootstrap;
using DeroGoldRemoteDaemonProxy.Services.Console;
using DeroGoldRemoteDaemonProxy.Services.Daemon;
using DeroGoldRemoteDaemonProxy.Services.IO;
using Microsoft.Extensions.DependencyInjection;

namespace DeroGoldRemoteDaemonProxy
{
    public class Program
    {
        private DependencyInjectionManager DependencyInjectionManager { get; set; }

        public static void Main(string[] args)
        {
            var program = new Program();
            program.MainAsync(args).Wait();
        }

        private async Task MainAsync(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.BindInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<BootstrapService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<ConsoleCommandService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<DaemonProxyService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            DependencyInjectionManager = new DependencyInjectionManager(serviceProvider);
            DependencyInjectionManager.StartProgramLoop();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var consoleCommandService = serviceProvider.GetService<ConsoleCommandService>();
            await consoleCommandService.RunCommandLoopAsync().ConfigureAwait(false);
            
            DependencyInjectionManager.StopProgramLoop();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            DependencyInjectionManager.StopProgramLoop();
            Environment.Exit(0);
        }
    }
}