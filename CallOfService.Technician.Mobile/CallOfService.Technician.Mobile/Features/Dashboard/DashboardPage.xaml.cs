using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Core.SystemServices;
using CallOfService.Technician.Mobile.Features.Calendar;
using CallOfService.Technician.Mobile.Features.Jobs;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using Xamarin.Forms;
using System.Threading.Tasks;

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
            //NavigationPage.SetHasNavigationBar(this, false);

			Title ="Call Of Service";

			_shouldInit = true;
            this.Subscribe<NewDateSelected>(m =>
            {
                this.CurrentPage = _jobsPage;
            });

			this.Subscribe<ShowCalendarView> (m=>{
				this.CurrentPage = _calendarPage;
			});

			this.Subscribe<UserUnauthorized> (async m=> await RefreshToken());
        }

		private async Task RefreshToken(){
			var userService = DependencyResolver.Resolve<IUserService> ();
			var loginService = DependencyResolver.Resolve<ILoginService> ();
			var cred = userService.GetUserCredentials ();
			var loginResult = await loginService.Login (cred.Email, cred.Password); 
			if(!loginResult.IsSuccessful)
			{
				await NavigationService.ShowLoginPage();
			}
		}


        protected async override void OnAppearing()
        {
			if (!_shouldInit)
				return;
            var appointmentService = DependencyResolver.Resolve<IAppointmentService>();
			Task.Run(async () => await appointmentService.RetrieveAndSaveAppointments()).ConfigureAwait(false);
            base.OnAppearing();
            _jobsPage = NavigationService.CreateAndBind<JobsPage>(DependencyResolver.Resolve<JobsViewModel>());
            _jobsPage.Title = "JOBS";
            Children.Add(_jobsPage);
			Task.Run(() => {
				_calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
				_calendarPage.Title = "CALENDAR";
				Device.BeginInvokeOnMainThread(() => Children.Add(_calendarPage));
			});
			_shouldInit = false;
        }
    }
}