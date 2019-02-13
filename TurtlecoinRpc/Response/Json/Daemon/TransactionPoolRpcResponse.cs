using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.Transaction;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class TransactionPoolRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the array of transactions in memory pool.
        /// </summary>
        [JsonProperty("transactions")]
        public TransactionShortRpcResponse[] Transactions { get; set; }
    }
}