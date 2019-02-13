using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class PeersRpcResponse : StatusRpcResponse
    {
        /// <summary>
        /// Get the array of peers. (peer_ip:peer_port)
        /// </summary>
        [JsonProperty("peers")]
        public string[] Peers { get; set; }

        [JsonProperty("gray_peers")]
        public string[] GrayPeers { get; set; }
    }
}