using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon.Transaction
{
    public class InputTransactionValueRpcResponse
    {
        [JsonProperty("height")]
        public ulong Height { get; set; }
    }
}