using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Json
{
    internal class JsonRpcRequest : JsonRpcRequest<string[]>
    {
        public JsonRpcRequest(string method, string password = null, ulong id = 0) : base(method, new string[0], password, id)
        {
        }
    }

    internal class JsonRpcRequest<TRequest>
    {
        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string JsonRpc { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; set; }

        [JsonProperty("params", Required = Required.Always)]
        public TRequest Parameters { get; set; }

        public JsonRpcRequest(string method, TRequest parameters, string password = null, ulong id = 0)
        {
            JsonRpc = "2.0";
            Id = id;
            Password = password;
            Method = method;
            Parameters = parameters;
        }
    }
}