using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Core.Networking;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Proxies.Abstratcs;
using Newtonsoft.Json;

namespace CallOfService.Technician.Mobile.Proxies
{
    public class AppointmentProxy : BaseProxy, IAppointmentProxy
    {
        public AppointmentProxy(ILogger logger) : base(logger)
        {
        }

        public async Task<List<Appointment>> GetAppointments(int userId)
        {
            try
            {
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                HttpResponseMessage responseMessage = await Client.GetAsync($"{UrlConstants.AppointmentsUrl}?view=year&userid={userId}");
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