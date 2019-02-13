using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class HeightRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the current daemon height.
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Get the current network height.
        /// </summary>
        [JsonProperty("network_height")]
        public ulong NetworkHeight { get; set; }
    }
}