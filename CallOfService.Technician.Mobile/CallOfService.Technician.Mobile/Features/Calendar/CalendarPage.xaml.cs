using System;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using XLabs.Forms.Controls;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Calendar
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
			Calendar.NavigationArrowsColor = Color.FromHex ("#44b6ae");
			Calendar.SelectedDateForegroundColor = Color.White;
			Calendar.SelectedDateBackgroundColor = Color.FromHex ("#44b6ae");
			Calendar.SelectionBackgroundStyle = CalendarView.BackgroundStyle.CircleFill;
			Calendar.DayOfWeekLabelBackgroundColor = Color.FromHex ("#44b6ae");
			Calendar.DayOfWeekLabelForegroundColor = Color.White;
			Calendar.MonthTitleBackgroundColor = Color.FromHex ("#44b6ae");
			Calendar.MonthTitleForegroundColor = Color.White;
        }

        private void CalendarView_OnDateSelected(object sender, DateTime e)
        {
            this.Publish(new NewDateSelected(e));
        }
    }
}