using System;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Console;
using DeroGoldRemoteDaemonProxy.Services.Daemon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using Nancy.Owin;
using Nancy.TinyIoc;

namespace DeroGoldRemoteDaemonProxy.Services.Nancy
{
    public class RestWebService : IInitializable, IDisposable
    {
        private Program Program { get; }

        private IWebHost WebHost { get; set; }

        public RestWebService(Program program)
        {
            Program = program;
        }

        public void Initialize()
        {
            Task.Run(() => StartAsync());
        }

        public async Task StartAsync(ushort port = 6969)
        {
            WebHost?.Dispose();
            WebHost = new WebHostBuilder()
                .UseContentRoot(Environment.CurrentDirectory)
                .UseKestrel()
                .ConfigureServices(a =>
                {
                    a.AddSingleton(Program.ServiceProvider.GetService<LoggerService>());
                    a.AddSingleton(Program.ServiceProvider.GetService<RemoteDaemonCollectionService>());
                })
                .UseStartup<Startup>()
                .UseUrls($"http://*:{port}")
                .Build();

            await WebHost.StartAsync();
        }

        public async Task StopAsync()
        {
            if (WebHost != null)
                await WebHost.StopAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            WebHost?.Dispose();
        }
    }

    internal sealed class Startup
    {
        private RemoteDaemonCollectionService RemoteDaemonCollectionService { get; }

        private LoggerService LoggerService { get; }

        public Startup(RemoteDaemonCollectionService remoteDaemonCollectionService, LoggerService loggerService)
        {
            RemoteDaemonCollectionService = remoteDaemonCollectionService;
            LoggerService = loggerService;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(a => a.Bootstrapper = new Bootstrapper(RemoteDaemonCollectionService, LoggerService)));
        }
    }

    internal sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        private RemoteDaemonCollectionService RemoteDaemonCollectionService { get; }

        private LoggerService LoggerService { get; }

        public Bootstrapper(RemoteDaemonCollectionService remoteDaemonCollectionService, LoggerService loggerService)
        {
            RemoteDaemonCollectionService = remoteDaemonCollectionService;
            LoggerService = loggerService;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(RemoteDaemonCollectionService);
            container.Register(LoggerService);
        }
    }
}