using System.Threading.Tasks;
using System.Windows.Input;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Welcome
{
    public class WelcomeViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        public WelcomeViewModel(IUserService userService, IAppointmentService appointmentService)
        {
            _userService = userService;
            _appointmentService = appointmentService;
            this.Subscribe<FinishedLoadingAppointments>(m =>
            {
                FinishedLoadingAppointments = true;
                //if (FinishedShowingMessages)
                //    NavigationService.NavigateToMainPage();
            });
        }

        public async Task<string> GetUserName()
        {
            return (await _userService.GetCurrentUser()).FirstName;
        }

        private bool _finishedLoadingAppointments;
        public bool FinishedLoadingAppointments
        {
            get { return _finishedLoadingAppointments; }
            set { SetPropertyValue(ref _finishedLoadingAppointments, value); }
        }

        private bool _message1;
        public bool Message1
        {
            get { return _message1; }
            set { SetPropertyValue(ref _message1, value); }
        }

        private bool _message2;
        public bool Message2
        {
            get { return _message2; }
            set { SetPropertyValue(ref _message2, value); }
        }

        public ICommand NavigateToDashboard
        {
            get
            {
                return new Command(() =>
                {
                    NavigationService.NavigateToMainPage();
                });
            }
        }

     
        public override void OnAppearing()
        {
            Task.Factory.StartNew(async () => await StartDownLoadingUserData());
        }

        private async Task StartDownLoadingUserData()
        {
            var appointmentsLoaded = await _appointmentService.RetrieveAndSaveAppointments();
            if (appointmentsLoaded)
                this.Publish(new FinishedLoadingAppointments());
        }
    }
}
