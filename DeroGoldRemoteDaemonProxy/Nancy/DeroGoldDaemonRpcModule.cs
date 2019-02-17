using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.Services.Console;
using DeroGoldRemoteDaemonProxy.Services.Daemon;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using TurtlecoinRpc;
using TurtlecoinRpc.Request.Json;
using TurtlecoinRpc.Request.Json.Daemon;
using TurtlecoinRpc.Response.Json;
using TurtlecoinRpc.Response.Json.Daemon;

namespace DeroGoldRemoteDaemonProxy.Nancy
{
    public sealed class DeroGoldDaemonRpcModule : NancyModule
    {
        private RemoteDaemonCollectionService RemoteDaemonCollectionService { get; }

        private LoggerService LoggerService { get; }

        public DeroGoldDaemonRpcModule(RemoteDaemonCollectionService remoteDaemonCollectionService, LoggerService loggerService)
        {
            RemoteDaemonCollectionService = remoteDaemonCollectionService;
            LoggerService = loggerService;

            Before += context =>
            {
                loggerService.LogMessage($"Incoming RPC Request: {context.Request.Path}");
                return null;
            };

            GetAndPost("/getheight", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetHeightAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/height", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetHeightAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/getinfo", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetInfoAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/info", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetInfoAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/gettransactions", async args => await RunRpcTaskAsync<TransactionsRpcResponse, TransactionsRpcRequest>((remoteDaemonRpcClient, cancellationToken, request) => remoteDaemonRpcClient.GetTransactionsAsync(request?.TransactionsHashes, cancellationToken)).ConfigureAwait(false));

            GetAndPost("/getpeers", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetPeersAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/peers", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetPeersAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/feeinfo", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetFeeInfoAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/fee", async args => await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetFeeInfoAsync(cancellationToken)).ConfigureAwait(false));

            GetAndPost("/json_rpc", async args => await RunJsonRpcTaskAsync().ConfigureAwait(false));

            GetAndPost("/get_pool_changes_lite", async args => await RunRpcTaskAsync<PoolChangesLiteRpcResponse, PoolChangesLiteRpcRequest>((remoteDaemonRpcClient, cancellationToken, request) => remoteDaemonRpcClient.GetPoolChangesLiteAsync(request?.TailBlockId, request?.KnownTransactionsIds)).ConfigureAwait(false));
        }

        private void GetAndPost(string path, Func<dynamic, Task<Response>> action)
        {
            Get(path, action);
            Post(path, action);
        }

        private async Task<Response> RunRpcTaskAsync<TResponse>(Func<RemoteDaemonRpcClient, CancellationToken, Task<TResponse>> func) where TResponse : class
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = new List<Task>();

            foreach (var (_, remoteDaemonRpcClient) in RemoteDaemonCollectionService.RemoteDaemonRpcClients)
            {
                if (remoteDaemonRpcClient.IsSynced)
                    tasks.Add(func(remoteDaemonRpcClient, cancellationTokenSource.Token));
            }

            if (tasks.Count == 0)
                return new Response { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = "All remote daemon is not ready or synced." };

            tasks.Add(Task.Delay(TimeSpan.FromSeconds(1000), cancellationTokenSource.Token));

            var task = await Task.WhenAny(tasks).ConfigureAwait(false);

            cancellationTokenSource.Cancel();

            if (!(task is Task<TResponse> response))
                return new Response { StatusCode = HttpStatusCode.RequestTimeout };

            var result = await response.ConfigureAwait(false);
            return Response.AsText(JsonConvert.SerializeObject(result), "application/json");
        }

        private async Task<Response> RunRpcTaskAsync<TResponse, TRequest>(Func<RemoteDaemonRpcClient, CancellationToken, TRequest, Task<TResponse>> func) where TResponse : class
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var tasks = new List<Task>();
            TRequest request;

            using (var streamReader = new StreamReader(Request.Body))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    request = serializer.Deserialize<TRequest>(jsonReader);
                }
            }

            foreach (var (_, remoteDaemonRpcClient) in RemoteDaemonCollectionService.RemoteDaemonRpcClients)
            {
                if (remoteDaemonRpcClient.IsSynced)
                    tasks.Add(func(remoteDaemonRpcClient, cancellationTokenSource.Token, request));
            }

            if (tasks.Count == 0)
                return new Response { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = "All remote daemon is not ready or synced." };

            tasks.Add(Task.Delay(TimeSpan.FromSeconds(1000), cancellationTokenSource.Token));

            var task = await Task.WhenAny(tasks).ConfigureAwait(false);

            cancellationTokenSource.Cancel();

            if (!(task is Task<TResponse> response))
                return new Response { StatusCode = HttpStatusCode.RequestTimeout };

            var result = await response.ConfigureAwait(false);
            return Response.AsText(JsonConvert.SerializeObject(result), "application/json");
        }

        private async Task<Response> RunJsonRpcTaskAsync()
        {
            JsonRpcRequest<object> request;

            using (var streamReader = new StreamReader(Request.Body))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    request = serializer.Deserialize<JsonRpcRequest<object>>(jsonReader);
                }
            }

            LoggerService.LogMessage($"Incoming RPC Request: {request.Method}");

            if (request.Method.Equals("getblockcount", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockCountAsync(cancellationToken)).ConfigureAwait(false);

            if (request.Method.Equals("on_getblockhash", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockHashAsync(((ulong[]) request.Parameters)[0], cancellationToken)).ConfigureAwait(false);

            if (request.Method.Equals("getblocktemplate", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<BlockTemplateRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockTemplateAsync(typedRequest.Parameters.ReserveSize, typedRequest.Parameters.WalletAddress, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("submitblock", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.SubmitBlockAsync(((string[]) request.Parameters)[0], cancellationToken)).ConfigureAwait(false);

            if (request.Method.Equals("getlastblockheader", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetLastBlockHeaderAsync(cancellationToken)).ConfigureAwait(false);

            if (request.Method.Equals("getblockheaderbyhash", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<BlockHeaderByHashRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockHeaderByHashAsync(typedRequest.Parameters.Hash, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("getblockheaderbyheight", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<BlockHeaderByHeightRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockHeaderByHeightAsync(typedRequest.Parameters.Height, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("getcurrencyid", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetCurrencyIdAsync(cancellationToken)).ConfigureAwait(false);

            if (request.Method.Equals("f_blocks_list_json", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<BlocksRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlocksAsync(typedRequest.Parameters.Height, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("f_block_json", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<BlockRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetBlockAsync(typedRequest.Parameters.Hash, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("f_transaction_json", StringComparison.OrdinalIgnoreCase))
            {
                using (var streamReader = new StreamReader(Request.Body))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        var typedRequest = serializer.Deserialize<JsonRpcRequest<TransactionRpcRequest>>(jsonReader);

                        return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetTransactionAsync(typedRequest.Parameters.Hash, cancellationToken)).ConfigureAwait(false);
                    }
                }
            }

            if (request.Method.Equals("f_on_transactions_pool_json", StringComparison.OrdinalIgnoreCase))
                return await RunRpcTaskAsync((remoteDaemonRpcClient, cancellationToken) => remoteDaemonRpcClient.GetTransactionPoolAsync(cancellationToken)).ConfigureAwait(false);


            return Response.AsText(JsonConvert.SerializeObject(new JsonRpcResponse<string> { Id = request.Id, JsonRpc = request.JsonRpc, Error = new ErrorRpcResponse { Code = -32601, Message = "Method not found" } }), "application/json");
        }
    }
}