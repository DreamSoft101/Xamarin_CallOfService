using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Features.Calendar;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace CallOfService.Mobile.Features.Dashboard
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
			Device.OnPlatform(() =>
			{
				_calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
				_calendarPage.Title = "CALENDAR";
				Children.Add(_calendarPage);
			}, () =>
			{
				Task.Run(() =>
				{
					_calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
					_calendarPage.Title = "CALENDAR";
					Device.BeginInvokeOnMainThread(() => Children.Add(_calendarPage));
				});
			});

			_shouldInit = false;
        }
    }
}