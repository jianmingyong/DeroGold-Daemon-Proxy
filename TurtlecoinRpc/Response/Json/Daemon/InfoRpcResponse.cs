using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class InfoRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the height of the daemon.
        /// </summary>
        [JsonProperty("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Get the difficulty of the top block.
        /// </summary>
        [JsonProperty("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Get the transaction count in the network.
        /// </summary>
        [JsonProperty("tx_count")]
        public ulong TransactionCount { get; set; }

        [JsonProperty("tx_pool_size")]
        public ulong TransactionPoolSize { get; set; }

        [JsonProperty("alt_blocks_count")]
        public ulong AltBlocksCount { get; set; }

        /// <summary>
        /// Get the number of outgoing connections from the daemon.
        /// </summary>
        [JsonProperty("outgoing_connections_count")]
        public ulong OutgoingConnectionsCount { get; set; }

        /// <summary>
        /// Get the number of incoming connections to the daemon.
        /// </summary>
        [JsonProperty("incoming_connections_count")]
        public ulong IncomingConnectionsCount { get; set; }

        [JsonProperty("white_peerlist_size")]
        public ulong WhitePeerlistSize { get; set; }

        [JsonProperty("grey_peerlist_size")]
        public ulong GreyPeerlistSize { get; set; }

        [JsonProperty("last_known_block_index")]
        public ulong LastKnownBlockIndex { get; set; }

        /// <summary>
        /// Get the height of the network.
        /// </summary>
        [JsonProperty("network_height")]
        public ulong NetworkHeight { get; set; }

        /// <summary>
        /// Get the pre-determined fork heights.
        /// </summary>
        [JsonProperty("upgrade_heights")]
        public ulong[] UpgradeHeights { get; set; }

        /// <summary>
        /// Get the supported fork height.
        /// </summary>
        [JsonProperty("supported_height")]
        public ulong SupportedHeight { get; set; }

        /// <summary>
        /// Get the hashrate of the network.
        /// </summary>
        [JsonProperty("hashrate")]
        public ulong Hashrate { get; set; }

        [JsonProperty("major_version")]
        public ulong MajorVersion { get; set; }

        [JsonProperty("minor_version")]
        public ulong MinorVersion { get; set; }

        /// <summary>
        /// Get the version of the daemon.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("start_time")]
        public ulong StartTime { get; set; }

        /// <summary>
        /// Get the sync status.
        /// </summary>
        [JsonProperty("synced")]
        public bool Synced { get; set; }

        /// <summary>
        /// Get whether the daemon is on testnet or not.
        /// </summary>
        [JsonProperty("testnet")]
        public bool Testnet { get; set; }
    }
}