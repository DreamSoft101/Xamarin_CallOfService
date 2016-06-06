using System.Threading.Tasks;
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
            viewModel = (WelcomeViewModel)BindingContext;
            var username = await viewModel.GetUserName();

            await Message1.FadeTo(0, 250);
            await Message2.FadeTo(0, 250);

            Message1.Text = "Hi";
            Message2.Text = username;

            await Message1.FadeTo(100, 500);
            await Message2.FadeTo(100, 500);

            await Task.Delay(3000);

            await Message1.FadeTo(0, 500);
            await Message2.FadeTo(0, 500);

            Message1.Text = "Welcome to";
            Message2.Text = "Call of Service";

            await Message1.FadeTo(100, 500);
            await Message2.FadeTo(100, 500);

            await Task.Delay(3000);

            await Message1.FadeTo(0, 500);
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
                if(viewModel.NavigateToDashboard.CanExecute(null))
                    viewModel.NavigateToDashboard.Execute(null);
            }
        }
    }
}
