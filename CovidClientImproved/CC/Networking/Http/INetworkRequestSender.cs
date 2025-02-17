using CovidClientImproved.CC.Networking.Http.Responses;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CovidClientImproved.CC.Networking.Http
{
    public interface INetworkRequestSender
    {
        Task<INetworkResponse> SendRequestAsync<T>(
            string url,
            HttpMethod method,
            object requestBody = null,
            Dictionary<string, string> headers = null);
    }
}
