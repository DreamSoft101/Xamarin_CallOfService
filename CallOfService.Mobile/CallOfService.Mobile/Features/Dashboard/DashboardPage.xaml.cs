using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Features.Jobs;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using PubSub;
using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Mobile.Core;
using CallOfService.Mobile.Features.Map;
using Plugin.Connectivity;
using Page = Xamarin.Forms.Page;

namespace CallOfService.Mobile.Features.Dashboard
{
    public partial class DashboardPage : TabbedPage
    {
        private bool _shouldInit;
        private Page _jobsPage;
        private Page _mapPage;

        public DashboardPage()
        {
            InitializeComponent();

            Title = "Call of Service";

            _shouldInit = true;

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
                await NavigationService.ShowLoginPageAsync();
            }
        }

        protected override async void OnAppearing()
        {
            if (!_shouldInit)
                return;

            var dialog = DependencyResolver.Resolve<IUserDialogs>();
            dialog.ShowLoading("Loading...");

            var analyticsService = DependencyResolver.Resolve<IAnalyticsService>();
            await analyticsService.Identify();
            analyticsService.Track("Loading App");

            CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
            {
                var userDialogs = DependencyResolver.Resolve<IUserDialogs>();

                if (!args.IsConnected)
                    userDialogs.Toast("Connection went offline.");
                else
                    userDialogs.Toast("Connection is back.");
            };

            var appointmentService = DependencyResolver.Resolve<IAppointmentService>();
            await appointmentService.RetrieveAndSaveAppointments();

            base.OnAppearing();

            var dataViewModel = DependencyResolver.Resolve<JobsViewModelData>();
            var jobsViewModel = DependencyResolver.Resolve<JobsViewModel>();
            jobsViewModel.Model = dataViewModel;
            var mapViewModel = DependencyResolver.Resolve<MapViewModel>();
            mapViewModel.Model = dataViewModel;

            Children.Clear();
            _jobsPage = NavigationService.CreateAndBind<JobsPage>(jobsViewModel);
            _jobsPage.Title = "Jobs";
            _jobsPage.Icon = "Jobs.png";
            Children.Add(_jobsPage);
            _mapPage = NavigationService.CreateAndBind<MapPage>(mapViewModel);
            _mapPage.Title = "MAP";
            _mapPage.Icon = "Map.png";
            Children.Add(_mapPage);

            if (CurrentPage == null)
            {
                CurrentPage = _jobsPage;
            }

            dialog.HideLoading();

            _shouldInit = false;
        }
    }
}