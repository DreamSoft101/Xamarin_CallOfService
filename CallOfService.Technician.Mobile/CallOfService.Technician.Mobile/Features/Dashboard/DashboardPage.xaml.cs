using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Features.Calendar;
using CallOfService.Technician.Mobile.Features.Jobs;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Technician.Mobile.Features.Dashboard
{
    public partial class DashboardPage : TabbedPage
    {
		private bool _shouldInit;
        Page _jobsPage;
        Page _calendarPage;

        public DashboardPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
			_shouldInit = true;
            this.Subscribe<NewDateSelected>(m =>
            {
                this.CurrentPage = _jobsPage;
            });
        }

        protected async override void OnAppearing()
        {
			if (!_shouldInit)
				return;
            var appointmentService = DependencyResolver.Resolve<IAppointmentService>();
            await appointmentService.RetrieveAndSaveAppointments();
            base.OnAppearing();
            _jobsPage = NavigationService.CreateAndBind<JobsPage>(DependencyResolver.Resolve<JobsViewModel>());
            _jobsPage.Title = "JOBS";
            Children.Add(_jobsPage);
            _calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
            _calendarPage.Title = "CALENDAR";
            Children.Add(_calendarPage);
			_shouldInit = false;
        }
    }
}