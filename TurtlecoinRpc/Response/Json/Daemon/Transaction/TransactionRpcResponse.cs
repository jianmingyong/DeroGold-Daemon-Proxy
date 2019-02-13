using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class TransactionRpcResponse
    {
        /// <summary>
        /// Get the transaction extra which can be any information in hex.
        /// </summary>
        [JsonProperty("extra")]
        public string Extra { get; set; }

        /// <summary>
        /// Get the delay in unlocking the amount.
        /// </summary>
        [JsonProperty("unlock_time")]
        public ulong UnlockTime { get; set; }

        [JsonProperty("version")]
        public ulong Version { get; set; }

        /// <summary>
        /// Get the array of input transactions.
        /// </summary>
        [JsonProperty("vin")]
        public InputTransactionRpcResponse[] InputTransactions { get; set; }

        /// <summary>
        /// Get the array of output transactions.
        /// </summary>
        [JsonProperty("vout")]
        public OutputTransactionRpcResponse[] OutputTransactions { get; set; }
    }
}