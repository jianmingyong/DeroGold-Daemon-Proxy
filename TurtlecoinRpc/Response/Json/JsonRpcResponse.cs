using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json
{
    public class JsonRpcResponse<TResponse>
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("error")]
        public ErrorRpcResponse Error { get; set; }

        [JsonProperty("result")]
        public TResponse Result { get; set; }
    }
}