using System;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.UI;
using PubSub;

namespace CallOfService.Technician.Mobile.Features.Calendar
{
    public partial class CalendarPage : BasePage
    {
        public CalendarPage()
        {
            InitializeComponent();
        }

        private void CalendarView_OnDateSelected(object sender, DateTime e)
        {
            this.Publish(new NewDateSelected(e));
        }
    }
}