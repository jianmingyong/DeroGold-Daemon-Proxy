using Newtonsoft.Json;

namespace TurtlecoinRpc.Response.Json.Daemon
{
    public class CurrencyIdRpcResponse
    {
        /// <summary>
        /// Get the unique currency identifier.
        /// </summary>
        [JsonProperty("currency_id_blob")]
        public string CurrencyIdBlob { get; set; }
    }
}