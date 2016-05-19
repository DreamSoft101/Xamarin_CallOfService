using System.Threading.Tasks;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.UI;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Welcome
{
    public partial class WelcomePage : BasePage
    {
        WelcomeViewModel viewModel;

        public WelcomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            viewModel = BindingContext as WelcomeViewModel;
            var username = await viewModel.GetUserName();

            Message1.FadeTo(0, 250);
            await Message2.FadeTo(0, 250);

            Message1.Text = "HI";
            Message2.Text = username;

            Message1.FadeTo(100, 500);
            await Message2.FadeTo(100, 500);

            await Task.Delay(3000);

            Message1.FadeTo(0, 500);
            await Message2.FadeTo(0, 500);

            Message1.Text = "Welcome to";
            Message2.Text = "Call of Service";

            Message1.FadeTo(100, 500);
            await Message2.FadeTo(100, 500);

            await Task.Delay(3000);

            Message1.FadeTo(0, 500);
            await Message2.FadeTo(0, 500);

            Message1.Text = "Loading configuration...";
            Message2.Text = "";

            await Message1.FadeTo(100, 500);

            viewModel.FinishedShowingMessages = true;
            if (viewModel.FinishedLoadingAppointments)
            {
                await Message1.FadeTo(0, 500);
                Message1.Text = "All Done !";
                await Message1.FadeTo(100, 500);
                //viewModel.ShowStartButton = true;
                //ToDo: Navigate to dashboard
                if(viewModel.NavigateToDashboard.CanExecute(null))
                    viewModel.NavigateToDashboard.Execute(null);
            }
        }
    }
}
