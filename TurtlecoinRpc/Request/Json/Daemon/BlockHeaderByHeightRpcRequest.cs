using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class BlockHeaderByHeightRpcRequest
    {
        [JsonProperty("height")]
        public ulong Height { get; set; }
    }
}