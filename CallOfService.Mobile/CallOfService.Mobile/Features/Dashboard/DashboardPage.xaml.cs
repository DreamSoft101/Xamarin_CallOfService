using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Features.Calendar;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using PubSub;
using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Core;
using Plugin.Connectivity;
using Page = Xamarin.Forms.Page;

namespace CallOfService.Mobile.Features.Dashboard
{
    public partial class DashboardPage : TabbedPage
    {
        private bool _shouldInit;
        private Page _jobsPage;
        private Page _calendarPage;

        public DashboardPage()
        {
            InitializeComponent();

            Title = "Call of Service";

            _shouldInit = true;
            this.Subscribe<NewDateSelected>(m =>
            {
                this.CurrentPage = _jobsPage;
            });

            this.Subscribe<ShowCalendarView>(m =>
            {
                this.CurrentPage = _calendarPage;
            });

            this.Subscribe<UserUnauthorized>(async m => await RefreshToken());
        }

        private async Task RefreshToken()
        {
            var userService = DependencyResolver.Resolve<IUserService>();
            var loginService = DependencyResolver.Resolve<ILoginService>();
            var cred = userService.GetUserCredentials();
            var loginResult = await loginService.Login(cred.Email, cred.Password);
            if (!loginResult.IsSuccessful)
            {
                await NavigationService.ShowLoginPage();
            }
        }

        protected override async void OnAppearing()
        {
            if (!_shouldInit)
                return;

            var analyticsService = DependencyResolver.Resolve<IAnalyticsService>();
            await analyticsService.Identify();
#pragma warning disable 4014
            analyticsService.Track("Loading App");
#pragma warning restore 4014

            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                var userDialogs = DependencyResolver.Resolve<IUserDialogs>();
                if (!args.IsConnected)
                    userDialogs.WarnToast("Connection went offline.");
                else
                    userDialogs.InfoToast("Connection is back.");
            };

            var appointmentService = DependencyResolver.Resolve<IAppointmentService>();
            await appointmentService.RetrieveAndSaveAppointments();
            base.OnAppearing();
            _jobsPage = NavigationService.CreateAndBind<JobsPage>(DependencyResolver.Resolve<JobsViewModel>());
            _jobsPage.Title = "JOBS";
            _jobsPage.Icon = "Jobs.png";
            Children.Add(_jobsPage);
            Device.OnPlatform(() =>
            {
                _calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
                _calendarPage.Title = "CALENDAR";
                _calendarPage.Icon = "Calendar.png";
                Children.Add(_calendarPage);
            }, () =>
            {
				_calendarPage = NavigationService.CreateAndBind<CalendarPage>(DependencyResolver.Resolve<CalendarViewModel>());
				_calendarPage.Title = "CALENDAR";
				_calendarPage.Icon = "Calendar.png";
				Children.Add(_calendarPage);
            });

            _shouldInit = false;
        }
    }
}