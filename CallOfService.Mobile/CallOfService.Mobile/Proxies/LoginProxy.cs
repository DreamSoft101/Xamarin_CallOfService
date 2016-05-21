using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.Networking;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Proxies.Abstratcs;
using CallOfService.Mobile.Services.Abstracts;
using Newtonsoft.Json;
using System.Net;

namespace CallOfService.Mobile.Proxies
{
    public class LoginProxy : BaseProxy, ILoginProxy
    {
        public LoginProxy(ILogger logger, IUserService userService) : base(logger, userService)
        {
        }

        public async Task<UserToken> Login(string userName, string password)
        {
            try
            {
                CreateHttpClient(useTokenExpirationHandler: false);
                var stringContent = new StringContent(JsonConvert.SerializeObject(new { username = userName, password = password }), Encoding.UTF8, "application/json");
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var responseMessage = await Client.PostAsync(UrlConstants.LoginUrl, stringContent);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserToken>(responseString);
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return null;
            }
        }

        public async Task<bool> Logout()
        {
            try
            {
                CreateHttpClient(useTokenExpirationHandler: false);
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                var responseMessage = await Client.PostAsync(UrlConstants.LogoutUrl, null);
                LogResponse(responseMessage, string.Empty);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Logger.WriteError(e);
                return false;
            }
        }

    }
}
