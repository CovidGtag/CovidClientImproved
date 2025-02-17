namespace CovidClientImproved.CC.Networking.Http.Errors
{
    public enum NetworkErrorCode
    {
        Unknown = 0,
        Timeout = 1001,
        InvalidResponse = 1002,
        NetworkError = 1003,
        ServerError = 1004,
        InvalidData = 1005
    }
}
