using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.BlockHeader
{
    public class BlockHeaderRpcResponse
    {
        [JsonProperty("major_version")]
        public ulong MajorVersion { get; set; }

        [JsonProperty("minor_version")]
        public ulong MinorVersion { get; set; }

        /// <summary>
        /// Get the time at which the block is occured on chain since Unix epoch.
        /// </summary>
        [JsonProperty("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Get the hash of the previous block.
        /// </summary>
        [JsonProperty("prev_hash")]
        public string PreviousHash { get; set; }

        [JsonProperty("nonce")]
        public ulong Nonce { get; set; }

        /// <summary>
        /// Get whether the last block was an orphan or not.
        /// </summary>
        [JsonProperty("orphan_status")]
        public bool OrphanStatus { get; set; }

        /// <summary>
        /// Get the height of the last block.
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Get the height away from the known top block.
        /// </summary>
        [JsonProperty("depth")]
        public ulong Depth { get; set; }

        /// <summary>
        /// Get the hash of the last block.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Get the difficulty of the last block.
        /// </summary>
        [JsonProperty("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Get the reward of the block.
        /// </summary>
        [JsonProperty("reward")]
        public ulong Reward { get; set; }

        /// <summary>
        /// Get the number of transactions in the block.
        /// </summary>
        [JsonProperty("num_txes")]
        public ulong NumberOfTransactions { get; set; }

        /// <summary>
        /// Get the size of the block.
        /// </summary>
        [JsonProperty("block_size")]
        public ulong BlockSize { get; set; }
    }
}