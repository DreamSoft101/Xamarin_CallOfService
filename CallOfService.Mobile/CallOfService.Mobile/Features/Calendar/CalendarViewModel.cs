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
#pragma warning disable 4014
            _analyticsService.Screen("Calendar");
#pragma warning restore 4014
        }

        public void OnDisappearing()
        {
            
        }
    }
}
