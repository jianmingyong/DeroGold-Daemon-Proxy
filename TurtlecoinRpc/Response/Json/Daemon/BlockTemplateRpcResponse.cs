using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class BlockTemplateRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the difficulty of the network.
        /// </summary>
        [JsonProperty("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Get the chain height of the network.
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Get the offset reserved.
        /// </summary>
        [JsonProperty("reserved_offset")]
        public ulong ReservedOffset { get; set; }

        /// <summary>
        /// Get the block template with empty "hole" for nonce.
        /// </summary>
        [JsonProperty("blocktemplate_blob")]
        public string BlockTemplateBlob { get; set; }
    }
}