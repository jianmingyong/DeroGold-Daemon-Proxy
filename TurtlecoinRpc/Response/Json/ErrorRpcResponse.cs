using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json
{
    public class ErrorRpcResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}