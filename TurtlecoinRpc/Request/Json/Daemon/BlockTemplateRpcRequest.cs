using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json.Daemon
{
    internal class BlockTemplateRpcRequest
    {
        [JsonProperty("reserve_size")]
        public ulong ReserveSize { get; set; }

        [JsonProperty("wallet_address")]
        public string WalletAddress { get; set; }
    }
}