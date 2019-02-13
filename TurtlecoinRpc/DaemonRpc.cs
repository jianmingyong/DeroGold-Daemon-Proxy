using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TurtlecoinRpc.Request.Http;
using TurtlecoinRpc.Request.Http.Daemon;
using TurtlecoinRpc.Request.Json.Daemon;
using TurtlecoinRpc.Response.Json;
using TurtlecoinRpc.Response.Json.Daemon;

namespace TurtlecoinRpc
{
    public class DaemonRpc : IDisposable
    {
        internal HttpRpcRequestOptions HttpRpcRequestOptions { get; }

        internal HttpClient HttpClient { get; }

        public DaemonRpc(string host, ushort port, HttpRpcRequestOptions httpRpcRequestOptions = null)
        {
            HttpRpcRequestOptions = httpRpcRequestOptions ?? new HttpRpcRequestOptions();
            HttpClient = new HttpClient { BaseAddress = new Uri($"http://{host}:{port}/"), Timeout = TimeSpan.FromMilliseconds(-1) };
        }

        /// <summary>
        /// Get the height of the daemon and the network.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<HeightRpcResponse> GetHeightAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new Request.Http.Daemon.HttpGetRpcRequest<HeightRpcResponse>(this, HttpRpcRequestOptions.UseLegacyEndpoints ? "getheight" : "height");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get information related to the network and daemon connection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<InfoRpcResponse> GetInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new Request.Http.Daemon.HttpGetRpcRequest<InfoRpcResponse>(this, HttpRpcRequestOptions.UseLegacyEndpoints ? "getinfo" : "info");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new Request.Http.Daemon.HttpPostRpcRequest<TransactionsRpcResponse, TransactionsRpcRequest>(this, "gettransactions", new TransactionsRpcRequest { TransactionsHashes = transactionsHashes ?? new string[0] });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the list of peers connected to the daemon.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<PeersRpcResponse> GetPeersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new Request.Http.Daemon.HttpGetRpcRequest<PeersRpcResponse>(this, HttpRpcRequestOptions.UseLegacyEndpoints ? "getpeers" : "peers");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get information about the fee set for the remote node.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<FeeInfoRpcResponse> GetFeeInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new Request.Http.Daemon.HttpGetRpcRequest<FeeInfoRpcResponse>(this, HttpRpcRequestOptions.UseLegacyEndpoints ? "feeinfo" : "fee");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the current chain height.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<BlockCountRpcResponse>> GetBlockCountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpJsonRpcRequest<BlockCountRpcResponse>(this, "getblockcount");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<string, ulong[]>(this, "on_getblockhash", new[] { height });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<BlockTemplateRpcResponse, BlockTemplateRpcRequest>(this, "getblocktemplate", new BlockTemplateRpcRequest { ReserveSize = reserveSize, WalletAddress = walletAddress });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<StatusRpcResponse, string[]>(this, "submitblock", new[] { blockBlob });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the last block header.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<JsonRpcResponse<LastBlockHeaderRpcResponse>> GetLastBlockHeaderAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpJsonRpcRequest<LastBlockHeaderRpcResponse>(this, "getlastblockheader");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<BlockHeaderByHashRpcResponse, BlockHeaderByHashRpcRequest>(this, "getblockheaderbyhash", new BlockHeaderByHashRpcRequest { Hash = hash });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<BlockHeaderByHeightRpcResponse, BlockHeaderByHeightRpcRequest>(this, "getblockheaderbyheight", new BlockHeaderByHeightRpcRequest { Height = height });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the unique currency identifier.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure or server certificate validation.</exception>
        /// <exception cref="OperationCanceledException">The request has timed out.</exception>
        public async Task<JsonRpcResponse<CurrencyIdRpcResponse>> GetCurrencyIdAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new HttpJsonRpcRequest<CurrencyIdRpcResponse>(this, "getcurrencyid");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<BlocksRpcResponse, BlocksRpcRequest>(this, "f_blocks_list_json", new BlocksRpcRequest { Height = height });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<BlockRpcResponse, BlockRpcRequest>(this, "f_block_json", new BlockRpcRequest { Hash = hash });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<TransactionRpcResponse, TransactionRpcRequest>(this, "f_transaction_json", new TransactionRpcRequest { Hash = hash });
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
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
            var request = new HttpJsonRpcRequest<TransactionPoolRpcResponse>(this, "f_on_transactions_pool_json");
            return await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}