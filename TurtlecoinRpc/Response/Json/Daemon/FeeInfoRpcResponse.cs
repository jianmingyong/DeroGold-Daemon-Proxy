using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class FeeInfoRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the address to which the fee is paid.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Get the fee amount.
        /// </summary>
        [JsonProperty("amount")]
        public ulong Amount { get; set; }
    }
}