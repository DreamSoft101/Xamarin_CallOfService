using CallOfService.Mobile.Core.Networking;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace CallOfService.Mobile.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        private const string EnableLocationKey = "EnableLocation";
        private static readonly bool EnableLocationDefault = true;
        public static bool EnableLocation
        {
            get { return AppSettings.GetValueOrDefault(EnableLocationKey, EnableLocationDefault); }
            set { AppSettings.AddOrUpdateValue(EnableLocationKey, value); }
        }

        private const string ServerUrlKey = "ServerUrl";
        private static readonly string ServerUrlDefault = UrlConstants.BaseUrlDefault;
        public static string ServerUrl
        {
            get { return AppSettings.GetValueOrDefault(ServerUrlKey, ServerUrlDefault); }
            set { AppSettings.AddOrUpdateValue(ServerUrlKey, value); }
        }
    }
}