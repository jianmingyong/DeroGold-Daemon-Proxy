using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class OutputTargetRpcResponse
    {
        [JsonProperty("data")]
        public OutputTargetDataRpcResponse Data { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}