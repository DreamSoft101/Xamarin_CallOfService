using Xamarin.Forms;

namespace CallOfService.Mobile.UI
{
    public class BasePage : ContentPage
    {

        public BasePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            var viewAwareViewModel = BindingContext as ViewModelBase;
            viewAwareViewModel?.OnAppearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            var viewAwareViewModel = BindingContext as ViewModelBase;
            viewAwareViewModel?.OnDisappearing();
            base.OnDisappearing();
        }
    }
}