namespace TurtlecoinRpc.Request.Http.Daemon
{
    internal class HttpPostRpcRequest<TResponse, TRequest> : Http.HttpPostRpcRequest<TResponse, TRequest>
    {
        public HttpPostRpcRequest(DaemonRpc daemonRpc, string endpoint, TRequest request) : base(endpoint, request, daemonRpc.HttpClient, daemonRpc.HttpRpcRequestOptions)
        {
        }
    }
}