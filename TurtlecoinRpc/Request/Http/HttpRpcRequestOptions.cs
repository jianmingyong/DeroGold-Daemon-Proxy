using System;

namespace TurtlecoinRpc.Request.Http
{
    public class HttpRpcRequestOptions
    {
        /// <summary>
        /// Get/Set the time delay for the request to be considered as a time out.
        /// </summary>
        public TimeSpan RequestTimeoutDelay { get; set; } = TimeSpan.FromSeconds(100);

        /// <summary>
        /// Get/Set whether to use legacy rpc request endpoints in favor for obsolete turtlecoin fork.
        /// </summary>
        public bool UseLegacyEndpoints { get; set; }
    }
}