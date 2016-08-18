using CallOfService.Mobile.Core;
using CallOfService.Mobile.Messages;
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

        public Source Source { get; set; }

        public override void OnAppearing()
        {
            _analyticsService.Screen("Calendar");
        }
    }
}
