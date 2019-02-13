using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class BlockCountRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the current chain height.
        /// </summary>
        [JsonProperty("count")]
        public ulong Count { get; set; }
    }
}