using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TurtlecoinRpc;
using TurtlecoinRpc.Request.Http;
using TurtlecoinRpc.Response.Json;
using TurtlecoinRpc.Response.Json.Daemon;

namespace DeroGoldDaemonProxy.Daemon
{
    public enum DaemonConnectionStatus
    {
        Disconnected = 0,

        Disconnecting = 1,

        Connecting = 2,

        Connected = 3
    }

    public enum DaemonReadyStatus
    {
        Ready = 0,

        Busy = 1
    }

    public class DaemonProxy : IDisposable
    {
        public DaemonConnectionStatus ConnectionStatus { get; private set; }

        public DaemonReadyStatus ReadyStatus { get; private set; }

        private DaemonRpc DaemonRpc { get; }

        public DaemonProxy(string host, ushort port, HttpRpcRequestOptions httpRpcRequestOptions = null)
        {
            DaemonRpc = new DaemonRpc(host, port, httpRpcRequestOptions);
        }

        public async Task StartProxyAsync()
        {
            if (ConnectionStatus == DaemonConnectionStatus.Connected || ConnectionStatus == DaemonConnectionStatus.Connecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Connecting;

            try
            {
                await DaemonRpc.GetInfoAsync().ConfigureAwait(false);
                ConnectionStatus = DaemonConnectionStatus.Connected;
            }
            catch (OperationCanceledException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
            }
            catch (HttpRequestException)
            {
                ConnectionStatus = DaemonConnectionStatus.Disconnected;
            }
        }

        public void StopProxy()
        {
            if (ConnectionStatus == DaemonConnectionStatus.Disconnected || ConnectionStatus == DaemonConnectionStatus.Disconnecting)
                return;

            ConnectionStatus = DaemonConnectionStatus.Disconnecting;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetHeightAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetInfoAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetTransactionsAsync(transactionsHashes, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetPeersAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetFeeInfoAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockCountAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockHashAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockTemplateAsync(reserveSize, walletAddress, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.SubmitBlockAsync(blockBlob, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetLastBlockHeaderAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockHeaderByHashAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockHeaderByHeightAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetCurrencyIdAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlocksAsync(height, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetBlockAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetTransactionAsync(hash, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
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
                ReadyStatus = DaemonReadyStatus.Busy;
                return await DaemonRpc.GetTransactionPoolAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ReadyStatus = DaemonReadyStatus.Ready;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DaemonRpc?.Dispose();
        }
    }
}