using System;
using System.Net.Http;
using System.Threading.Tasks;
using TurtlecoinRpc.Request.Http;

namespace DeroGoldRemoteDaemonProxy.Daemon
{
    public enum DaemonConnectionStatus
    {
        Disconnected = 0,

        Disconnecting = 1,

        Connecting = 2,

        Connected = 3
    }

    public class RemoteDaemonRpcClient : TurtlecoinRpc.RemoteDaemonRpcClient
    {
        public event Action<RemoteDaemonRpcClient, string, ConsoleColor> Log;

        private const string Connecting = "Connecting to this remote daemon...";

        private const string Disconnected = "Stopped listening to this remote daemon.";

        private const string ConnectError = "Unable to connect to this remote daemon.";

        private const string ConnectSuccess = "This remote daemon is now connected and ready to listen to request.";

        private const string ConnectionLost = "Lost connection to the remote daemon. Retry in 10 seconds.";

        private const string RequestTimeout = "Unable to get a response from the remote daemon. This remote daemon may be stuck.";

        private const string RetryExceeded = "Retry count exceeded. Stop listening from this remote daemon.";

        private const string Synced = "This remote daemon is synced and ready to rock!";

        public string Host { get; }

        public ushort Port { get; }

        public bool IsSynced { get; private set; }

        public DaemonConnectionStatus ConnectionStatus { get; private set; }

        private bool IsRunning { get; set; }

        public RemoteDaemonRpcClient(string host, ushort port, HttpRpcRequestOptions httpRpcRequestOptions = null) : base(host, port, httpRpcRequestOptions)
        {
            Host = host;
            Port = port;
        }

        public new void Dispose()
        {
            StopListeningAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            base.Dispose();
        }

        public async Task StartListeningAsync()
        {
            if (ConnectionStatus == DaemonConnectionStatus.Connected || ConnectionStatus == DaemonConnectionStatus.Connecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Connecting;

            Log?.Invoke(this, Connecting, ConsoleColor.White);

            try
            {
                await CheckSyncStatusAsync().ConfigureAwait(false);

                await Task.Factory.StartNew(async () =>
                {
                    IsRunning = true;

                    var retryCount = 0;

                    do
                    {
                        try
                        {
                            await CheckSyncStatusAsync().ConfigureAwait(false);
                            retryCount = 0;
                        }
                        catch (OperationCanceledException)
                        {
                            retryCount++;
                            IsSynced = false;
                            Log?.Invoke(this, RequestTimeout, ConsoleColor.Yellow);
                        }
                        catch (HttpRequestException)
                        {
                            retryCount++;
                            IsSynced = false;
                            Log?.Invoke(this, ConnectionLost, ConsoleColor.Red);
                        }
                        catch (Exception)
                        {
                            IsRunning = false;
                            return;
                        }
                        finally
                        {
                            await Task.Delay(10000).ConfigureAwait(false);
                        }

                        if (retryCount < 10)
                            continue;

                        Log?.Invoke(this, RetryExceeded, ConsoleColor.Red);
                        await StopListeningAsync(false).ConfigureAwait(false);
                    } while (ConnectionStatus == DaemonConnectionStatus.Connected);

                    IsRunning = false;
                }, TaskCreationOptions.LongRunning).ConfigureAwait(false);

                ConnectionStatus = DaemonConnectionStatus.Connected;

                Log?.Invoke(this, ConnectSuccess, ConsoleColor.Green);
            }
            catch (OperationCanceledException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
                Log?.Invoke(this, ConnectError, ConsoleColor.Red);
            }
            catch (HttpRequestException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
                Log?.Invoke(this, ConnectError, ConsoleColor.Red);
            }
        }

        public async Task StopListeningAsync(bool waitForThreadStop = true)
        {
            if (ConnectionStatus == DaemonConnectionStatus.Disconnected || ConnectionStatus == DaemonConnectionStatus.Disconnecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Disconnecting;

            if (waitForThreadStop)
            {
                await Task.Run(async () =>
                {
                    while (IsRunning)
                        await Task.Delay(1).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }

            Log?.Invoke(this, Disconnected, ConsoleColor.Green);

            ConnectionStatus = DaemonConnectionStatus.Disconnected;
        }

        private async Task CheckSyncStatusAsync()
        {
            var info = await GetInfoAsync().ConfigureAwait(false);

            if (info.Synced)
            {
                if (!IsSynced)
                {
                    IsSynced = true;
                    Log?.Invoke(this, Synced, ConsoleColor.Green);
                }
            }
            else
            {
                IsSynced = false;
                Log?.Invoke(this, $"This remote daemon is syncing with the network. ({Math.Truncate(Convert.ToDouble(info.Height) / Convert.ToDouble(info.NetworkHeight) * 100 * 100) / 100:F}% completed) They are {info.NetworkHeight - info.Height} blocks behind. Slow and steady wins the race!", ConsoleColor.Yellow);
            }
        }
    }
}