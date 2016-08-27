using System;
using System.Threading.Tasks;
using CallOfService.Mobile.Core.DI;
using CallOfService.Mobile.Features.JobDetails;
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

        public static Page CreateAndBind<T>(object obj) where T : Page, new()
        {
            T page;
            var weakReference = new WeakReference<T>(new T { BindingContext = obj });
            weakReference.TryGetTarget(out page);
            return page;
        }

        public static Task NavigateToDashboardScreenAsync()
        {
            return Navigation.PushAsync(CreateAndBind<MasterDetailMainPage>(new object()));
        }

        public static Task NavigateToJobDetailsAsync()
        {
            return Navigation.PushAsync(CreateAndBind<JobDetailsPage>(DependencyResolver.Resolve<JobDetailsViewModel>()));
        }

        public static Task ShowLoginPageAsync()
        {
            var vm = DependencyResolver.Resolve<LoginViewModel>();
            vm.Relogin = true;
            return Navigation.PushModalAsync(CreateAndBind<LoginPage>(vm));
        }

        public static Task ShowModalAsync<TPage, TViewModel>() where TPage : Page, new()
        {
            var vm = DependencyResolver.Resolve<TViewModel>();
            return Navigation.PushModalAsync(CreateAndBind<TPage>(vm));
        }

        public static Task ShowModalAsync<TPage, TViewModel>(TViewModel vm) where TPage : Page, new()
        {
            return Navigation.PushModalAsync(CreateAndBind<TPage>(vm));
        }

        public static async Task NavigateBackAsync()
        {
            var page = await Navigation.PopAsync();
            (page?.BindingContext as IDisposable)?.Dispose();
        }

        public static async Task NavigateToRootAsync()
        {
            await Navigation.PopToRootAsync();
        }

        public static async Task DismissAsync()
        {
            var page = await Navigation.PopModalAsync();
            (page?.BindingContext as IDisposable)?.Dispose();
        }

        public static Task NavigateToMainPageAsync()
        {
            return MainNavigation.PushAsync(CreateAndBind<MasterDetailMainPage>(new object()));
        }

        public static Task NavigateToLoginPageAsync()
        {
            return MainNavigation.PushAsync(CreateAndBind<LoginPage>(DependencyResolver.Resolve<LoginViewModel>()));
        }

        public static Task NavigateToSettingsPageAsync()
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
