using System;
using System.Net;
using System.Net.Http;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Services.Abstracts;

namespace CallOfService.Mobile.Core.Networking
{
    public class BaseProxy
    {
        protected readonly ILogger Logger;
        private readonly IUserService _userService;
        protected HttpClient Client;

        public BaseProxy(ILogger logger,IUserService userService)
        {
            Logger = logger;
            _userService = userService;
            Client = CreateHttpClient();
        }

		protected HttpClient CreateHttpClient(int minutes = 2,bool useTokenExpirationHandler = true)
		{
			var serverUri = new Uri (UrlConstants.BaseUrl);

			HttpClient httpClient;

			if (useTokenExpirationHandler)
				httpClient = new HttpClient (new TokenExpirationHandler ()) {
					BaseAddress = serverUri,
					Timeout = new TimeSpan (0, minutes, 0),
				};
			else
				httpClient = new HttpClient {
					BaseAddress = serverUri,
					Timeout = new TimeSpan (0, minutes, 0),
				};

			var userCredentials = _userService.GetUserCredentials ();
			if (userCredentials != null) {
				httpClient.DefaultRequestHeaders.Add ("Token-Id", userCredentials.Token);
			}

			return httpClient;
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