using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TurtlecoinRpc;
using TurtlecoinRpc.Request.Http;
using TurtlecoinRpc.Response.Json;
using TurtlecoinRpc.Response.Json.Daemon;

namespace DeroGoldRemoteDaemonProxy.Daemon
{
    public enum DaemonConnectionStatus
    {
        Disconnected = 0,

        Disconnecting = 1,

        Connecting = 2,

        Connected = 3
    }

    public class DaemonProxy : IDisposable
    {
        public event Action<DaemonProxy, string, ConsoleColor> Log;

        public string Host { get; }

        public ushort Port { get; }

        public bool IsSynced { get; private set; }

        public bool IsReady { get; private set; }

        public DaemonConnectionStatus ConnectionStatus { get; private set; }

        private DaemonRpc DaemonRpc { get; }

        public DaemonProxy(string host, ushort port, HttpRpcRequestOptions httpRpcRequestOptions = null)
        {
            Host = host;
            Port = port;
            DaemonRpc = new DaemonRpc(host, port, httpRpcRequestOptions);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DaemonRpc?.Dispose();
        }

        public async Task StartProxyAsync()
        {
            if (ConnectionStatus == DaemonConnectionStatus.Connected || ConnectionStatus == DaemonConnectionStatus.Connecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Connecting;
            IsSynced = false;

            Log?.Invoke(this, "Attempting to connect this remote daemon.", ConsoleColor.White);

            try
            {
                await DaemonRpc.GetInfoAsync().ConfigureAwait(false);

                await Task.Factory.StartNew(async () =>
                {
                    var retryCount = 0;

                    do
                    {
                        try
                        {
                            var info = await GetInfoAsync().ConfigureAwait(false);

                            if (info.Synced)
                            {
                                if (!IsSynced)
                                {
                                    IsSynced = true;
                                    Log?.Invoke(this, "This remote daemon is synced and ready to rock!", ConsoleColor.Green);
                                }
                            }
                            else
                            {
                                IsSynced = false;
                                Log?.Invoke(this, $"This remote daemon is syncing with the network. ({Convert.ToDouble(info.Height) / Convert.ToDouble(info.NetworkHeight) * 100:F}% completed) They are {info.NetworkHeight - info.Height} blocks behind. Slow and steady wins the race!", ConsoleColor.Yellow);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            retryCount++;
                            IsSynced = false;
                            Log?.Invoke(this, "Unable to get a response from the daemon. This daemon may be stuck.", ConsoleColor.Yellow);
                        }
                        catch (HttpRequestException)
                        {
                            retryCount++;
                            IsSynced = false;
                            Log?.Invoke(this, "Lost connection to the remote daemon. Retry in 10 seconds.", ConsoleColor.Red);
                        }
                        finally
                        {
                            await Task.Delay(10000).ConfigureAwait(false);
                        }

                        if (retryCount < 10)
                            continue;

                        Log?.Invoke(this, "Retry count exceeded. Disconnecting from this remote daemon.", ConsoleColor.Red);
                        StopProxy();
                    } while (ConnectionStatus == DaemonConnectionStatus.Connected);
                }, TaskCreationOptions.LongRunning).ConfigureAwait(false);

                ConnectionStatus = DaemonConnectionStatus.Connected;

                Log?.Invoke(this, "This remote daemon have been connected!", ConsoleColor.Green);
            }
            catch (OperationCanceledException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
                Log?.Invoke(this, "Unable to connect to this remote daemon!", ConsoleColor.Red);
            }
            catch (HttpRequestException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
                Log?.Invoke(this, "Unable to connect to this remote daemon!", ConsoleColor.Red);
            }
        }

        public void StopProxy()
        {
            if (ConnectionStatus == DaemonConnectionStatus.Disconnected || ConnectionStatus == DaemonConnectionStatus.Disconnecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Disconnecting;

            Log?.Invoke(this, "This remote daemon have been disconnected.", ConsoleColor.Green);

            ConnectionStatus = DaemonConnectionStatus.Disconnected;
        }

        /// <summary>
        /// Get the height of the daemon and the network.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<HeightRpcResponse> GetHeightAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetHeightAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get information related to the network and daemon connection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<InfoRpcResponse> GetInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetInfoAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get list of missed transactions.
        /// </summary>
        /// <param name="transactionsHashes"></param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<TransactionsRpcResponse> GetTransactionsAsync(string[] transactionsHashes = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetTransactionsAsync(transactionsHashes, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the list of peers connected to the daemon.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<PeersRpcResponse> GetPeersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetPeersAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get information about the fee set for the remote node.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<FeeInfoRpcResponse> GetFeeInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetFeeInfoAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the current chain height.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockCountRpcResponse>> GetBlockCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockCountAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the block hash for a given height off by one.
        /// </summary>
        /// <param name="height">The height of the block whose previous hash is to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<string>> GetBlockHashAsync(ulong height, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockHashAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the block template with an empty "hole" for nonce.
        /// </summary>
        /// <param name="reserveSize">Size of the reserve to be specified.</param>
        /// <param name="walletAddress">Valid TurtleCoin wallet address.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockTemplateRpcResponse>> GetBlockTemplateAsync(ulong reserveSize, string walletAddress, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockTemplateAsync(reserveSize, walletAddress, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Submits mined block.
        /// </summary>
        /// <param name="blockBlob">Block blob of the mined block.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<StatusRpcResponse>> SubmitBlockAsync(string blockBlob, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.SubmitBlockAsync(blockBlob, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the last block header.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<JsonRpcResponse<LastBlockHeaderRpcResponse>> GetLastBlockHeaderAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetLastBlockHeaderAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the block header by given block hash.
        /// </summary>
        /// <param name="hash">Hash of the block.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockHeaderByHashRpcResponse>> GetBlockHeaderByHashAsync(string hash, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockHeaderByHashAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the block header by given block height.
        /// </summary>
        /// <param name="height">Height of the block.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockHeaderByHeightRpcResponse>> GetBlockHeaderByHeightAsync(ulong height, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockHeaderByHeightAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the unique currency identifier.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<CurrencyIdRpcResponse>> GetCurrencyIdAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetCurrencyIdAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the information on the last 30 blocks from height. (inclusive)
        /// </summary>
        /// <param name="height">Height of the last block to be included in the result.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>Requires blockchain explorer RPC.</remarks>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlocksRpcResponse>> GetBlocksAsync(ulong height, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlocksAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the information on a single block.
        /// </summary>
        /// <param name="hash">Hash of the block.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>Requires blockchain explorer RPC.</remarks>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockRpcResponse>> GetBlockAsync(string hash, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetBlockAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the information on single transaction.
        /// </summary>
        /// <param name="hash">Hash of the transaction.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>Requires blockchain explorer RPC.</remarks>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<TransactionRpcResponse>> GetTransactionAsync(string hash, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetTransactionAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }

        /// <summary>
        /// Get the list of transaction hashes present in memory pool.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>Requires blockchain explorer RPC.</remarks>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<TransactionPoolRpcResponse>> GetTransactionPoolAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IsReady = false;
                return await DaemonRpc.GetTransactionPoolAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                IsReady = true;
            }
        }
    }
}