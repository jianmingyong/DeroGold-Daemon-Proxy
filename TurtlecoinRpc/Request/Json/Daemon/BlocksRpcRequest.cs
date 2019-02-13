using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class BlocksRpcRequest
    {
        [JsonProperty("height")]
        public ulong Height { get; set; }
    }
}