using CallOfService.Mobile.Core;
using CallOfService.Mobile.UI;
using PropertyChanged;

namespace CallOfService.Mobile.Features.Calendar
{
    [ImplementPropertyChanged]
    public class CalendarViewModel : IViewAwareViewModel
    {
        private readonly IAnalyticsService _analyticsService;

        public CalendarViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public void Dispose()
        {
            
        }

        public void OnAppearing()
        {
            _analyticsService.Screen("Calendar");
        }

        public void OnDisappearing()
        {
            
        }
    }
}
