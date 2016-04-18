using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
                var url =
                    $"{UrlConstants.AppointmentsUrl}?View=year&UserId={userId}&date={DateTime.Now.ToString("yyyy-MM-dd")}";
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

        public async Task<Job> GetJobById(int jobId)
        {
            try
            {
                CreateHttpClient();
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var url = $"{UrlConstants.JobByIdUrl}/{jobId}";
                HttpResponseMessage responseMessage = await Client.GetAsync(url);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Job>(responseString);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return null;
            }
        }

        private string _googleApiKey = "AIzaSyAhg7GY7slOwawnzamu-uN36_K8mJL8q_E";

        public async Task<GpsPoint> GetJobLocation(AddressInfo location)
        {

            try
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://maps.googleapis.com/"),
                    Timeout = new TimeSpan(0, 2, 0)
                };
                string url = $"maps/api/geocode/json?address={location.FormattedAddress}"; //&key={_googleApiKey}";
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage message = await httpClient.GetAsync(url);
                string responseString = await message.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeAnonymousType(responseString,
                    new {results = new[] {new {geometry = new {location = new {lat = "", lng = ""}}}}});

                return new GpsPoint
                {
                    Lat = result.results[0].geometry.location.lat,
                    Lng = result.results[0].geometry.location.lng,
                    IsValid = true
                };
            }
            catch (Exception)
            {
                return new GpsPoint {IsValid = false};
            }

        }

        public async Task<bool> StartJob(int jobId)
        {
            try
            {
                CreateHttpClient();
                var stringContent = new StringContent(JsonConvert.SerializeObject(new {Id = jobId}), Encoding.UTF8,
                    "application/json");

                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var url = $"{UrlConstants.StartJob}";
                HttpResponseMessage responseMessage = await Client.PostAsync(url, stringContent);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
        }

        public async Task<bool> FinishJob(int jobId)
        {
            try
            {
                CreateHttpClient();
                var stringContent = new StringContent(JsonConvert.SerializeObject(new {Id = jobId}), Encoding.UTF8,
                    "application/json");

                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                var url = $"{UrlConstants.FinishJob}";
                HttpResponseMessage responseMessage = await Client.PostAsync(url, stringContent);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return false;
            }
        }
    }
}