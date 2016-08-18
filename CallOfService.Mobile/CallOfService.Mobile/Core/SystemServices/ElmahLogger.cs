using System;
using System.Collections.Generic;
using CallOfService.Mobile.Services.Abstracts;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Plugin.DeviceInfo;
using Xamarin.Forms;

namespace CallOfService.Mobile.Core.SystemServices
{
    public class ElmahLogger : ILogger
    {
        private const string ApiKey = "f644e36c15ac44c1868b6c11084265c8";
        private const string LogId = "ab9c3a12-ec45-425c-b5ee-8cdbb3137b95";
        private readonly string _userEmail;
        private readonly string _deviceDetails;
        private readonly IElmahioAPI _api;

        public ElmahLogger(IUserService userService)
        {
            if (string.IsNullOrEmpty(_userEmail))
                _userEmail = userService.GetUserCredentials()?.Email;

            if (string.IsNullOrEmpty(_deviceDetails))
                _deviceDetails = $"{Device.OS} - {CrossDeviceInfo.Current.Version} - {CrossDeviceInfo.Current.Model}";

            _api = ElmahioAPI.Create(ApiKey);
        }

        public void WriteInfo(string message, string details = null, IList<Item> dataList = null)
        {
            Send("Information", message, details, null, dataList);
        }

        public void WriteWarning(string message, string details = null, Exception exception = null, IList<Item> dataList = null)
        {
            Send("Warning", message, details, exception, dataList);
        }

        public void WriteError(string message, string details = null, Exception exception = null, IList<Item> dataList = null)
        {
            Send("Error", message, details, exception, dataList);
        }

        private void Send(string severity, string message, string details, Exception exception = null, IList<Item> dataList = null)
        {
            try
            {
                _api.Messages.Create(LogId, new CreateMessage
                {
                    DateTime = DateTime.UtcNow,
                    Severity = severity,
                    Source = _deviceDetails,
                    Title = message,
                    Type = exception?.GetType()?.FullName,
                    Detail = $"{details} - {exception}",
                    User = _userEmail,
                    Data = dataList
                });
            }
            catch
            {

            }
        }
    }

}