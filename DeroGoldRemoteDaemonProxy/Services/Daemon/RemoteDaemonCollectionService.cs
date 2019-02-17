using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.Daemon;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Console;

namespace DeroGoldRemoteDaemonProxy.Services.Daemon
{
    public class RemoteDaemonCollectionService : IInitializable, IDisposable
    {
        private Dictionary<string, RemoteDaemonRpcClient> _remoteDaemonRpcClients;

        public Dictionary<string, RemoteDaemonRpcClient> RemoteDaemonRpcClients => new Dictionary<string, RemoteDaemonRpcClient>(_remoteDaemonRpcClients);

        private LoggerService LoggerService { get; }

        private bool IsRunning { get; set; }

        private bool ExitRequested { get; set; }

        private Semaphore PeersConnectedListLock { get; set; }

        private List<string> PeersConnectedList { get; set; }

        public RemoteDaemonCollectionService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public void Initialize()
        {
            _remoteDaemonRpcClients = new Dictionary<string, RemoteDaemonRpcClient>();
            PeersConnectedListLock = new Semaphore(1, 1);
            PeersConnectedList = new List<string>();

            // Seed Nodes
            AddDaemonProxy("97.64.253.98", 6969);
            AddDaemonProxy("51.255.209.200", 6969);
            AddDaemonProxy("23.96.93.180", 6969);
            AddDaemonProxy("5.172.219.172", 6969);

            // Public Nodes
            AddDaemonProxy("dego.stx.nl", 6969);
            AddDaemonProxy("dego-stroppy.ddns.net", 6969);
            AddDaemonProxy("node-eu.dego.gq", 6969);
            AddDaemonProxy("185.17.27.105", 6969);
            AddDaemonProxy("publicnode.ydns.eu", 6969);

            // Public Blockchain Explorer Nodes
            AddDaemonProxy("explorer.dego.gq", 6969);
            AddDaemonProxy("pool.llama.horse", 42068);
            AddDaemonProxy("dego.pool.flowmine.xyz", 42065);

            Task.Factory.StartNew(async () =>
            {
                IsRunning = true;

                do
                {
                    foreach (var (key, value) in RemoteDaemonRpcClients)
                    {
                        if (value.ConnectionStatus == DaemonConnectionStatus.Disconnected)
                        {
                            RemoveListener(value);
                            value.Dispose();
                            _remoteDaemonRpcClients.Remove(key);
                        }
                    }

                    if (ExitRequested)
                    {
                        foreach (var (key, value) in RemoteDaemonRpcClients)
                        {
                            await value.StopListeningAsync().ConfigureAwait(false);
                            RemoveListener(value);
                            value.Dispose();
                            _remoteDaemonRpcClients.Remove(key);
                        }

                        break;
                    }

                    await Task.Delay(1).ConfigureAwait(false);
                } while (IsRunning);

                IsRunning = false;
            }, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            ExitRequested = true;

            Task.Run(async () =>
            {
                while (IsRunning)
                    await Task.Delay(1).ConfigureAwait(false);
            }).Wait();

            PeersConnectedListLock?.Dispose();
        }

        public void AddDaemonProxy(string host, ushort port)
        {
            Task.Factory.StartNew(async () => { await AddDaemonProxyAsync(host, port).ConfigureAwait(false); }, TaskCreationOptions.LongRunning);
        }

        public async Task AddDaemonProxyAsync(string host, ushort port)
        {
            var ipAddresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
            var ipAddress = ipAddresses.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? host;

            try
            {
                PeersConnectedListLock.WaitOne();

                if (PeersConnectedList.Contains(ipAddress))
                    return;

                PeersConnectedList.Add(ipAddress);
            }
            finally
            {
                PeersConnectedListLock.Release();
            }

            var remoteDaemonRpcClient = new RemoteDaemonRpcClient(ipAddress, port);
            AddListener(remoteDaemonRpcClient);
            await remoteDaemonRpcClient.StartListeningAsync().ConfigureAwait(false);

            if (remoteDaemonRpcClient.ConnectionStatus == DaemonConnectionStatus.Connected)
                _remoteDaemonRpcClients.Add(ipAddress, remoteDaemonRpcClient);
            else
            {
                PeersConnectedList.Remove(ipAddress);
                RemoveListener(remoteDaemonRpcClient);
                remoteDaemonRpcClient.Dispose();
            }
        }

        private void AddListener(RemoteDaemonRpcClient remoteDaemonRpcClient)
        {
            remoteDaemonRpcClient.Log += DaemonProxyOnLog;
        }

        private void RemoveListener(RemoteDaemonRpcClient remoteDaemonRpcClient)
        {
            remoteDaemonRpcClient.Log -= DaemonProxyOnLog;
        }

        private void DaemonProxyOnLog(RemoteDaemonRpcClient remoteDaemonRpcClient, string message, ConsoleColor consoleColor)
        {
            LoggerService.LogMessage($"[{remoteDaemonRpcClient.Host}:{remoteDaemonRpcClient.Port}] {message}", consoleColor);
        }
    }
}