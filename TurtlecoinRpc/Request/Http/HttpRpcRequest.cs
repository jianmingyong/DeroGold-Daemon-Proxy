using System.Net.Http;

namespace TurtlecoinRpc.Request.Http
{
    internal abstract class HttpRpcRequest
    {
        protected string Endpoint { get; }

        protected HttpClient HttpClient { get; }

        protected HttpRpcRequestOptions HttpRpcRequestOptions { get; }

        protected HttpRpcRequest(string endpoint, HttpClient httpClient = null, HttpRpcRequestOptions httpRpcRequestOptions = null)
        {
            Endpoint = endpoint;
            HttpClient = httpClient ?? new HttpClient();
            HttpRpcRequestOptions = httpRpcRequestOptions ?? new HttpRpcRequestOptions();
        }
    }
}