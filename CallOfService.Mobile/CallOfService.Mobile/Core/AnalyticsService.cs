using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Services.Abstracts;
using Segment;
using Segment.Model;
using Version.Plugin;
using Xamarin.Forms;

namespace CallOfService.Mobile.Core
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public AnalyticsService(IUserService userService, ILogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task Identify()
        {
            try
            {
                var user = await _userService.GetCurrentUser();
                var userId = user.TenantId + "-" + user.UserId;
                Analytics.Client.Identify(userId, new Traits
                {
                    {"email", user.Email},
                    {"firstName", user.FirstName},
                    {"lastName", user.LastName},
                    {"name", user.FirstName + " " + user.LastName},
                    {"phone", user.Phone},
                    {"tenantId", user.TenantId},
                    {"analyticsSource", "xamarin"},
                    {"source", user.Source},
                    {"referrerUrl", user.ReferrerUrl},
                    {"platform", Device.OS.ToString()},
                    {"version", CrossVersion.Current.Version}
                });
                Analytics.Client.Group(userId, user.TenantId, new Traits
                {
                    {"id", user.TenantId},
                    {"name", user.CompanyName},
                    {"companyId", user.TenantId},
                    {"company_id", user.TenantId},
                    {"groupId", user.TenantId},
                    {"analyticsSource", "xamarin"},
                    {"source", user.Source},
                    {"referrerUrl", user.ReferrerUrl},
                    {"platform", Device.OS.ToString()}
                });
            }
            catch (Exception e)
            {
                _logger.WriteWarning("Exceptoin while identifying user to segment", exception: e);
            }
        }

        public async void Track(string eventName, Properties properties = null, Options options = null)
        {
            try
            {
                if(properties == null)
                    properties = new Properties();

                properties.Add("platform", Device.OS.ToString());
                properties.Add("version", CrossVersion.Current.Version);

                var user = await _userService.GetCurrentUser();
                var userId = user.TenantId + "-" + user.UserId;
                Analytics.Client.Track(userId, eventName, properties, options);
            }
            catch (Exception e)
            {
                _logger.WriteError("Exceptoin while tracking user action to segment", exception: e);
            }
        }

        public async void Screen(string screenName, Properties properties = null, Options options = null)
        {
            try
            {
                if (properties == null)
                    properties = new Properties();

                properties.Add("platform", Device.OS.ToString());
                properties.Add("version", CrossVersion.Current.Version);

                var user = await _userService.GetCurrentUser();
                var userId = user.TenantId + "-" + user.UserId;
                Analytics.Client.Track(userId, $"visit {screenName}", properties, options);
            }
            catch (Exception e)
            {
                _logger.WriteWarning("Exceptoin while tracking page visit user to segment", exception: e);
            }
        }

        public void Initialize()
        {
            Analytics.Initialize("fsRp8sDmNQbkQzldzqwdSCurOTF0S1vj");
        }
    }

    public interface IAnalyticsService
    {
        Task Identify();
        void Track(string eventName, Properties properties = null, Options options = null);
        void Screen(string screenName, Properties properties = null, Options options = null);
        void Initialize();
    }
}
