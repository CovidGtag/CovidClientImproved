using CovidClientImproved.CC.Networking.Http.Errors;

namespace CovidClientImproved.CC.Networking.Http.Responses
{
    public interface INetworkResponse
    {
        bool IsSuccess { get; }
        object Data { get; }
        NetworkError Error { get; }
        int StatusCode { get; }
    }
}
