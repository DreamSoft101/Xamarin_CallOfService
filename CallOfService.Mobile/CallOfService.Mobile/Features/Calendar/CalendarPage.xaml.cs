using System;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Calendar
{
    public partial class CalendarPage : BasePage
    {
        public CalendarPage()
        {
            InitializeComponent();
			Calendar.MinDate = DateTime.Today.AddYears (-10);
			Calendar.MaxDate = DateTime.Today.AddYears (10);

			Calendar.ShowNavigationArrows = true;
			Calendar.TodayBackgroundStyle = CalendarView.BackgroundStyle.CircleOutline;
			Calendar.NavigationArrowsColor = Color.FromHex("#44b6ae");
			Calendar.SelectedDateForegroundColor = Color.White;
			Calendar.SelectedDateBackgroundColor = Color.FromHex ("#44b6ae");
			Calendar.SelectionBackgroundStyle = CalendarView.BackgroundStyle.CircleFill;
			Calendar.DayOfWeekLabelBackgroundColor = Color.White;
			Calendar.DayOfWeekLabelForegroundColor = Color.FromHex("#44b6ae");
			Calendar.MonthTitleBackgroundColor = Color.White;
			Calendar.MonthTitleForegroundColor = Color.FromHex("#44b6ae");
        }

        private void CalendarView_OnDateSelected(object sender, DateTime e)
        {
            var vm = (CalendarViewModel) BindingContext;
            vm.Date = e;
            vm.SelectDate.Execute(null);
        }
    }
}