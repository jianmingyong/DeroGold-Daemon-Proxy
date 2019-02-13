using Newtonsoft.Json;
using TurtlecoinRpc.Response.Json.Daemon.BlockHeader;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class BlockHeaderByHashRpcResponse
    {
        [JsonProperty("block_header")]
        public BlockHeaderRpcResponse BlockHeader { get; set; }
    }
}