using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Services.Abstracts;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Polly;

namespace CallOfService.Mobile.Core.Networking
{
    public class BaseProxy
    {
        protected readonly ILogger Logger;
        private readonly IUserService _userService;
        protected HttpClient Client;

        public BaseProxy(ILogger logger, IUserService userService)
        {
            Logger = logger;
            _userService = userService;
            CreateHttpClient();
        }

        protected void CreateHttpClient(int minutes = 1, bool useTokenExpirationHandler = true, bool emptyBaseUrl = false)
        {
            var serverUri = new Uri(UrlConstants.BaseUrl);

            HttpClient httpClient;

            if (useTokenExpirationHandler)
                httpClient = new HttpClient(new TokenExpirationHandler())
                {
                    BaseAddress = emptyBaseUrl ? null : serverUri,
                    Timeout = new TimeSpan(0, minutes, 0),
                };
            else
                httpClient = new HttpClient(new NativeMessageHandler())
                {
                    BaseAddress = emptyBaseUrl ? null : serverUri,
                    Timeout = new TimeSpan(0, minutes, 0),
                };

            var userCredentials = _userService.GetUserCredentials();
            if (userCredentials != null)
            {
                httpClient.DefaultRequestHeaders.Add("Token-Id", userCredentials.Token);
            }

			Client = httpClient;
        }

        protected async Task<T> GetAsync<T>(string url, int timeOutMinutes = 1, T defaultValue = null) where T : class
        {
            if (!IsOnline())
                return defaultValue;

            try
            {
                CreateHttpClient(timeOutMinutes);
                Client.DefaultRequestHeaders.Add("Accept", "application/json");

                var responseMessage = await Policy
                  .Handle<WebException>()
                  .WaitAndRetryAsync
                  (
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                  )
                  .ExecuteAsync(async () => await Client.GetAsync(url));

                var responseString = await responseMessage.Content.ReadAsStringAsync();
                LogResponse(responseMessage, responseString, false);
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return defaultValue;
            }
        }

        protected async Task<string> GetStringAsync(string url, int timeOutMinutes = 1, bool emptyBaseUrl = false)
        {
            if (!IsOnline())
                return null;

            try
            {
                CreateHttpClient(timeOutMinutes, true, emptyBaseUrl);
                Client.DefaultRequestHeaders.Add("Accept", "application/json");

                var responseMessage = await Policy
                 .Handle<WebException>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 5,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                 )
                 .ExecuteAsync(async () => await Client.GetAsync(url));

                var responseString = await responseMessage.Content.ReadAsStringAsync();
                LogResponse(responseMessage, responseString, false);
                return responseString;
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return null;
            }
        }

        protected async Task<string> PostStringAsync(string url, StringContent postStringContent, int timeOutMinutes = 1, bool useTokenExpiryHandler = true)
        {
            if (!IsOnline())
                return null;

            try
            {
                CreateHttpClient(timeOutMinutes, useTokenExpiryHandler);

                Client.DefaultRequestHeaders.Add("Accept", "application/json");

                var responseMessage = await Policy
                 .Handle<WebException>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 5,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                 )
                 .ExecuteAsync(async () => await Client.PostAsync(url, postStringContent));

                LogResponse(responseMessage, string.Empty);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                    return null;
                else
                {
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    Logger.WriteError(responseString);
                    return responseString;
                }
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return null;
            }
        }

        protected async Task<bool> PostAsync(string url, StringContent postStringContent, int timeOutMinutes = 1, bool useTokenExpiryHandler = true)
        {
            if (!IsOnline())
                return false;

            try
            {
                CreateHttpClient(timeOutMinutes, useTokenExpiryHandler);
                
                Client.DefaultRequestHeaders.Add("Accept", "application/json");

                var responseMessage = await Policy
                 .Handle<WebException>()
                 .WaitAndRetryAsync
                 (
                   retryCount: 5,
                   sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                 )
                 .ExecuteAsync(async () => await Client.PostAsync(url, postStringContent));

                LogResponse(responseMessage, string.Empty);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                {
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    Logger.WriteError(responseString);
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return false;
            }
        }

        protected async Task<bool> PostFormDataAsync(string url, IDictionary<StringContent, string> formContents, IList<Tuple<StreamContent, string, string>> attachements, int timeOutMinutes = 1)
        {
            if (!IsOnline())
                return false;

            try
            {
                CreateHttpClient(10);
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                using (var formData = new MultipartFormDataContent("form-data"))
                {
                    foreach (var stringContent in formContents)
                    {
                        formData.Add(stringContent.Key, stringContent.Value);
                    }

                    foreach (var attachment in attachements)
                    {
                        formData.Add(attachment.Item1, attachment.Item2, attachment.Item3);
                    }

                    try
                    {
						var responseMessage = await Policy
							.Handle<WebException>()
							.WaitAndRetryAsync
							(
								retryCount: 5,
								sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
							)
							.ExecuteAsync(async () =>
							{
								return await Client.PostAsync(url, formData);
							});

                        LogResponse(responseMessage, string.Empty);
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                            return true;
                        else
                        {
                            var responseString = await responseMessage.Content.ReadAsStringAsync();
                            Logger.WriteError(responseString);
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteError(e);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return false;
            }
        }

        protected bool IsOnline()
        {
            return CrossConnectivity.Current.IsConnected;
        }


        protected void LogResponse(HttpResponseMessage responseMessage, string content, bool logSuccessfulResponse = true)
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