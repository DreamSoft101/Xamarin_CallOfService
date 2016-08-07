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

namespace CallOfService.Mobile.Proxies
{
    public class LoginProxy : BaseProxy, ILoginProxy
    {
        public LoginProxy(ILogger logger, IUserService userService) : base(logger, userService)
        {
        }

        public async Task<UserToken> Login(string userName, string password)
        {
            var url = UrlConstants.LoginUrl;
            var stringContent = new StringContent(JsonConvert.SerializeObject(new {username = userName, password = password}), Encoding.UTF8, "application/json");
            var responseString = await PostStringAsync(url, stringContent, 1, false);

            try
            {
                return string.IsNullOrEmpty(responseString) ? null : JsonConvert.DeserializeObject<UserToken>(responseString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Logout()
        {
            var url = UrlConstants.LogoutUrl;
            return await PostAsync(url, null, 1, false);
        }

    }
}
