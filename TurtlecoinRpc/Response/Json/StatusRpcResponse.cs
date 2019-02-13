using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json
{
    public class StatusRpcResponse
    {
        /// <summary>
        /// Get the status of request.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}