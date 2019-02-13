using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.BlockHeader;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class BlockRpcResponse : StatusRpcResponse
    {
        [JsonProperty("block")]
        public BlockHeaderDetailedRpcResponse Block { get; set; }
    }
}