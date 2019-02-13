using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class TransactionShortRpcResponse
    {
        /// <summary>
        /// Get the hash of the transaction.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Get the fees for the transaction.
        /// </summary>
        [JsonProperty("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Get the output amount of the transaction.
        /// </summary>
        [JsonProperty("amount_out")]
        public ulong AmountOut { get; set; }

        /// <summary>
        /// Get the size of the transaction.
        /// </summary>
        [JsonProperty("size")]
        public ulong Size { get; set; }
    }
}