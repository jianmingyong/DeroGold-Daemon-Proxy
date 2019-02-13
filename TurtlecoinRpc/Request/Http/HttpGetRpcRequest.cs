using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Http
{
    internal class HttpGetRpcRequest<TResponse> : HttpRpcRequest
    {
        public HttpGetRpcRequest(string endpoint, HttpClient httpClient = null, HttpRpcRequestOptions httpRpcRequestOptions = null) : base(endpoint, httpClient, httpRpcRequestOptions)
        {
        }

        public async Task<TResponse> GetResponseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            CancellationTokenSource cancellationTokenSource = null;

            if (cancellationToken == default(CancellationToken))
            {
                cancellationTokenSource = new CancellationTokenSource(HttpRpcRequestOptions.RequestTimeoutDelay);
                cancellationToken = cancellationTokenSource.Token;
            }

            try
            {
                var response = await HttpClient.GetAsync(Endpoint, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResponse>(json);
            }
            finally
            {
                cancellationTokenSource?.Dispose();
            }
        }
    }
}