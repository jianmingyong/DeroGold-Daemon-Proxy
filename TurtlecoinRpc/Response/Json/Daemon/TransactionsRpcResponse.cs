using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class TransactionsRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the array of hex values of missed transactions.
        /// </summary>
        [JsonProperty("txs_as_hex")]
        public string[] TransactionsAsHex { get; set; }

        /// <summary>
        /// Get the array of missed transactions.
        /// </summary>
        [JsonProperty("missed_tx")]
        public string[] MissedTransaction { get; set; }
    }
}