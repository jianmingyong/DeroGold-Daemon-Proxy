using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class OutputTransactionRpcResponse
    {
        [JsonProperty("amount")]
        public ulong Amount { get; set; }

        [JsonProperty("target")]
        public OutputTargetRpcResponse Target { get; set; }
    }
}