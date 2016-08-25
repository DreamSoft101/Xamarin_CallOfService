using System;
using System.Windows.Input;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Calendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;

        public CalendarViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public DateTime Date { get; set; }

        public override void OnAppearing()
        {
            _analyticsService.Screen("Calendar");
        }

        public ICommand SelectDate
        {
            get
            {
                return new Command(async () =>
                {
                    this.Publish(new NewDateSelected(Date));
                    await NavigationService.Navigation.PopModalAsync(true);
                });
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new Command(async () =>
                {
                    await NavigationService.Navigation.PopModalAsync(true);
                });
            }
        }
    }
}
