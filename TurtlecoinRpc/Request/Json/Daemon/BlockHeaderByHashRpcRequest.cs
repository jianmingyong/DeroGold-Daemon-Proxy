using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class BlockHeaderByHashRpcRequest
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}