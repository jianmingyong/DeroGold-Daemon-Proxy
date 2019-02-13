using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class OutputTargetDataRpcResponse
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}