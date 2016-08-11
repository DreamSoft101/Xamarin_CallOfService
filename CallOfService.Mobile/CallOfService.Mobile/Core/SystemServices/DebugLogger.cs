using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Mobile.Services.Abstracts;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using Xamarin.Forms;

namespace CallOfService.Mobile.Core.SystemServices
{
    public class ElmahLogger : ILogger
    {
        private readonly string _userEmail;
        private readonly string _deviceDetails;

        public ElmahLogger(IUserService userService)
        {
            if (string.IsNullOrEmpty(_userEmail))
                _userEmail = userService.GetUserCredentials()?.Email;

            if (string.IsNullOrEmpty(_deviceDetails))
                _deviceDetails = $"{Device.OS} - {CrossDeviceInfo.Current.Version} - {CrossDeviceInfo.Current.Model}";
        }

        public void WriteInfo(string message, string details = null)
        {
            Send("Information", message, details);
        }

        public void WriteWarning(string message, string details = null, Exception exception = null)
        {
            Send("Warning", message, details, exception);
        }

        public void WriteError(string message, string details = null, Exception exception = null)
        {
            Send("Error", message, details, exception);
        }

        private void Send(string severity, string message, string details, Exception exceptoin = null)
        {
            var elmahUrl = new Uri("https://elmah.io/api/v2/");
            var httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = elmahUrl,
                Timeout = TimeSpan.FromSeconds(1)
            };
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            const string messageUrl = "messages?logid=ab9c3a12-ec45-425c-b5ee-8cdbb3137b95";

            
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var messageObject = new { source = _deviceDetails, severity = severity, title = message, detail = $"{details} - {exceptoin}", user = _userEmail };
                    var messageContent = new StringContent(JsonConvert.SerializeObject(messageObject), Encoding.UTF8, "application/json");

                    await httpClient.PostAsync(messageUrl, messageContent);
                }
                catch(Exception e)
                {
                    
                }
            });
        }
    }

    public class DebugLogger : ILogger
    {
        public void WriteInfo(string message, string details = null)
        {
            Debug.WriteLine(message);
        }

        public void WriteWarning(string message, string details = null, Exception exception = null)
        {
            Debug.WriteLine(message);
        }

        public void WriteError(string message, string details = null, Exception exception = null)
        {
            Debug.WriteLine(message);
        }
    }

    public interface ILogger
    {
        void WriteInfo(string message, string details = null);
        void WriteWarning(string message, string details = null, Exception exception = null);
        void WriteError(string message, string details = null, Exception exception = null);
    }
}
