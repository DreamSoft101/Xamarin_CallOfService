using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.Networking;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using Newtonsoft.Json;

namespace CallOfService.Technician.Mobile.Proxies
{
    public class LoginProxy : BaseProxy, ILoginProxy
    {

        public LoginProxy(ILogger logger) : base(logger)
        {
        }

        public async Task<UserToken> Login(string userName, string password)
        {
            try
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new {username = userName, password = password}), Encoding.UTF8, "application/json");
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                HttpResponseMessage responseMessage = await Client.PostAsync(UrlConstants.LoginUrl, stringContent);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserToken>(responseString);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

    }
}
