using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TurtlecoinRpc.Request.Http
{
    internal class HttpPostRpcRequest<TResponse, TRequest> : HttpRpcRequest
    {
        private TRequest Request { get; }

        public HttpPostRpcRequest(string endpoint, TRequest request, HttpClient httpClient = null, HttpRpcRequestOptions httpRpcRequestOptions = null) : base(endpoint, httpClient, httpRpcRequestOptions)
        {
            Request = request;
        }

        public async Task<TResponse> GetResponseAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new StringContent(JsonConvert.SerializeObject(Request), Encoding.UTF8, "application/json");

            CancellationTokenSource cancellationTokenSource = null;

            if (cancellationToken == default(CancellationToken))
            {
                cancellationTokenSource = new CancellationTokenSource(HttpRpcRequestOptions.RequestTimeoutDelay);
                cancellationToken = cancellationTokenSource.Token;
            }

            try
            {
                var response = await HttpClient.PostAsync(Endpoint, request, cancellationToken).ConfigureAwait(false);
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