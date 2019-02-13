using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.BlockHeader
{
    public class BlockHeaderShortRpcResponse
    {
        /// <summary>
        /// Get the difficulty of the block.
        /// </summary>
        [JsonProperty("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Get the time at which the block is occured on the chain since Unix epoch.
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Get the height of the block.
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Get the hash of the block.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Get the number of transactions in the block.
        /// </summary>
        [JsonProperty("tx_count")]
        public ulong TransactionCount { get; set; }

        /// <summary>
        /// Get the size of the block.
        /// </summary>
        [JsonProperty("cumul_size")]
        public ulong CumulativeSize { get; set; }
    }
}