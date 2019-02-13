using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.BlockHeader;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class BlocksRpcResponse : StatusRpcResponse
    {
        [JsonProperty("blocks")]
        public BlockHeaderShortRpcResponse[] Blocks { get; set; }
    }
}