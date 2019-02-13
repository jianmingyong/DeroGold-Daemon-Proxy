using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.BlockHeader;
using TurtlecoinRpc.Response.Json.Daemon.Transaction;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class TransactionRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the details of the transaction.
        /// </summary>
        [JsonProperty("txDetails")]
        public TransactionDetailedRpcResponse TransactionDetails { get; set; }

        /// <summary>
        /// Get the details of the block in which transaction is present.
        /// </summary>
        [JsonProperty("block")]
        public BlockHeaderShortRpcResponse Block { get; set; }
    }
}