using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DeroGoldDaemonProxy.Daemon;
using DeroGoldDaemonProxy.Services.Console;
using TheDialgaTeam.DependencyInjection;

namespace DeroGoldDaemonProxy.Services.Daemon
{
    public class DaemonProxyService : IInitializableAsync
    {
        private Dictionary<string, DaemonProxy> _daemonProxies;

        public Dictionary<string, DaemonProxy> DaemonProxies => new Dictionary<string, DaemonProxy>(_daemonProxies);

        private LoggerService LoggerService { get; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private List<string> PeersConnectedList { get; set; }

        public DaemonProxyService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public async Task InitializeAsync()
        {
            _daemonProxies = new Dictionary<string, DaemonProxy>();
            CancellationTokenSource = new CancellationTokenSource();
            PeersConnectedList = new List<string>();

            //await Task.Factory.StartNew(async () =>
            //{
            //    do
            //    {
            //        foreach (var daemonProxy in DaemonProxies)
            //        {
            //            if (!daemonProxy.Value.IsRunning && daemonProxy.Value.IsErrored)
            //            {
            //                RemoveListener(daemonProxy.Value);
            //                daemonProxy.Value.Dispose();
            //                _daemonProxies.Remove(daemonProxy.Key);
            //                continue;
            //            }

            //            if (daemonProxy.Value.IsReady && !daemonProxy.Value.IsBusy)
            //            {
            //                try
            //                {
            //                    if (!daemonProxy.Value.IsPeerDiscovered)
            //                    {
            //                        var peers = await daemonProxy.Value.GetPeersAsync();

            //                        foreach (var peer in peers.Peers)
            //                            await AddDaemonProxyAsync(peer.Remove(peer.IndexOf(':')), 6969);

            //                        foreach (var peer in peers.GrayPeers)
            //                            await AddDaemonProxyAsync(peer.Remove(peer.IndexOf(':')), 6969);

            //                        daemonProxy.Value.IsPeerDiscovered = true;
            //                    }
            //                }
            //                catch (Exception)
            //                {
            //                    await LoggerService.LogMessageAsync("Unable to get the peers from the daemon.", ConsoleColor.Red).ConfigureAwait(false);
            //                }
            //            }
            //        }

            //        await Task.Delay(1000).ConfigureAwait(false);
            //    } while (!CancellationTokenSource.IsCancellationRequested);
            //}, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
        }

        public async Task AddDaemonProxyAsync(string host, ushort port)
        {
            if (PeersConnectedList.Contains(host))
                return;

            var ipAddress = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
            var daemonProxy = new DaemonProxy(ipAddress.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? host, port);

            await daemonProxy.StartProxyAsync().ConfigureAwait(false);

            _daemonProxies.Add(host, daemonProxy);
            PeersConnectedList.Add(host);
        }
    }
}