using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.Daemon;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Console;

namespace DeroGoldRemoteDaemonProxy.Services.Daemon
{
    public class DaemonProxyService : IInitializable
    {
        private Dictionary<string, DaemonProxy> _daemonProxies;

        public Dictionary<string, DaemonProxy> DaemonProxies => new Dictionary<string, DaemonProxy>(_daemonProxies);

        private LoggerService LoggerService { get; }

        private bool IsRunning { get; set; }

        public DaemonProxyService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public void Initialize()
        {
            _daemonProxies = new Dictionary<string, DaemonProxy>();

            AddDaemonProxy("97.64.253.98", 6969);
            AddDaemonProxy("51.255.209.200", 6969);
            AddDaemonProxy("23.96.93.180", 6969);
            AddDaemonProxy("5.172.219.172", 6969);

            Task.Factory.StartNew(async () =>
            {
                IsRunning = true;

                do
                {
                    foreach (var daemonProxy in DaemonProxies)
                    {
                        if (daemonProxy.Value.ConnectionStatus == DaemonConnectionStatus.Disconnected)
                        {
                            RemoveListener(daemonProxy.Value);
                            daemonProxy.Value.Dispose();
                            _daemonProxies.Remove(daemonProxy.Key);
                            continue;
                        }
                    }

                    await Task.Delay(1).ConfigureAwait(false);
                } while (IsRunning);

                IsRunning = false;
            }, TaskCreationOptions.LongRunning);
        }

        public void AddDaemonProxy(string host, ushort port)
        {
            Task.Run(async () => { await AddDaemonProxyAsync(host, port).ConfigureAwait(false); });
        }

        private async Task AddDaemonProxyAsync(string host, ushort port)
        {
            var ipAddresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
            var ipAddress = ipAddresses.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? host;

            if (_daemonProxies.ContainsKey(ipAddress))
                return;

            var daemonProxy = new DaemonProxy(ipAddress, port);
            AddListener(daemonProxy);
            await daemonProxy.StartProxyAsync().ConfigureAwait(false);

            if (daemonProxy.ConnectionStatus == DaemonConnectionStatus.Connected)
                _daemonProxies.Add(host, daemonProxy);
            else
            {
                RemoveListener(daemonProxy);
                daemonProxy.Dispose();
            }
        }

        private void AddListener(DaemonProxy daemonProxy)
        {
            daemonProxy.Log += DaemonProxyOnLog;
        }

        private void RemoveListener(DaemonProxy daemonProxy)
        {
            daemonProxy.Log -= DaemonProxyOnLog;
        }

        private void DaemonProxyOnLog(DaemonProxy daemonProxy, string message, ConsoleColor consoleColor)
        {
            LoggerService.LogMessage($"[{daemonProxy.Host}:{daemonProxy.Port}] {message}", consoleColor);
        }
    }
}