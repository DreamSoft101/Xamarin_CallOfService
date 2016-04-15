using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Features.Calendar;
using CallOfService.Technician.Mobile.Features.Jobs;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Dashboard
{
    public partial class DashboardPage : TabbedPage
    {
		private bool _shouldInit;

		public DashboardPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
			_shouldInit = true;
        }

        protected async override void OnAppearing()
        {
			if (!_shouldInit)
				return;
            var appointmentService = DependencyResolver.Resolve<IAppointmentService>();
            await appointmentService.RetrieveAndSaveAppointments();
            base.OnAppearing();
            var jobsPage = NavigationService.CreateAndBind<JobsPage>(DependencyResolver.Resolve<JobsViewModel>());
            jobsPage.Title = "JOBS";
            Children.Add(jobsPage);
            var calendarPage =
                NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
            calendarPage.Title = "CALENDAR";
            Children.Add(calendarPage);
			_shouldInit = false;
        }
    }
}