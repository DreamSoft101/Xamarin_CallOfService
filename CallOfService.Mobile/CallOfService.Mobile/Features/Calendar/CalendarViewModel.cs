using CallOfService.Mobile.Core;
using CallOfService.Mobile.UI;

namespace CallOfService.Mobile.Features.Calendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;

        public CalendarViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public override void OnAppearing()
        {
            _analyticsService.Screen("Calendar");
        }
    }
}
