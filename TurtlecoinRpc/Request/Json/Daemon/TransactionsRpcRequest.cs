using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class TransactionsRpcRequest
    {
        [JsonProperty("txs_hashes")]
        public string[] TransactionsHashes { get; set; }
    }
}