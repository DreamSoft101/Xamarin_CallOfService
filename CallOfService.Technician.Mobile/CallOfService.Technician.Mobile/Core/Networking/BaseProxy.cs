using System;
using System.Net;
using System.Net.Http;
using CallOfService.Technician.Mobile.Core.SystemServices;

namespace CallOfService.Technician.Mobile.Core.Networking
{
    public class BaseProxy
    {
        protected readonly ILogger Logger;
        protected HttpClient Client;

        public BaseProxy(ILogger logger)
        {
            Logger = logger;
            Client = CreateHttpClient();
        }

        private HttpClient CreateHttpClient()
        {
            var serverUri = new Uri(UrlConstants.BaseUrl);

            return new HttpClient
            {
                BaseAddress = serverUri,
                Timeout = new TimeSpan(0, 2, 0)
            };
        }

        protected void LogResponse(HttpResponseMessage responseMessage, string content,
            bool logSuccessfulResponse = true)
        {
            if (responseMessage.StatusCode != HttpStatusCode.Created &&
                responseMessage.StatusCode != HttpStatusCode.OK)
            {
                Logger.WriteError(
                    $"Error making a HTTP request URL : {responseMessage.RequestMessage.RequestUri}, " +
                    $"HttpMethod : {responseMessage.RequestMessage.Method}, " +
                    $"Status Code : {responseMessage.StatusCode}, " +
                    $"Response : {content} ");
            }

            if (logSuccessfulResponse)
            {
                Logger.WriteInfo(
                    $"Successful HTTP request , request URL : {responseMessage.RequestMessage.RequestUri}, " +
                    $"HttpMethod : {responseMessage.RequestMessage.Method}, " +
                    $"Status Code : {responseMessage.StatusCode}, " +
                    $"Response : {content} ");
            }
        }

        protected void LogRequest(string requestPayload)
        {
            Logger.WriteInfo(requestPayload);
        }
    }
}