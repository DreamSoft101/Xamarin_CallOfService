using System;using System.Collections.Generic;using System.Diagnostics;using System.IO;using System.Net;using System.Net.Http;using System.Text;using System.Threading.Tasks;using CallOfService.Technician.Mobile.Core.Networking;using CallOfService.Technician.Mobile.Core.SystemServices;using CallOfService.Technician.Mobile.Domain;using CallOfService.Technician.Mobile.Proxies.Abstratcs;using CallOfService.Technician.Mobile.Services.Abstracts;using Newtonsoft.Json;namespace CallOfService.Technician.Mobile.Proxies{    public class AppointmentProxy : BaseProxy, IAppointmentProxy    {        public AppointmentProxy(ILogger logger, IUserService userService) : base(logger, userService)        {        }        public async Task<List<Appointment>> GetAppointments(int userId)        {            try            {                CreateHttpClient();                Client.DefaultRequestHeaders.Add("Accept", "application/json");                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");                var url =                    $"{UrlConstants.AppointmentsUrl}?View=year&UserId={userId}&date={DateTime.Now.ToString("yyyy-MM-dd")}";                HttpResponseMessage responseMessage = await Client.GetAsync(url);                var responseString = await responseMessage.Content.ReadAsStringAsync();				LogResponse(responseMessage,responseString,false);                return JsonConvert.DeserializeObject<List<Appointment>>(responseString);            }            catch (Exception e)            {                Debug.WriteLine(e.ToString());                return null;            }        }        public async Task<Job> GetJobById(int jobId)        {            try            {                CreateHttpClient();                Client.DefaultRequestHeaders.Add("Accept", "application/json");                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");                var url = $"{UrlConstants.JobByIdUrl}/{jobId}";                HttpResponseMessage responseMessage = await Client.GetAsync(url);                var responseString = await responseMessage.Content.ReadAsStringAsync();				LogResponse(responseMessage,responseString,false);                return JsonConvert.DeserializeObject<Job>(responseString);            }            catch (Exception e)            {                Debug.WriteLine(e.ToString());                return null;            }        }        private string _googleApiKey = "AIzaSyDwSOIXf8vGJ6fuU0_TUHuY2BAIXi9UZAE";        public async Task<GpsPoint> GetJobLocation(AddressInfo location)        {            try            {                var httpClient = new HttpClient                {                    BaseAddress = new Uri("https://maps.googleapis.com/"),                    Timeout = new TimeSpan(0, 2, 0)                };				string url = $"maps/api/geocode/json?address={location.FormattedAddress}&key={_googleApiKey}";                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");                HttpResponseMessage message = await httpClient.GetAsync(url);                string responseString = await message.Content.ReadAsStringAsync();				LogResponse(message,responseString);                var result = JsonConvert.DeserializeAnonymousType(responseString,                    new {results = new[] {new {geometry = new {location = new {lat = "", lng = ""}}}}});                return new GpsPoint                {                    Lat = result.results[0].geometry.location.lat,                    Lng = result.results[0].geometry.location.lng,                    IsValid = true                };            }            catch (Exception)            {                return new GpsPoint {IsValid = false};            }        }        public async Task<bool> StartJob(int jobId)        {            try            {                CreateHttpClient();                var stringContent = new StringContent(JsonConvert.SerializeObject(new {Id = jobId}), Encoding.UTF8,                    "application/json");                Client.DefaultRequestHeaders.Add("Accept", "application/json");                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");                var url = $"{UrlConstants.StartJob}";                HttpResponseMessage responseMessage = await Client.PostAsync(url, stringContent);				LogResponse(responseMessage,string.Empty);                if (responseMessage.StatusCode == HttpStatusCode.OK)                    return true;                else                {                    string responseString = await responseMessage.Content.ReadAsStringAsync();                    Debug.WriteLine(responseString);                }                return false;            }            catch (Exception e)            {                Debug.WriteLine(e.ToString());                return false;            }        }        public async Task<bool> FinishJob(int jobId)        {            try            {                CreateHttpClient();                var stringContent = new StringContent(JsonConvert.SerializeObject(new {Id = jobId}), Encoding.UTF8,                    "application/json");                Client.DefaultRequestHeaders.Add("Accept", "application/json");                //Client.DefaultRequestHeaders.Add("Content-Type", "application/json");                var url = $"{UrlConstants.FinishJob}";                HttpResponseMessage responseMessage = await Client.PostAsync(url, stringContent);				LogResponse(responseMessage,string.Empty);                if (responseMessage.StatusCode == HttpStatusCode.OK)                    return true;                else                {                    string responseString = await responseMessage.Content.ReadAsStringAsync();                    Debug.WriteLine(responseString);                }                return false;            }            catch (Exception e)            {                Debug.WriteLine(e.ToString());                return false;            }        }		public async Task<bool> AddNote(int jobNumber, string newNoteText, List<byte[]> attachments, DateTime now)        {            CreateHttpClient(10);            Client.DefaultRequestHeaders.Add("Accept", "application/json");            using (var formData = new MultipartFormDataContent("form-data"))            {                formData.Add(new StringContent(jobNumber.ToString(), Encoding.UTF8), "Id");                formData.Add(new StringContent(newNoteText, Encoding.UTF8), "Note");                formData.Add(new StringContent(GetTime(now).ToString(), Encoding.UTF8), "Timestamp");                for (int index = 0; index < attachments.Count; index++)                {                    var data = attachments[index];					formData.Add(new StreamContent(new MemoryStream (data)), $"S{index + 1}", $"{jobNumber} - {Guid.NewGuid()}.jpg");                }                try                {                    var responseMessage = await Client.PostAsync(UrlConstants.NewNoteUrl, formData);					LogResponse(responseMessage,string.Empty);                    if (responseMessage.StatusCode == HttpStatusCode.OK)                        return true;                    return false;                }                catch (Exception e)                {                    Debug.WriteLine(e.ToString());                    return false;                }            }        }		private long GetTime(DateTime datetime)		{			long retval=0;			var st = new DateTime(1970,1,1);			var t = datetime.ToUniversalTime()-st;			retval = (long)(t.TotalMilliseconds+0.5);			return retval;		}    }}