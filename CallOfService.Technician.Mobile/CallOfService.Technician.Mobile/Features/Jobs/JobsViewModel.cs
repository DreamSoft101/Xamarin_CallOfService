using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PropertyChanged;
using Xamarin.Forms;
using PubSub;
using CallOfService.Technician.Mobile.Core.SystemServices;

namespace CallOfService.Technician.Mobile.Features.Jobs
{
    [ImplementPropertyChanged]
    public class JobsViewModel : IViewAwareViewModel
    {
        private readonly IAppointmentService _appointmentService;

        public JobsViewModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
            Appointments = new ObservableCollection<AppointmentViewModel>();
			this.Subscribe<JobSelected> (async m => {
				await NavigationService.NavigateToJobDetails ();
				this.Publish(new ViewJobDetails(m.Appointment.JobId));
			});
        }

        public ObservableCollection<AppointmentViewModel> Appointments { get; set; }

        public DateTime Date { get; set; }
        
        public void Dispose()
        {   
            Appointments.Clear();
        }

        public async void OnAppearing()
        {
            Date = DateTime.Today;
            var appointments = await _appointmentService.AppointmentsByDay(Date);
            Appointments.Clear();
            foreach (var appointment in appointments)
            {
                Appointments.Add(new AppointmentViewModel
                {
                    Title = appointment.Title,
                    Location = appointment.Title,
                    StartTimeEndTimeFormated =  $"{appointment.StartString} - {appointment.EndString}",
					JobId = appointment.JobId
                });
            }
        }

        public void OnDisappearing()
        {
        }
    }

    [ImplementPropertyChanged]
    public class AppointmentViewModel
    {
        public string Title { get; set; }
        public string StartTimeEndTimeFormated { get; set; }
        public string Location { get; set; }
		public int JobId { get; set;}
		public ICommand ViewDetails { get { return new Command (() => this.Publish (new JobSelected (this))); } }
    }
}