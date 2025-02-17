using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CovidClientImproved.CC.Networking.Http.Errors;
using CovidClientImproved.CC.Networking.Http.Responses;
using Newtonsoft.Json;

namespace CovidClientImproved.CC.Networking.Http.Client
{
    public class NetworkRequestSender : INetworkRequestSender
    {
        private readonly HttpClient _httpClient;

        public NetworkRequestSender(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<INetworkResponse> SendRequestAsync<T>(
            string url,
            HttpMethod method,
            object requestBody = null,
            Dictionary<string, string> headers = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);
                
                if (headers != null)
                {
                    foreach (var kvp in headers)
                    {
                        request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                    }
                }

                if (requestBody != null)
                {
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(requestBody),
                        Encoding.UTF8,
                        "application/json");
                }

                var response = await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseContentRead,
                    CancellationToken.None);

                return await ProcessResponseAsync<T>(response);
            }
            catch (OperationCanceledException)
            {
                return CreateErrorResponse<T>(NetworkErrorCode.Timeout, "Request timed out");
            }
            catch (HttpRequestException ex)
            {
                return CreateErrorResponse<T>(NetworkErrorCode.NetworkError, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>(NetworkErrorCode.Unknown, ex.Message);
            }
        }

        private async Task<INetworkResponse> ProcessResponseAsync<T>(
            HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                return new NetworkResponse(
                    isSuccess: false,
                    data: default,
                    error: new NetworkError
                    {
                        ErrorCode = MapStatusCodeToErrorCode(statusCode),
                        Timestamp = DateTime.UtcNow
                    },
                    statusCode: statusCode);
            }

            try
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new NetworkResponse(
                    isSuccess: true,
                    data: responseData,
                    error: null,
                    statusCode: statusCode);
            }
            catch
            {
                return CreateErrorResponse<T>(NetworkErrorCode.InvalidResponse, "Invalid response format");
            }
        }

        private static NetworkErrorCode MapStatusCodeToErrorCode(int statusCode)
        {
            if (statusCode >= 500 && statusCode < 600)
                return NetworkErrorCode.ServerError;
            if (statusCode == 401 || statusCode == 403)
                return NetworkErrorCode.InvalidData;
            if (statusCode >= 400 && statusCode < 500)
                return NetworkErrorCode.InvalidData;
            return NetworkErrorCode.Unknown;
        }

        private INetworkResponse CreateErrorResponse<T>(
            NetworkErrorCode errorCode,
            string errorMessage)
        {
            return new NetworkResponse(
                isSuccess: false,
                data: default,
                error: new NetworkError
                {
                    ErrorCode = errorCode,
                    Timestamp = DateTime.UtcNow
                },
                statusCode: 0);
        }
    }
}
