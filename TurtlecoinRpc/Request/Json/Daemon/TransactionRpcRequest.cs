using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class TransactionRpcRequest
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}