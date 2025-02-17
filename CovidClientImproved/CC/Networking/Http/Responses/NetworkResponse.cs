using CovidClientImproved.CC.Networking.Http.Errors;

namespace CovidClientImproved.CC.Networking.Http.Responses
{
    public class NetworkResponse : INetworkResponse
    {
        public NetworkResponse(bool isSuccess, object data, NetworkError error, int statusCode)
        {
            Data = data;
        }

        public bool IsSuccess { get; }
        public object Data { get; }
        public NetworkError Error { get; }
        public int StatusCode { get; }
    }
}
