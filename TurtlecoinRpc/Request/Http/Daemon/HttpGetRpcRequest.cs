namespace TurtlecoinRpc.Request.Http.Daemon
{
    internal class HttpGetRpcRequest<TResponse> : Http.HttpGetRpcRequest<TResponse>
    {
        public HttpGetRpcRequest(DaemonRpc daemonRpc, string endpoint) : base(endpoint, daemonRpc.HttpClient, daemonRpc.HttpRpcRequestOptions)
        {
        }
    }
}