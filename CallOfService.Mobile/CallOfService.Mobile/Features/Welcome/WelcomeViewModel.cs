using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CallOfService.Mobile.Core.SystemServices;
using CallOfService.Mobile.Domain;
using CallOfService.Mobile.Messages;
using CallOfService.Mobile.Services.Abstracts;
using CallOfService.Mobile.UI;
using PropertyChanged;
using PubSub;
using Xamarin.Forms;

namespace CallOfService.Mobile.Features.Welcome
{

    [ImplementPropertyChanged]
    public class WelcomeViewModel : IViewAwareViewModel
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;

        public WelcomeViewModel(IUserService userService, IAppointmentService appointmentService)
        {
            _userService = userService;
            _appointmentService = appointmentService;
            this.Subscribe<FinishedLoadingAppointments>((m) =>
            {
                FinishedLoadingAppointments = true;
                if (FinishedShowingMessages)
                    ShowStartButton = true;
            });
        }

		public async Task<string> GetUserName(){
			return (await _userService.GetCurrentUser ()).FirstName;
		}

        public bool FinishedLoadingAppointments { get; set; }
        public bool FinishedShowingMessages { get; set; }
        public bool ShowStartButton { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }

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

        public void Dispose()
        {
            
        }

        public void OnAppearing()
        {
            Task.Factory.StartNew(async () => await StartDownLoadingUserData());
        }

        private async Task StartDownLoadingUserData()
        {
            var appointmentsLoaded = await _appointmentService.RetrieveAndSaveAppointments();
            if(appointmentsLoaded)
                this.Publish(new FinishedLoadingAppointments());
        }

        public void OnDisappearing()
        {
            
        }
    }
}
