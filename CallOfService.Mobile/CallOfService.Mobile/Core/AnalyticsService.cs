using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Services.Abstracts;
using Segment;
using Segment.Model;
using Xamarin.Forms;

namespace CallOfService.Mobile.Core
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUserService _userService;

        public AnalyticsService(IUserService userService)
        {
            _userService = userService;
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
                    {"platform", Device.OS.ToString()}
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
                System.Diagnostics.Debug.WriteLine("Exception while accessing analytics: " + e);
            }
        }

        public async Task Track(string eventName, Properties properties = null, Options options = null)
        {
            try
            {
                var user = await _userService.GetCurrentUser();
                var userId = user.TenantId + "-" + user.UserId;
                Analytics.Client.Track(userId, eventName, properties, options);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception while accessing analytics: " + e);
            }
        }

        public async Task Screen(string screenName, Properties properties = null, Options options = null)
        {
            try
            {
                var user = await _userService.GetCurrentUser();
                var userId = user.TenantId + "-" + user.UserId;
                Analytics.Client.Screen(userId, screenName, properties, options);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception while accessing analytics: " + e);
            }
        }
    }

    public interface IAnalyticsService
    {
        Task Identify();
        Task Track(string eventName, Properties properties = null, Options options = null);
        Task Screen(string screenName, Properties properties = null, Options options = null);
    }
}
