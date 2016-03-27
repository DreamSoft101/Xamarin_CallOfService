using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Messages;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PropertyChanged;
using PubSub;

namespace CallOfService.Technician.Mobile.Features.Welcome
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

        public void Dispose()
        {
            
        }

        public void OnAppearing()
        {
            Task.Factory.StartNew(StartDownLoadingUserData);
        }

        private void StartDownLoadingUserData()
        {
            var appointments = _appointmentService.GetAppointments();
            this.Publish(new FinishedLoadingAppointments());
        }

        public void OnDisappearing()
        {
            
        }
    }
}
