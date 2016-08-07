using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Features.JobDetails;
using CallOfService.Mobile.Features.Welcome;
using Xamarin.Forms;
using CallOfService.Mobile.Features.Login;
using System.Linq;
using CallOfService.Mobile.Features.Settings;

namespace CallOfService.Mobile.Core.SystemServices
{
    public class NavigationService
    {
        public static INavigation Navigation { get; set; }
        public static INavigation MainNavigation { get; set; }

        public static Task NavigateToWelcomeScreen()
        {
            return MainNavigation.PushAsync(CreateAndBind<WelcomePage>(DependencyResolver.Resolve<WelcomeViewModel>()));
        }

        public static Page CreateAndBind<T>(object obj) where T : Page, new()
        {
            T page;
            var weakReference = new WeakReference<T>(new T { BindingContext = obj });
            weakReference.TryGetTarget(out page);
            return page;
        }

        public static Task NavigateToDashboardScreen()
        {
            return Navigation.PushAsync(CreateAndBind<MasterDetailMainPage>(new object()));
        }

        public static Task NavigateToJobDetails()
        {
            return Navigation.PushAsync(CreateAndBind<JobDetailsPage>(DependencyResolver.Resolve<JobDetailsViewModel>()));
        }

        public static Task ShowLoginPage()
        {
            var vm = DependencyResolver.Resolve<LoginViewModel>();
            vm.Relogin = true;
            return Navigation.PushModalAsync(CreateAndBind<LoginPage>(vm));
        }

        public static Task ShowModal<TPage, TViewModel>() where TPage : Page, new()
        {
            var vm = DependencyResolver.Resolve<TViewModel>();
            return Navigation.PushModalAsync(CreateAndBind<TPage>(vm));
        }

        public static async Task NavigateBack()
        {
            var page = await Navigation.PopAsync();
            (page.BindingContext as IDisposable)?.Dispose();
        }

        public static async Task Dismiss()
        {
            var page = await Navigation.PopModalAsync();
            (page.BindingContext as IDisposable)?.Dispose();
        }

        public static Task NavigateToMainPage()
        {
            return MainNavigation.PushAsync(CreateAndBind<MasterDetailMainPage>(new object()));
        }

        public static Task NavigateToLoginPage()
        {
            return MainNavigation.PushAsync(CreateAndBind<LoginPage>(DependencyResolver.Resolve<LoginViewModel>()));
        }

        public static Task NavigateToSettingsPage()
        {
            return MainNavigation.PushAsync(CreateAndBind<SettingsPage>(DependencyResolver.Resolve<SettingsViewModel>()));
        }

        public static bool IsJobDetailsPresent()
        {
            var lastPage = Navigation.NavigationStack.Last();
            return lastPage.GetType() == typeof(JobDetailsPage);
        }
    }
}
