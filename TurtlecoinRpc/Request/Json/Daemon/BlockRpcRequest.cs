using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class BlockRpcRequest
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}