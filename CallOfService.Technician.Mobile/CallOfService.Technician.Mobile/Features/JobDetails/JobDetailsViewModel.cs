using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CallOfService.Technician.Mobile.Core.DI;
using CallOfService.Technician.Mobile.Domain;
using CallOfService.Technician.Mobile.Services.Abstracts;
using CallOfService.Technician.Mobile.UI;
using PubSub;
using Xamarin.Forms;
using System.Windows.Input;
using CallOfService.Technician.Mobile.Core.SystemServices;
using PropertyChanged;

namespace CallOfService.Technician.Mobile.Features.JobDetails
{
	[ImplementPropertyChanged]
    public class JobDetailsViewModel : IViewAwareViewModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserDialogs _userDialogs;

        public JobDetailsViewModel(IAppointmentService appointmentService, IUserDialogs userDialogs)
        {
            _appointmentService = appointmentService;
            _userDialogs = userDialogs;
            Notes = new ObservableCollection<NoteViewModel>();
            CustomFields = new ObservableCollection<string>();
            this.Subscribe<ViewJobDetails>(async m => await LoadJobeDetails(m.JobId));
        }
		public ICommand NavigateBack {
			get { 
				return new Command (async ()=> {
					await NavigationService.NavigateBack();
				});
			}
		}
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public string Contact { get; set; }
        public string JobNumber { get; set; }
        public string Status { get; set; }
        public string Strata { get; set; }
        public ObservableCollection<NoteViewModel> Notes { get; set; }
	    public ObservableCollection<string> CustomFields  { get; set; }
        public string Custom { get; set; }

        private async Task LoadJobeDetails(int jobId)
        {
            _userDialogs.ShowLoading("Loading Job Details");
            Appointment appointment = await _appointmentService.GetAppointmentByJobId(jobId);
            var job = await _appointmentService.GetJobById(jobId);
            if (job.Date != null) Date = job.Date.Value;
            StartTime = appointment.Start.TimeOfDay;
            EndTime = appointment.End.TimeOfDay;
            Location = appointment.Location;
            Title = appointment.Title;
            Contact = job.ContactName;
            JobNumber = job.Id.ToString();
            Status = job.StatusDescription;
            Notes.Clear();
			foreach (var note in job.Notes) {
				var noteViewModel = DependencyResolver.Resolve<NoteViewModel> ();
				noteViewModel.LoadNote (note);
				Notes.Add (noteViewModel);
			}

            var strings = job.CustomFields.Select(c => string.Concat(c.Key, " : ", c.Value)).ToList();
            CustomFields.Clear();
            foreach (var s in strings)
            {
                CustomFields.Add(s);
            }

            _userDialogs.HideLoading();
        }
        
	    public void Dispose()
        {
            Notes.Clear();
            CustomFields.Clear();
        }

        public void OnAppearing()
        {

        }

        public void OnDisappearing()
        {
            Notes.Clear();
            CustomFields.Clear();
        }
    }

	[ImplementPropertyChanged]
    public class NoteViewModel
    {
        private readonly IAppointmentService _appointmentService;

        public NoteViewModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
			ThumbnilImageSources = new ObservableCollection<ImageSource>();
        }

        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<ImageSource> ThumbnilImageSources { get; set; }

        public void LoadNote(Note note)
        {
            Description = note.Description;
            Date = note.Timestamp.DateTime;
            foreach (var fileReference in note.Files)
            {
                ThumbnilImageSources.Add(new UriImageSource
                {
                    Uri = _appointmentService.GetFileUri(fileReference, true)
                });
            }
        }
    }
}