using System;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Bootstrap;
using DeroGoldRemoteDaemonProxy.Services.Console;
using DeroGoldRemoteDaemonProxy.Services.Daemon;
using DeroGoldRemoteDaemonProxy.Services.IO;
using DeroGoldRemoteDaemonProxy.Services.Nancy;
using Microsoft.Extensions.DependencyInjection;

namespace DeroGoldRemoteDaemonProxy
{
    public class Program
    {
        public ServiceProvider ServiceProvider { get; private set; }

        private DependencyInjectionManager DependencyInjectionManager { get; set; }

        public static async Task Main(string[] args)
        {
            var program = new Program();
            await program.MainAsync(args).ConfigureAwait(false);
        }

        private async Task MainAsync(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(this);
            serviceCollection.BindInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<BootstrapService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<ConsoleCommandService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<RemoteDaemonCollectionService>();
            serviceCollection.BindInterfacesAndSelfAsSingleton<RestWebService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            DependencyInjectionManager = new DependencyInjectionManager(ServiceProvider);
            DependencyInjectionManager.StartProgramLoop();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var consoleCommandService = ServiceProvider.GetService<ConsoleCommandService>();
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