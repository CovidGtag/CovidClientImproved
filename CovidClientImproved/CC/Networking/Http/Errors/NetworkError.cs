using System;

namespace CovidClientImproved.CC.Networking.Http.Errors
{
    public class NetworkError
    {
        public NetworkErrorCode ErrorCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
