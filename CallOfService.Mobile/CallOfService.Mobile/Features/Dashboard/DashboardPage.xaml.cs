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

            //this.Subscribe<NewDateSelected>(async m =>
            //{
            //    //if (CurrentPage is JobsPage)
            //    //    await ((JobsViewModel)((JobsPage)CurrentPage).BindingContext).LoadDate(m.DateTime);
            //    //else if (CurrentPage is MapPage)
            //    //    await ((MapViewModel)((MapPage)CurrentPage).BindingContext).LoadDate(m.DateTime);
            //    //else
            //    //{
            //    //    CurrentPage = _jobsPage;
            //    //    await ((JobsViewModel)((JobsPage)CurrentPage).BindingContext).LoadDate(m.DateTime);
            //    //}
            //});

            this.Subscribe<UserUnauthorized>(async m => await RefreshToken());
        }

        //protected override async void OnCurrentPageChanged()
        //{
        //    if (CurrentPage is JobsPage && _mapPage != null)
        //    {
        //        await ((JobsViewModel)((JobsPage)CurrentPage).BindingContext).LoadDate(((MapViewModel)_mapPage.BindingContext).Date);
        //    }
        //    else if (CurrentPage is MapPage && _jobsPage != null)
        //    {
        //        await ((MapViewModel)((MapPage)CurrentPage).BindingContext).LoadDate(((JobsViewModel)_jobsPage.BindingContext).Date);
        //    }
        //}

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

            Children.Clear();
            _jobsPage = NavigationService.CreateAndBind<JobsPage>(DependencyResolver.Resolve<JobsViewModel>());
            _jobsPage.Title = "JOBS";
            _jobsPage.Icon = "Jobs.png";
            Children.Add(_jobsPage);
            _mapPage = NavigationService.CreateAndBind<MapPage>(DependencyResolver.Resolve<MapViewModel>());
            _mapPage.Title = "MAP";
            _mapPage.Icon = "Calendar.png";
            Children.Add(_mapPage);

            dialog.HideLoading();

            _shouldInit = false;
        }
    }
}