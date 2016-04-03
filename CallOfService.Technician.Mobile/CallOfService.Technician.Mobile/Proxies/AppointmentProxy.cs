using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.Networking;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using CallOfService.Technician.Mobile.Services.Abstracts;
using Newtonsoft.Json;

namespace CallOfService.Technician.Mobile.Proxies
{
    public class AppointmentProxy : BaseProxy, IAppointmentProxy
    {

        public AppointmentProxy(ILogger logger, IUserService userService) : base(logger, userService)
        {
        }

        public async Task<List<Appointment>> GetAppointments(int userId)
        {
            try
            {
                CreateHttpClient();
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var url = $"{UrlConstants.AppointmentsUrl}?View=year&UserId={userId}&date={DateTime.Now.ToString("yyyy-MM-dd")}";
                HttpResponseMessage responseMessage = await Client.GetAsync(url);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Appointment>>(responseString);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

    }
}