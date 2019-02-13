using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class TransactionDetailedRpcResponse : TransactionShortRpcResponse
    {
        /// <summary>
        /// Get the payment Id of the transaction.
        /// </summary>
        [JsonProperty("paymentId")]
        public string PaymentId { get; set; }

        /// <summary>
        /// Get the mixin of the transaction.
        /// </summary>
        [JsonProperty("mixin")]
        public ulong Mixin { get; set; }
    }
}